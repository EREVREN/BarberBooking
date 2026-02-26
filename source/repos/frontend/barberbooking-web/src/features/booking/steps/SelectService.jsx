import { useEffect, useState } from "react";
import { fetchBarberServices } from "@/shared/api/services";
import { useBooking } from "../BookingProvider";


export default function SelectService({onNext, onBack}) {
  
  const { booking, setBooking } = useBooking();
 const [services, setServices] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (!booking.barber?.id) return;

    fetchBarberServices(booking.barber.id)
      .then(data => {
        setServices(data);
      })
      .catch(() => {
        setError("Failed to load services");
      })
      .finally(() => {
        setLoading(false);
        });
  }, [booking.barber?.id]);
 
  
 if (loading) {
    return <p className="text-gray-500">Loading services…</p>;
  }

  if (error) {
    return <p className="text-red-600">{error}</p>;
  }

  if (services.length === 0) {
    return (
      <p className="text-gray-500">
        No services available. Please add services from admin panel.
      </p>
    );
  }
  
  return (
    <div className="max-w-3xl mx-auto px-4">
      <h2 className="text-2xl font-semibold mb-4">Select Service</h2>

      <ul>
        {services.map(s => (
          <li key={s.id}>
            <button
            className="w-full bg-black text-white py-3 rounded-lg mt-4 hover:bg-gray-800 transition disabled:opacity-50 disabled:cursor-not-allowed"
              onClick={() => {
                setBooking(prev => ({
                  ...prev,
                  service: {
                    id: s.id,
                    name: s.name,
                    durationMinutes: s.durationMinutes,
                    price: s.price
                  },
                  slot: null
                }));
                 onNext();
  
              }}
              
            >
              <p className="font-medium">{s.name} – {s.durationMinutes} min</p>
            </button>
          </li>
        ))}
      </ul>

      <button className="w-full border border-gray-300 rounded-lg p-4 mt-4 bg-white hover:border-black hover:bg-gray-50 transition" onClick={onBack}>Back</button>
    </div>
  );
}

