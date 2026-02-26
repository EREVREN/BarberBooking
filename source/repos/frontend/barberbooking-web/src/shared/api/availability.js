export default async function fetchAvailableSlots({
  barberId,
  serviceDurationMinutes,
  date,
}) {
    const params = new URLSearchParams({
        barberId: String(barberId),
        date: String(date),
        serviceDurationMinutes: String(serviceDurationMinutes ?? 30),
    });

    const res = await fetch(
        `http://localhost:5273/api/availability?${params.toString()}`
    );

    if (!res.ok) {
        throw new Error("Failed to load availability");
    }

    return res.json();
}


