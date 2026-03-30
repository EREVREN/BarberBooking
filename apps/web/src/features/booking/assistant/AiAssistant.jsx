import { useEffect, useRef, useState } from "react";
import { reserveAi } from "@/shared/api/ai";
import { useBooking } from "../BookingProvider";
import { createCustomer } from "@/shared/api/customers";
import { confirmBooking } from "@/shared/api/bookings";
import fetchAvailableSlots from "@/shared/api/availability";
import { fetchBarbers } from "@/shared/api/barbers";
import { fetchBarberServices } from "@/shared/api/services";

function nowIso() {
  return new Date().toISOString();
}

function isConfirm(text) {
  const lowered = text.trim().toLowerCase();
  return (
    lowered === "confirm" ||
    lowered.startsWith("confirm ") ||
    lowered === "onayla" ||
    lowered.startsWith("onayla ") ||
    lowered === "onay" ||
    lowered === "confirm."
  );
}

function isBookingDetails(text) {
  const lowered = text.trim().toLowerCase();
  return lowered.includes("booking details") || lowered.includes("return booking details");
}

function isAvailabilityInquiry(text) {
  const lowered = text.trim().toLowerCase();
  return (
    lowered.includes("availability") ||
    lowered.includes("available") ||
    lowered.includes("any slot") ||
    lowered.includes("any slots") ||
    lowered.includes("free slot") ||
    lowered.includes("free slots") ||
    lowered.includes("is there any") ||
    lowered.includes("can i reserve")
  );
}

function isAffirmative(text) {
  const lowered = text.trim().toLowerCase();
  return (
    lowered === "correct" ||
    lowered === "doğru" ||
    lowered === "dogru" ||
    lowered === "evet" ||
    lowered === "tamam" ||
    lowered === "okey" ||
    lowered === "yes" ||
    lowered === "yeah" ||
    lowered === "yep" ||
    lowered === "ok" ||
    lowered === "okay" ||
    lowered === "right" ||
    lowered === "true"
  );
}

function pad2(value) {
  return String(value).padStart(2, "0");
}

function toYmd(date) {
  return `${date.getFullYear()}-${pad2(date.getMonth() + 1)}-${pad2(date.getDate())}`;
}

function normalize(value) {
  return String(value ?? "")
    .toLowerCase()
    .replace(/[^\p{L}\p{N}\s]/gu, " ")
    .replace(/\s+/g, " ")
    .trim();
}

function parseBarberNameFromText(text) {
  // Capture "barber <name>" anywhere, stopping at common separators/keywords.
  const lowered = text.toLowerCase();
  const match = lowered.match(
    /\bbarber\s+(?<name>[a-z\u00c0-\u024f\s]+?)(?=\s+(for|with|tomorrow|today|on|at|please|service)\b|[,.!?]|$)/i
  );
  return match?.groups?.name ? match.groups.name.trim() : null;
}

function parseRequestedDateTime(text) {
  const lowered = text.toLowerCase();

  // Date: explicit YYYY-MM-DD
  const ymd = lowered.match(/\b(\d{4})-(\d{2})-(\d{2})\b/);
  const now = new Date();
  let date = null;
  if (ymd) {
    date = `${ymd[1]}-${ymd[2]}-${ymd[3]}`;
  } else if (lowered.includes("tomorrow")) {
    const d = new Date(now);
    d.setDate(d.getDate() + 1);
    date = toYmd(d);
  } else if (lowered.includes("today")) {
    date = toYmd(now);
  }

  // Time: prefer explicit HH:MM / HH.MM to avoid matching years like 2026.
  // Accept:
  // - "10:00", "10.00"
  // - "at 10", "at 10:00", "at 10.00"
  let timeMatch =
    lowered.match(/\b(\d{1,2})[:.](\d{2})\b/) ??
    lowered.match(/\bat\s+(\d{1,2})(?:[:.](\d{2}))?\b/);
  let time = null;
  if (timeMatch) {
    const h = Number(timeMatch[1]);
    const m = Number(timeMatch[2] ?? "0");
    if (Number.isFinite(h) && Number.isFinite(m) && h >= 0 && h <= 23 && m >= 0 && m <= 59) {
      time = `${pad2(h)}:${pad2(m)}`;
    }
  }

  if (!date || !time) return null;
  return { date, time };
}

function parseEmail(text) {
  const match = text.match(/[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}/i);
  return match ? match[0] : null;
}

