
export async function fetchAvailableDates(barberId, serviceId) {
  const res = await fetch(
    `/api/barbers/${barberId}/services/${serviceId}/dates`
  );

  if (!res.ok) throw new Error("Failed to load dates");

  return res.json();
}
