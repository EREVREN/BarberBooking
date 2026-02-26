export async function createCustomer(customerData) {
  const res = await fetch("/api/customers", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(customerData),
  });
  if (!res.ok) {
    throw new Error(`Failed to create customer (${res.status})`);
  }
  return res.json();
}