function isValidEmail(email) {
  // Simple sanity check; backend currently allows null but UX wants a real address.
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(String(email ?? "").trim());
}

function parsePhoneNumber(text) {
  // Capture a likely phone number and reject timestamps/dates like "2026-03-13T10:00:00+03:00"
  // that otherwise match naive digit/dash patterns.
  const lowered = String(text ?? "").toLowerCase();
  const match = lowered.match(/(?:phone|tel|gsm|mobile)\s*(?:is|:)?\s*(?<num>\+?[\d\s().-]{8,})/i);
  let candidate = match?.groups?.num ? match.groups.num.trim() : null;

  // Also accept "just digits" as a phone number (common user behavior), but only if the whole
  // message looks like a phone and not a timestamp.
  if (!candidate) {
    const raw = String(text ?? "").trim();
    if (/^\+?\d{10,15}$/.test(raw)) {
      candidate = raw;
    }
  }
  if (!candidate) return null;
  if (candidate.includes("t") || candidate.includes(":")) return null;
  if (/^\d{4}-\d{2}-\d{2}$/.test(candidate)) return null;

  const digits = candidate.replace(/[^\d+]/g, "");
  const normalized = digits.startsWith("+") ? `+${digits.slice(1).replace(/\D/g, "")}` : digits.replace(/\D/g, "");
  const digitCount = normalized.startsWith("+") ? normalized.length - 1 : normalized.length;
  if (digitCount < 10) return null;
  if (normalized.length > 20) return null;
  return normalized;
}

function parseBarberNameHint(text) {
  const lowered = text.toLowerCase();

  // "by <name>" (common phrasing), but don't include trailing intent text.
  const byMatch = lowered.match(
    /\bby\s+(?<name>[a-z\u00c0-\u024f\s]+?)(?=\s+(barber|for|with|tomorrow|today|on|at)\b|[,.!?]|$)/i
  );
  if (byMatch?.groups?.name) {
    return byMatch.groups.name.replace(/^\s*barber\s+/i, "").trim();
  }

  // "barber <name>" anywhere in the text.
  return parseBarberNameFromText(lowered);
}

function parseServiceHint(text) {
  const lowered = text.toLowerCase();
  if (lowered.includes("haircut")) return "haircut";
  if (lowered.includes("beard")) return "beard";
  return null;
}

