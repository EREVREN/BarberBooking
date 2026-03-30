export async function createCustomer(customerData) {
  const rawName =
    customerData?.name ??
    customerData?.Name ??
    `${customerData?.firstname ?? ""} ${customerData?.lastname ?? ""}`.trim();

  const rawPhone =
    customerData?.phoneNumber ??
    customerData?.PhoneNumber ??
    customerData?.phone ??
    "";

  const name = String(rawName ?? "").trim();
  const rawPhoneText = String(rawPhone ?? "").trim();
  const normalizedPhone = rawPhoneText.startsWith("+")
    ? `+${rawPhoneText.slice(1).replace(/\D/g, "")}`
    : rawPhoneText.replace(/\D/g, "");
  const phoneDigits = normalizedPhone.startsWith("+") ? normalizedPhone.length - 1 : normalizedPhone.length;
  const phoneNumber = normalizedPhone;

  if (!name) {
    throw new Error("Customer name is required.");
  }
  if (!phoneNumber) {
    throw new Error("Customer phone number is required.");
  }
  // Backend EF constraints (CustomerConfiguration): max length 20.
  if (name.length > 20) {
    throw new Error(`Customer name must be 20 characters or less (got ${name.length}).`);
  }
  if (phoneDigits < 10) {
    throw new Error("Customer phone number must have at least 10 digits.");
  }
  if (phoneNumber.length > 20) {
    throw new Error("Customer phone number is too long.");
  }
  const email = customerData?.email ? String(customerData.email).trim() : null;
  if (email && email.length > 20) {
    throw new Error(`Customer email must be 20 characters or less (got ${email.length}).`);
  }

  const res = await fetch("/api/customers", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      name,
      phoneNumber,
      email,
      address: customerData?.address ?? null,
    }),
  });
  if (!res.ok) {
    const text = await res.text().catch(() => "");
    throw new Error(text || `Failed to create customer (${res.status})`);
  }
  return res.json();
}
