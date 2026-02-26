//const API_BASE = import.meta.env.VITE_API_BASE_URL;

export async function fetchBarberServices(barberId) {
  const res = await fetch(
    `/api/barbers/${barberId}/services`
  );

  if (!res.ok) {
    throw new Error("Failed to load services");
  }

  return res.json();
}