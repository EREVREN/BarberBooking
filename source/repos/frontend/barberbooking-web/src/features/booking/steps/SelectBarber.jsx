import { useEffect, useState } from "react";
import { fetchBarbers } from "@/shared/api/barbers";
import { useBooking } from "../BookingProvider";


export function SelectBarber({ onNext }) {
  const { setBooking } = useBooking();
  const [barbers, setBarbers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetchBarbers()
      .then(data => {
        setBarbers(data);
      })
      .catch(() => {
        setError("Failed to load barbers");
      })
      .finally(() => {
        setLoading(false);
      });
  }, []);

  if (loading) {
    return <p className="text-gray-500">Loading barbers…</p>;
  }

  if (error) {
    return <p className="text-red-600">{error}</p>;
  }

  if (barbers.length === 0) {
    return (
      <p className="text-gray-500">
        No barbers available. Please add barbers from admin panel.
      </p>
    );
  }
  
   
  return (
    <div className="max-w-3xl mx-auto px-4">
      <h2 className="text-2xl font-semibold mb-4">Select Barber</h2>

      <ul className="grid grid-cols-1 sm:grid-cols-2 gap-4">
        {barbers.map(b => (
          <li key={b.id}>
            <button
              className="w-full bg-black text-white py-3 rounded-lg mt-4 hover:bg-gray-800 transition disabled:opacity-50 disabled:cursor-not-allowed"
              onClick={() => {
                setBooking(prev => ({
                  ...prev,      
                barber: { id: b.id, name: b.name },
                service: null,
                slot: null
                }));
                onNext();
               
              }}
            >
              <p className="font-medium">{b.name}</p>
            </button>
          </li>
        ))}
      </ul>
    </div>
  );
}
