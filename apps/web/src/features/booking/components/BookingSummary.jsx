import { useBooking } from "../BookingProvider";
import { useState } from "react";

export function BookingSummary({onNext, onBack}) {
  const { booking } = useBooking();
  const {loading} = useState(false);
  const {error} = useState(null);
  
  const isComplete = booking.barber?.id &&
    booking.service?.id &&
    booking.date &&
    booking.slot&&
    booking.customer
    ;
    
    console.log(booking);


  return (
    <div style={{ marginTop: 24 }}>
      <h2 className="text-2xl font-semibold mb-4">Booking Summary</h2>

      <Row label="Barber" value={booking.barber?.name} />
      <Row label="Service" value={booking.service?.name} />
      <Row label="Price" value={booking.service?.price && `$${booking.service.price}`} />
      <Row
        label="Duration"
        value={
          booking.service?.durationMinutes &&
          `${booking.service.durationMinutes} min`
        }
      />
      <Row label="Date" value={booking.date} />
      <Row
        label="Time"
        value={booking.slot && `${booking.slot.start.split("T")[1].substring(0, 5)} - ${booking.slot.end.split("T")[1].substring(0, 5)}`}
      />
      <Row label="Customer" value={`${booking.customer?.firstname || "Not specified"} ${booking.customer?.lastname || ""}`} />

      {error && <p style={{ color: "red" }}>{error}</p>}

      <button
        onClick={() => {
          onNext();
        }}
        disabled={!isComplete || loading}
        className="
                    w-full bg-black text-white py-3 rounded-lg mt-4 hover:bg-gray-800 transition
                    disabled:opacity-50 disabled:cursor-not-allowed
                  "
      >
        {loading ? "Loading..." : "Next"}
      </button>
      <button
        className="w-full border border-gray-300 rounded-lg p-4 mt-4 bg-white hover:border-black hover:bg-gray-50 transition"
        onClick={onBack}
        disabled={loading}
      >
        Back
      </button>
    </div>
  );
}

function Row({ label, value }) {
  return (
    <div style={{ display: "flex", justifyContent: "space-between" }}>
      <strong>{label}</strong>
      <span>{value || "-"}</span>
    </div>
  );
}
