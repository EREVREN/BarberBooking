export function CustomerForm({ customer, setCustomer }) {
  return (
    <div>
      <h2 className="font-semibold mb-2">Your Info</h2>

      <input
        placeholder="Name"
        value={customer.name}
        onChange={e =>
          setCustomer({ ...customer, name: e.target.value })
        }
        className="border p-2 block w-full mb-2"
      />

      <input
        placeholder="Phone"
        value={customer.phone}
        onChange={e =>
          setCustomer({ ...customer, phone: e.target.value })
        }
        className="border p-2 block w-full"
      />
    </div>
  );
}