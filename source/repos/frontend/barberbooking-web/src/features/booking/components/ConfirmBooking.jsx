import { useBooking } from "../BookingProvider";
import { useState } from "react";
import { confirmBooking } from "@/shared/api/bookings";
import { useToast } from "@/shared/toast/useToast";
import { createCustomer } from "@/shared/api/customers";

export function ConfirmBooking({ onNext, onBack, onGoToSlots }) {
  const { booking, setBooking } = useBooking();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const { showToast } = useToast();

  function extractCustomerId(createdCustomer) {
    if (!createdCustomer) return null;
    if (typeof createdCustomer === "string" || typeof createdCustomer === "number") {
      return createdCustomer;
    }
    return createdCustomer.id ?? createdCustomer.customerId ?? null;
  }

  async function handleConfirm() {
    if (loading) return;

    setLoading(true);
    setError(null);

    try {
      let customerId = booking.customer?.id;

      if (!customerId) {
        const createdCustomer = await createCustomer(booking.customer);
        customerId = extractCustomerId(createdCustomer);

        if (!customerId) {
          throw new Error("Customer created but no customerId was returned.");
        }

        setBooking(prev => ({
          ...prev,
          customer: { ...prev.customer, id: customerId },
        }));
      }

      await confirmBooking({
        barberId: booking.barber?.id,
        serviceId: booking.service?.id,
        startTime: booking.slot?.start,
        endTime: booking.slot?.end,
        customerId
      });

      showToast("Appointment confirmed", "success");
      onNext?.();
    } catch (e) {
      if (e.status === 409) {
        showToast("This slot was just booked. Please select another time.", "warning");
        setBooking(prev => ({ ...prev, slot: null }));
        onGoToSlots?.();
      } else {
        showToast(e.message || "Unexpected error", "error");
        setError(e.message);
      }
    } finally {
      setLoading(false);
    }
  }

  return (
    <div style={{ marginTop: 24 }}>
      <h2 className="text-2xl font-semibold mb-4 mt-6">Confirm Booking</h2>
      <p>Review your booking details before confirming:</p>
      <ul className="list-disc pl-5 mt-2">
        <li>Barber: {booking.barber?.name}</li>
        <li>Service: {booking.service?.name}</li>
        <li>Date: {booking.date}</li>
        <li>Time: {booking.slot?.start ? `${booking.slot.start.split("T")[1].substring(0, 5)} - ${booking.slot.end.split("T")[1].substring(0, 5)}` : "Not selected"}</li>
      </ul>
      <button onClick={handleConfirm} disabled={loading} className="
                    w-full bg-black text-white py-3 rounded-lg mt-4 hover:bg-gray-800 transition
                    disabled:opacity-50 disabled:cursor-not-allowed
                  ">
        {loading ? "Confirming..." : "Confirm"}
      </button>
      <button
        className="w-full border border-gray-300 rounded-lg p-4 mt-4 bg-white hover:border-black hover:bg-gray-50 transition"
        onClick={onBack}
        disabled={loading}
      >
        Back
      </button>
      {error && <p style={{ color: "red" }}>{error}</p>}
    </div>
  );
}