function parseCustomerName(text) {
  const t = String(text ?? "").trim();
  if (!t) return null;

  // Prefer explicit identity phrases.
  // Examples:
  // - "I am Ali Evren"
  // - "my name is Ali Evren"
  // - "customername: Ali Evren"
  const explicit = t.match(
    /\b(?:i\s*am|i'm|im|my\s+name\s+is|name\s+is|customer\s*name|customername|customername:|customer\s*name:|name:|benim\s+ismim|ismim|adim|adım)\s*[:]?[\s]+(?<first>[a-z\u00c0-\u024f\u0130\u0131\u015e\u015f\u011e\u011f\u00dc\u00fc\u00d6\u00f6\u00c7\u00e7]+)\s+(?<last>[a-z\u00c0-\u024f\u0130\u0131\u015e\u015f\u011e\u011f\u00dc\u00fc\u00d6\u00f6\u00c7\u00e7]+)\b/i
  );
  if (explicit?.groups?.first && explicit?.groups?.last) {
    const firstname = explicit.groups.first;
    const lastname = explicit.groups.last;
    return { firstname, lastname, name: `${firstname} ${lastname}` };
  }

  // Fallback: message is just "First Last". Avoid misclassifying service/barber/time sentences as a name.
  const lowered = t.toLowerCase();
  if (
    lowered.includes("barber") ||
    lowered.includes("haircut") ||
    lowered.includes("beard") ||
    lowered.includes("tomorrow") ||
    lowered.includes("today") ||
    /\d/.test(lowered) ||
    lowered.includes("@") ||
    lowered.includes(":")
  ) {
    return null;
  }

  const simple = t.match(
    /^(?<first>[a-z\u00c0-\u024f\u0130\u0131\u015e\u015f\u011e\u011f\u00dc\u00fc\u00d6\u00f6\u00c7\u00e7]+)\s+(?<last>[a-z\u00c0-\u024f\u0130\u0131\u015e\u015f\u011e\u011f\u00dc\u00fc\u00d6\u00f6\u00c7\u00e7]+)$/
  );
  if (simple?.groups?.first && simple?.groups?.last) {
    const firstname = simple.groups.first;
    const lastname = simple.groups.last;
    return { firstname, lastname, name: `${firstname} ${lastname}` };
  }

  return null;
}

function renderMessage(msg) {
  return (
    <div
      key={msg.id}
      className={
        msg.role === "user"
          ? "self-end bg-black text-white"
          : "self-start bg-gray-100 text-gray-900"
      }
      style={{
        maxWidth: "85%",
        borderRadius: 14,
        padding: "10px 12px",
        whiteSpace: "pre-wrap",
        wordBreak: "break-word",
      }}
    >
      <div style={{ fontSize: 14, lineHeight: 1.35 }}>{msg.text}</div>
      <div style={{ fontSize: 11, opacity: 0.65, marginTop: 6 }}>{msg.at}</div>
    </div>
  );
}

export function AiAssistant() {
  const { booking, setBooking } = useBooking();
  const [input, setInput] = useState("");
  const [isSending, setIsSending] = useState(false);
  const listRef = useRef(null);

  const [messages, setMessages] = useState([]);
  const [pending, setPending] = useState(null);
  const [hints, setHints] = useState({});
  const [previousResponseId, setPreviousResponseId] = useState(null);
  const [bookingMode, setBookingMode] = useState(false);

  const barbersCacheRef = useRef(null);
  const servicesCacheRef = useRef(new Map());

  function clearAssistantState() {
    setMessages([]);
    setInput("");
    setPending(null);
    setHints({});
    setPreviousResponseId(null);
    setBookingMode(false);
  }

  // Intentionally no browser persistence: assistant state is per page load.

  async function getAllBarbers() {
    if (Array.isArray(barbersCacheRef.current) && barbersCacheRef.current.length) return barbersCacheRef.current;
    const list = await fetchBarbers();
    barbersCacheRef.current = Array.isArray(list) ? list : [];
    return barbersCacheRef.current;
  }

  async function getAllServices(barberId) {
    if (!barberId) return [];
    const key = String(barberId);
    if (servicesCacheRef.current.has(key)) return servicesCacheRef.current.get(key);
    const services = await fetchBarberServices(barberId);
    const normalized = Array.isArray(services) ? services : [];
    servicesCacheRef.current.set(key, normalized);
    return normalized;
  }

  function formatBarberOptions(list) {
    const top = (Array.isArray(list) ? list : []).slice(0, 6);
    if (!top.length) return "";
    return `Barbers: ${top.map((b) => b.name).join(", ")}.`;
  }

  function formatServiceOptions(list) {
    const top = (Array.isArray(list) ? list : []).slice(0, 6);
    if (!top.length) return "";
    return `Services: ${top
      .map((s) => {
        const mins = s?.durationMinutes ? `${s.durationMinutes}m` : "";
        return mins ? `${s.name} (${mins})` : s.name;
      })
      .join(", ")}.`;
  }

  function missingCustomerField(snapshot) {
    const nameOk = Boolean(snapshot?.customer?.firstname && snapshot?.customer?.lastname) || Boolean(snapshot?.customer?.name);
    if (!nameOk) return "name";

    const email = String(snapshot?.customer?.email ?? "").trim();
    if (!email || !isValidEmail(email)) return "email";

    const phone = String(snapshot?.customer?.phoneNumber ?? "").trim();
    const normalizedPhone = phone.startsWith("+") ? `+${phone.slice(1).replace(/\D/g, "")}` : phone.replace(/\D/g, "");
    const phoneDigits = normalizedPhone.startsWith("+") ? normalizedPhone.length - 1 : normalizedPhone.length;
    if (!normalizedPhone || phoneDigits < 10 || normalizedPhone.length > 20) return "phone";

    return null;
  }

  async function promptForNext(snapshot, pendingSnapshot) {
    if (!snapshot?.barber?.id) {
      const barbers = await getAllBarbers();
      const opts = formatBarberOptions(barbers);
      return `Which barber would you like?${opts ? ` ${opts}` : ""}`;
    }

    if (!snapshot?.service?.id) {
      const services = await getAllServices(snapshot.barber.id);
      const opts = formatServiceOptions(services);
      return `Great, ${snapshot?.barber?.name ?? "that barber"}. What service would you like?${opts ? ` ${opts}` : ""}`;
    }

    if (!pendingSnapshot?.date || !pendingSnapshot?.time) {
      // If UI already has a selected slot, we can move forward.
      if (snapshot?.slot?.start && snapshot?.slot?.end) {
        if (!bookingMode) {
          return `I found an available slot. Would you like me to confirm it? Type "confirm" or "onayla".`;
        }
        const missing = missingCustomerField(snapshot);
        if (missing) {
          if (missing === "name") return `Before I confirm, what is your name? (Example: "Benim ismim Betül Evren")`;
          if (missing === "email") return `Thanks. What is your email? (Example: "betul@gmail.com")`;
          return `And your phone number? (Example: "+90555 444 3333")`;
        }
        return `Everything is ready. Type "confirm" to finalize the booking.`;
      }
      return `What day and time would you like? You can say "tomorrow at 10:00".`;
    }

    // We have a requested day/time; if slot isn't chosen yet, encourage availability check/selection.
    if (!snapshot?.slot?.start || !snapshot?.slot?.end) {
      return `Okay. I’ll check availability for ${pendingSnapshot.date} at ${pendingSnapshot.time}.`;
    }

    if (!bookingMode) {
      return `I can do ${pendingSnapshot.date} at ${pendingSnapshot.time}. Should I confirm it? Type "confirm" or "onayla".`;
    }

    const missing = missingCustomerField(snapshot);
    if (missing) {
      if (missing === "name") return `Before I confirm, what is your name? (Example: "Benim ismim Betül Evren")`;
      if (missing === "email") return `Thanks. What is your email? (Example: "betul@gmail.com")`;
      return `And your phone number? (Example: "+90555 444 3333")`;
    }

    return `Everything is ready. Type "confirm" to finalize the booking.`;
  }

  useEffect(() => {
    if (!listRef.current) return;
    listRef.current.scrollTop = listRef.current.scrollHeight;
  }, [messages.length]);

  function push(role, text) {
    setMessages((prev) => [
      ...prev,
      {
        id: `${Date.now()}-${Math.random().toString(16).slice(2)}`,
        role,
        text,
        at: new Date().toLocaleString(),
      },
    ]);
  }

  function deriveHintsFromMessages() {
    const derivedHints = {};
    const derivedPending = {};
    let derivedCustomer = null;

    // Scan most recent user messages first.
    const recent = [...messages]
      .filter((m) => m?.role === "user" && typeof m.text === "string")
      .slice(-30);

    for (const m of recent) {
      const t = m.text;
      const email = parseEmail(t);
      const name = parseCustomerName(t);
      const phone = parsePhoneNumber(t);
      const barberNameHint = parseBarberNameHint(t);
      const serviceHint = parseServiceHint(t);
      const dt = parseRequestedDateTime(t);

      if (email) {
        derivedCustomer = { ...(derivedCustomer ?? {}), email };
      }
      if (name) {
        derivedCustomer = { ...(derivedCustomer ?? {}), firstname: name.firstname, lastname: name.lastname, name: name.name };
      }
      if (phone) {
        derivedCustomer = { ...(derivedCustomer ?? {}), phoneNumber: phone };
      }
      if (barberNameHint && !derivedHints.barberName) {
        derivedHints.barberName = barberNameHint;
        derivedPending.barberName = barberNameHint;
      }
      if (serviceHint && !derivedHints.serviceName) {
        derivedHints.serviceName = serviceHint;
        derivedPending.serviceName = serviceHint;
      }
      if (dt && !derivedPending.date && !derivedPending.time) {
        derivedPending.date = dt.date;
        derivedPending.time = dt.time;
      }
    }

    return { derivedHints, derivedPending, derivedCustomer };
  }

  async function ensureCustomerIdFor(snapshot) {
    const existing = snapshot?.customer?.id;
    if (existing) return existing;

    const created = await createCustomer(snapshot.customer);
    const id = created?.id ?? created?.customerId ?? created?.Id ?? null;
    if (!id) throw new Error("Customer created but no customerId returned.");

    setBooking((prev) => ({
      ...prev,
      customer: { ...prev.customer, id },
    }));
    return id;
  }

  async function confirmFromSnapshot(snapshot) {
    if (!snapshot?.barber?.id) throw new Error("Select a barber first.");
    if (!snapshot?.service?.id) throw new Error("Select a service first.");
    if (!snapshot?.slot?.start || !snapshot?.slot?.end) throw new Error("Select a time slot first.");

    const nameOk = Boolean(snapshot?.customer?.firstname && snapshot?.customer?.lastname) || Boolean(snapshot?.customer?.name);
    if (!nameOk) {
      throw new Error('Enter customer name (example: "I am Ali Evren").');
    }

    const email = String(snapshot?.customer?.email ?? "").trim();
    if (!email || !isValidEmail(email)) {
      throw new Error('Enter customer email (example: "email: ali@gmail.com").');
    }

    // Backend requires PhoneNumber (unique, non-null) for customer creation.
    const rawPhone = String(snapshot?.customer?.phoneNumber ?? "").trim();
    const normalizedPhone = rawPhone.startsWith("+")
      ? `+${rawPhone.slice(1).replace(/\D/g, "")}`
      : rawPhone.replace(/\D/g, "");
    const digitCount = normalizedPhone.startsWith("+") ? normalizedPhone.length - 1 : normalizedPhone.length;
    if (!normalizedPhone || digitCount < 10 || normalizedPhone.length > 20) {
      throw new Error('Enter customer phone number (example: "phone: +90555 123 4567").');
    }
    if (normalizedPhone !== rawPhone) {
      setBooking((prev) => ({ ...prev, customer: { ...prev.customer, phoneNumber: normalizedPhone } }));
      snapshot = { ...snapshot, customer: { ...snapshot.customer, phoneNumber: normalizedPhone } };
    }

    const customerId = await ensureCustomerIdFor(snapshot);

    await confirmBooking({
      barberId: snapshot.barber.id,
      serviceId: snapshot.service.id,
      customerId,
      startTime: snapshot.slot.start,
      endTime: snapshot.slot.end,
    });
  }

  async function maybeApplyScheduledTime(scheduledTimeIso) {
    if (!scheduledTimeIso) return;
    if (!booking?.barber?.id || !booking?.service?.durationMinutes) return;

    const date = scheduledTimeIso.slice(0, 10);
    const availability = await fetchAvailableSlots({
      barberId: booking.barber.id,
      serviceDurationMinutes: booking.service.durationMinutes,
      date,
    });

    const slots = Array.isArray(availability) ? availability : (availability?.slots ?? []);
    const match = slots.find((s) => s?.start === scheduledTimeIso && s?.isAvailable);
    if (!match) return;

    setBooking((prev) => ({ ...prev, date, slot: match }));
  }

  async function findSlotForPending(bookingSnapshot, p) {
    if (!p) return;
    if (!bookingSnapshot?.barber?.id || !bookingSnapshot?.service?.durationMinutes) return;
    if (!p.date || !p.time) return;

    const availability = await fetchAvailableSlots({
      barberId: bookingSnapshot.barber.id,
      serviceDurationMinutes: bookingSnapshot.service.durationMinutes,
      date: p.date,
    });

    const slots = Array.isArray(availability) ? availability : (availability?.slots ?? []);
    const match = slots.find((s) => {
      if (!s?.isAvailable || typeof s.start !== "string") return false;
      if (!s.start.startsWith(p.date)) return false;
      const t = s.start.split("T")[1]?.slice(0, 5);
      return t === p.time;
    });

    if (match) return { slot: match, slots };

    const first = slots.find((s) => s?.isAvailable);
    if (first) return { slot: first, slots, usedFallback: true };
    return { slot: null, slots };
  }

  // When the user later selects barber/service, automatically resolve any pending time request.
  useEffect(() => {
    if (!pending) return;
    if (!booking?.barber?.id || !booking?.service?.durationMinutes) return;
    if (booking?.slot?.start) return; // don't override explicit selection

    (async () => {
      const found = await findSlotForPending(booking, pending);
      if (!found) return;
      if (!found.slot) {
        push("assistant", `No available slots on ${pending.date}. Pick another date.`);
        return;
      }
      setBooking((prev) => ({ ...prev, date: pending.date, slot: found.slot }));
      if (found.usedFallback) {
        const nextTime = typeof found.slot.start === "string" ? found.slot.start.split("T")[1]?.slice(0, 5) : "";
        push(
          "assistant",
          `That time (${pending.date} ${pending.time}) isn't available. The next available time is ${nextTime}. Would you like to book ${nextTime}?`
        );
      } else {
        push("assistant", `Good news. ${pending.date} at ${pending.time} is available. Would you like me to confirm it? Type "confirm" or "onayla".`);
      }
    })();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [booking?.barber?.id, booking?.service?.durationMinutes]);

  async function hydrateBookingFromHints(overrides) {
    const hintsSnapshot = overrides?.hints ?? hints;
    const pendingSnapshot = overrides?.pending ?? pending;

    let nextBooking = overrides?.baseBooking ?? booking;

    const barberName = hintsSnapshot?.barberName ?? pendingSnapshot?.barberName ?? null;
    if (!nextBooking?.barber?.id && barberName) {
      const list = await fetchBarbers();
      const target = normalize(barberName);
      const found =
        list.find((b) => normalize(b.name) === target) ??
        list.find((b) => normalize(b.name).includes(target)) ??
        null;

      if (found) {
        nextBooking = {
          ...nextBooking,
          barber: { id: found.id, name: found.name },
          service: null,
          slot: null,
        };
      }
    }

    const serviceHint = hintsSnapshot?.serviceName ?? pendingSnapshot?.serviceName ?? null;
    if (nextBooking?.barber?.id && !nextBooking?.service?.id && serviceHint) {
      const services = await fetchBarberServices(nextBooking.barber.id);
      const target = normalize(serviceHint);
      const found =
        services.find((s) => normalize(s.name) === target) ??
        services.find((s) => normalize(s.name).includes(target)) ??
        null;

      if (found) {
        nextBooking = {
          ...nextBooking,
          service: {
            id: found.id,
            name: found.name,
            durationMinutes: found.durationMinutes,
            price: found.price,
          },
          slot: null,
        };
      }
    }

    if (pendingSnapshot?.date && !nextBooking?.date) {
      nextBooking = { ...nextBooking, date: pendingSnapshot.date, slot: null };
    }

    if (pendingSnapshot?.date && pendingSnapshot?.time && nextBooking?.barber?.id && nextBooking?.service?.durationMinutes && !nextBooking?.slot) {
      const found = await findSlotForPending(nextBooking, pendingSnapshot);
      if (found?.slot) {
        nextBooking = { ...nextBooking, date: pendingSnapshot.date, slot: found.slot };
      }
    }

    if (nextBooking !== booking) {
      setBooking(nextBooking);
    }

    return nextBooking;
  }

  function bookingSummaryText(snapshot) {
    const b = snapshot?.barber?.name ? `${snapshot.barber.name} (${snapshot.barber.id})` : "none";
    const s = snapshot?.service?.name ? `${snapshot.service.name} (${snapshot.service.id})` : "none";
    const d = snapshot?.date ?? "none";
    const slot = snapshot?.slot?.start ? `${snapshot.slot.start} - ${snapshot.slot.end}` : "none";
    const email = snapshot?.customer?.email ?? "none";
    const phone = snapshot?.customer?.phoneNumber ?? "none";
    return [
      `Barber: ${b}`,
      `Service: ${s}`,
      `Date: ${d}`,
      `Slot: ${slot}`,
      `Customer email: ${email}`,
      `Customer phone: ${phone}`,
    ].join("\n");
  }

  async function send() {
    const text = input.trim();
    if (!text || isSending) return;

    setInput("");
    push("user", text);
    setIsSending(true);

    try {
      let baseBooking = booking;

      const { derivedHints, derivedPending, derivedCustomer } = deriveHintsFromMessages();

      if (isBookingDetails(text)) {
        const hydrated = await hydrateBookingFromHints({ baseBooking });
        push("assistant", bookingSummaryText(hydrated));
        return;
      }

      const email = parseEmail(text);
      const name = parseCustomerName(text);
      const phone = parsePhoneNumber(text);
      const barberNameHint = parseBarberNameHint(text);
      const serviceHint = parseServiceHint(text);

      if (email || name || phone) {
        baseBooking = {
          ...baseBooking,
          customer: {
            ...baseBooking.customer,
            ...(name ? { firstname: name.firstname, lastname: name.lastname, name: name.name } : null),
            ...(email ? { email } : null),
            ...(phone ? { phoneNumber: phone } : null),
          },
        };
        setBooking((prev) => ({
          ...prev,
          customer: {
            ...prev.customer,
            ...(name ? { firstname: name.firstname, lastname: name.lastname, name: name.name } : null),
            ...(email ? { email } : null),
            ...(phone ? { phoneNumber: phone } : null),
          },
        }));
      }

      const parsed = parseRequestedDateTime(text);
      const nextPending = { ...(pending ?? {}) };
      if (parsed) {
        nextPending.date = parsed.date;
        nextPending.time = parsed.time;
        setPending((prev) => ({ ...(prev ?? {}), ...parsed }));
        // Set the date immediately so slot fetching later uses the right day.
        if (booking?.date !== parsed.date) {
          baseBooking = { ...baseBooking, date: parsed.date, slot: null };
          setBooking((prev) => ({ ...prev, date: parsed.date, slot: null }));
        }
      }

      const nextHints = { ...(hints ?? {}) };
      if (barberNameHint || serviceHint) {
        if (barberNameHint) {
          nextHints.barberName = barberNameHint;
          nextPending.barberName = barberNameHint;
        }
        if (serviceHint) {
          nextHints.serviceName = serviceHint;
          nextPending.serviceName = serviceHint;
        }

        setHints((prev) => ({
          ...(prev ?? {}),
          ...(barberNameHint ? { barberName: barberNameHint } : null),
          ...(serviceHint ? { serviceName: serviceHint } : null),
        }));
        setPending((prev) => ({
          ...(prev ?? {}),
          ...(barberNameHint ? { barberName: barberNameHint } : null),
          ...(serviceHint ? { serviceName: serviceHint } : null),
        }));
      }

      if (isConfirm(text)) {
        setBookingMode(true);
        // If user says "confirm ..." include any derived hints/customer data from chat history.
        if (derivedCustomer) {
          baseBooking = {
            ...baseBooking,
            customer: { ...baseBooking.customer, ...derivedCustomer },
          };
          setBooking((prev) => ({
            ...prev,
            customer: { ...prev.customer, ...derivedCustomer },
          }));
        }

        const hydrated = await hydrateBookingFromHints({
          hints: { ...derivedHints, ...nextHints },
          pending: { ...derivedPending, ...nextPending },
          baseBooking,
        });
        try {
          await confirmFromSnapshot(hydrated);
          push("assistant", "Confirmed. Your booking has been created.");
        } catch {
          // Ask for the next missing piece in a conversational way.
          push("assistant", await promptForNext(hydrated, { ...derivedPending, ...nextPending }));
        }
        return;
      }

      // Availability questions should be answered by real availability API, not the LLM.
      if (isAvailabilityInquiry(text)) {
        const hydrated = await hydrateBookingFromHints({
          hints: { ...derivedHints, ...nextHints },
          pending: { ...derivedPending, ...nextPending },
          baseBooking,
        });

        const date = (nextPending?.date ?? hydrated?.date ?? null);
        const wantsAlternatives = /\b(other|another|else)\b/i.test(text);
        const time = wantsAlternatives ? null : (nextPending?.time ?? null);

        if (!hydrated?.barber?.id || !hydrated?.service?.durationMinutes) {
          push("assistant", await promptForNext(hydrated, { ...derivedPending, ...nextPending }));
          return;
        }

        if (!date) {
          push("assistant", "Which date should I check? (example: tomorrow or 2026-03-13)");
          return;
        }

        const availability = await fetchAvailableSlots({
          barberId: hydrated.barber.id,
          serviceDurationMinutes: hydrated.service.durationMinutes,
          date,
        });
        const slots = Array.isArray(availability) ? availability : (availability?.slots ?? []);
        const availableSlots = slots.filter((s) => s?.isAvailable && typeof s.start === "string");

        if (!availableSlots.length) {
          push("assistant", `No available slots on ${date}. Try another date.`);
          return;
        }

        if (time) {
          const match = availableSlots.find((s) => s.start.startsWith(date) && s.start.split("T")[1]?.slice(0, 5) === time);
          if (match) {
            push("assistant", `${date} at ${time} is available. If you want to book it, type "confirm".`);
          } else {
            const first = availableSlots[0];
            const nextTime = first?.start?.split("T")[1]?.slice(0, 5) ?? "";
            push(
              "assistant",
              `${date} at ${time} is not available. Next available is ${nextTime}. Say "tomorrow at ${nextTime}" or ask for "other available slots".`
            );
          }
          return;
        }

        const times = availableSlots.slice(0, 8).map((s) => s.start.split("T")[1]?.slice(0, 5)).filter(Boolean);
        push("assistant", `Available times on ${date}:\n- ${times.join("\n- ")}\nTell me a time (example: 10:00).`);
        return;
      }

      if (isAffirmative(text)) {
        const hydrated = await hydrateBookingFromHints({
          hints: { ...derivedHints, ...nextHints },
          pending: { ...derivedPending, ...nextPending },
          baseBooking,
        });
        push("assistant", await promptForNext(hydrated, { ...derivedPending, ...nextPending }));
        return;
      }

      // If the user input was clearly actionable (barber/service/time/customer info),
      // prefer deterministic flow over an LLM answer.
      if (email || name || phone || barberNameHint || serviceHint || parsed) {
        const hydrated = await hydrateBookingFromHints({
          hints: { ...derivedHints, ...nextHints },
          pending: { ...derivedPending, ...nextPending },
          baseBooking,
        });
        push("assistant", await promptForNext(hydrated, { ...derivedPending, ...nextPending }));
        return;
      }

      const ctx = [
        `Context: ${"Main Screen"}`,
        `Now: ${nowIso()}`,
        `Selected barber: ${booking?.barber?.name ?? "none"} (${booking?.barber?.id ?? "none"})`,
        `Selected service: ${booking?.service?.name ?? "none"} (${booking?.service?.id ?? "none"})`,
        `Service duration minutes: ${booking?.service?.durationMinutes ?? "unknown"}`,
        `Selected date: ${booking?.date ?? "none"}`,
        `Selected slot: ${booking?.slot?.start ?? "none"} - ${booking?.slot?.end ?? "none"}`,
      ].join("\n");

      const res = await reserveAi({
        transcript: text,
        context: ctx,
        barberId: booking?.barber?.id ?? null,
        serviceDurationMinutes: booking?.service?.durationMinutes ?? null,
        previousResponseId,
      });

      const spoken = res?.spokenResponse ? String(res.spokenResponse) : "OK";
      const scheduledTime = res?.scheduledTime ? String(res.scheduledTime) : null;
      const responseId = res?.responseId ? String(res.responseId) : null;
      if (responseId) setPreviousResponseId(responseId);

      const hydratedAfterAi = await hydrateBookingFromHints({
        hints: { ...derivedHints, ...nextHints },
        pending: { ...derivedPending, ...nextPending },
        baseBooking,
      });
      const followUp = await promptForNext(hydratedAfterAi, { ...derivedPending, ...nextPending });
      const assistantText = followUp ? `${spoken}\n\n${followUp}` : spoken;

      push("assistant", assistantText);

      if (scheduledTime) {
        await maybeApplyScheduledTime(scheduledTime);
        // Also store it as pending (so later changes can re-resolve).
        if (typeof scheduledTime === "string" && scheduledTime.length >= 16) {
          const dt = {
            date: scheduledTime.slice(0, 10),
            time: scheduledTime.split("T")[1]?.slice(0, 5) ?? null,
          };
          if (dt.date && dt.time) setPending(dt);
        }
      }

      // If we already have a pending request and enough selections, resolve it even if AI didn't return scheduledTime.
      // (We already hydrated above for next-step computation, but keep this for clarity/side effects.)
      await hydrateBookingFromHints({
        hints: { ...derivedHints, ...nextHints },
        pending: { ...derivedPending, ...nextPending },
        baseBooking,
      });
    } catch (e) {
      push("assistant", `Error: ${e?.message ?? String(e)}`);
    } finally {
      setIsSending(false);
    }
  }

  return (
    <div className="mt-8 border rounded-xl bg-white">
      <div className="px-4 py-3 border-b flex items-center justify-between">
        <div>
          <div className="font-semibold">AI Assistant</div>
          <div className="text-xs text-gray-500">Ask for a time, then include name and phone. Type "confirm" to book.</div>
        </div>
        <button
          className="text-xs px-2 py-1 border rounded hover:bg-gray-50"
          onClick={clearAssistantState}
          type="button"
          disabled={isSending}
        >
          New chat
        </button>
      </div>

      <div
        ref={listRef}
        className="px-4 py-3"
        style={{ maxHeight: 320, overflow: "auto", display: "flex", flexDirection: "column", gap: 10 }}
      >
        {messages.length === 0 ? (
          <div className="text-sm text-gray-500">
            Try: "tomorrow at 10" then select barber/service. Share customer info like: "I am Hatice Evren, phone: +90555 123 4567". Then type "confirm".
          </div>
        ) : (
          messages.map(renderMessage)
        )}
      </div>

      <div className="px-4 py-3 border-t flex gap-2">
        <input
          className="flex-1 border rounded-lg px-3 py-2 text-sm"
          value={input}
          placeholder={isSending ? "Sending..." : "Type a message..."}
          onChange={(e) => setInput(e.target.value)}
          onKeyDown={(e) => {
            if (e.key === "Enter") send();
          }}
          disabled={isSending}
        />
        <button
          className="bg-black text-white px-4 py-2 rounded-lg text-sm disabled:opacity-50"
          onClick={send}
          disabled={isSending || !input.trim()}
          type="button"
        >
          Send
        </button>
      </div>
    </div>
  );
}
