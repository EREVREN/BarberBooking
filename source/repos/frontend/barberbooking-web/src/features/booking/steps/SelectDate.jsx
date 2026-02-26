import { useBooking } from "../BookingProvider";


export function SelectDate({ onNext, onBack }) {
  const { booking, setBooking } = useBooking();

  return (
   <div className="max-w-3xl mx-auto px-4">
      <h2 className="text-2xl font-semibold mb-4" >Select Date</h2>

      <input 
        className="w-full border border-gray-300 rounded-lg p-4 mt-4 bg-white hover:border-black hover:bg-gray-50 transition"
        type="date"
        value={booking.date || ""}
        onChange={e =>
          setBooking(prev => ({
            ...prev,
            date: e.target.value,
            slot: null
          }))
        }
      />

      <div style={{ marginTop: 16 }}>
        
        <button
          className="w-full bg-black text-white py-3 rounded-lg mt-4 hover:bg-gray-800 transition disabled:opacity-50 disabled:cursor-not-allowed"
          onClick={onNext}
          disabled={!booking.date}
        >
          Next
        </button>
        <button 
        className="w-full border border-gray-300 rounded-lg p-4 mt-4 bg-white hover:border-black hover:bg-gray-50 transition"
          disabled={!booking.date}
        onClick={onBack}>Back</button>
      </div>
    </div>
  );
}
