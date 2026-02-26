
//const API_BASE = import.meta.env.VITE_API_BASE_URL;

export async function fetchBarbers() {
  const res = await fetch("/api/barbers");
  if (!res.ok) {
    throw new Error(`Failed to load barbers (${res.status})`);
  }
  return res.json();
}

export async function createBarber(barberData) {
  const res = await fetch("/api/admin/barbers", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(barberData),
  });
  if (!res.ok) {
    throw new Error(`Failed to create barber (${res.status})`);
  }
  return res.json();
}
