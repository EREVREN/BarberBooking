import { useState } from "react";
import { useBooking } from "../BookingProvider";




export default function CreateCustomer ({ disabled, selected, onNext, onBack }) {
    const { booking, setBooking } = useBooking();
    const [customer, setCustomer] = useState(selected || {});

  return (

    <div>
      <h2 className="font-semibold mb-2">Your Info</h2>

      <input
        placeholder="First Name"
        value={customer.firstname}
        onChange={e =>
          setCustomer({ ...customer, firstname: e.target.value })
        }
        className="border p-2 block w-full mb-2"
      />
<input
        placeholder="Last Name"
        value={customer.lastname}
        onChange={e =>
          setCustomer({ ...customer, lastname: e.target.value })
        }
        className="border p-2 block w-full mb-2"
      />
      <input
        placeholder="Phone"
        value={customer.phoneNumber}
        onChange={e =>
          setCustomer({ ...customer, phoneNumber: e.target.value })
        }
        className="border p-2 block w-full mb-2"
      />

      <input
        placeholder="email"
        value={customer.email}
        onChange={e =>
          setCustomer({ ...customer, email: e.target.value })
        }
        className="border p-2 block w-full mb-2"
      />
      <input
        placeholder="Address"
        value={customer.address}
        onChange={e =>
          setCustomer({ ...customer, address: e.target.value })
        }
        className="border p-2 block w-full mb-2"
      />
      <button
        className="w-full bg-black text-white py-3 rounded-lg mt-4 hover:bg-gray-800 transition disabled:opacity-50 disabled:cursor-not-allowed"
        onClick= {() => {
          setBooking(prev => ({ ...prev, customer }));
          onNext();
        }}
        disabled={disabled}
      >
       Next
      </button>
      <button
        onClick={onBack}
        disabled={disabled} 
        className="w-full border border-gray-300 rounded-lg p-4 mt-4 bg-white hover:border-black hover:bg-gray-50 transition"
      >
        Back
      </button>
    </div>
  );
}