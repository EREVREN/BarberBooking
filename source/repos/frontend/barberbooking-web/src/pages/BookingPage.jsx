import { useState } from "react";
import { BarberSelect } from "@/features/booking/BarberSelect";
import { ServiceSelect } from "@/features/booking/ServiceSelect";
import { DateSelect } from "@/features/booking/DateSelect";
import { SlotGrid } from "@/features/booking/SlotGrid";
import { BookingSummary } from "@/features/booking/BookingSummary";
import { confirmBooking } from "@/shared/api/bookings";

export default function BookingPage() {
    const [step, setStep] = useState(1);
    const [barber, setBarber] = useState(null);
    const [service, setService] = useState(null);
    const [date, setDate] = useState(null);
    const [slot, setSlot] = useState(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    const next = () => setStep((s) => s + 1);
    const back = () => setStep((s) => s - 1);

    async function handleConfirm() {
        setLoading(true);
        setError(null);

        try {
            await confirmBooking({
                barberId: barber.id,
                serviceId: service.id,
                startTime: slot.startTime,
                endTime: slot.endTime
            });

            setStep(6);
        } catch (e) {
            setError(e.message);
        } finally {
            setLoading(false);
        }
    }

    return (
        <div className="booking-page">
            {step === 1 && <BarberSelect onSelect={(b) => { setBarber(b); next(); }} />}

            {step === 2 && (
                <ServiceSelect
                    barberId={barber.id}
                    onSelect={(s) => { setService(s); next(); }}
                    onBack={back}
                />
            )}

            {step === 3 && (
                <DateSelect
                    onSelect={(d) => { setDate(d); next(); }}
                    onBack={back}
                />
            )}

            {step === 4 && (
                <SlotGrid
                    barberId={barber.id}
                    serviceId={service.id}
                    date={date}
                    selected={slot}
                    onSelect={(s) => { setSlot(s); next(); }}
                    onBack={back}
                />
            )}

            {step === 5 && (
                <BookingSummary
                    barber={barber}
                    service={service}
                    date={date}
                    slot={slot}
                    loading={loading}
                    error={error}
                    onBack={back}
                    onConfirm={handleConfirm}
                />
            )}

            {step === 6 && <h2>✅ Booking Confirmed</h2>}
        </div>
    );
}