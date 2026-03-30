import { useEffect, useState } from "react";
import { useBooking } from "./BookingProvider";
import { SelectBarber } from "./steps/SelectBarber";
import SelectService from "./steps/SelectService";
import { SelectDate } from "./steps/SelectDate";
import { SelectSlots } from "./steps/SelectSlots";
import { BookingSummary } from "./components/BookingSummary";
import CreateCustomer from "./customer/CreateCustomer";
import { ConfirmBooking } from "./components/ConfirmBooking";

const emptyBooking = {
  barber: null,
  service: null,
  date: null,
  slot: null,
  customer: {
    firstname: "",
    lastname: "",
    phoneNumber: "",
    email: "",
    address: "",
  },
};

export default function BookingStepper({ resetSignal }) {
  const [step, setStep] = useState(1);
  const [loading] = useState(false);
  const [error] = useState(null);

  const next = () => setStep(s => s + 1);
  const back = () => setStep(s => s - 1);

  const { booking, setBooking } = useBooking();

  useEffect(() => {
    setStep(1);
    setBooking(emptyBooking);
  }, [resetSignal, setBooking]);

  return (
    <div className="booking-page">
      {step === 1 && <SelectBarber disabled={loading} onNext={next} />}

      {step === 2 && <SelectService disabled={loading} onNext={next} onBack={back} />}

      {step === 3 && <SelectDate disabled={loading} onNext={next} onBack={back} />}

      {step === 4 && (
        <SelectSlots disabled={loading} selected={booking.slot} onNext={next} onBack={back} />
      )}

      {step === 5 && (
        <CreateCustomer
          disabled={loading}
          selected={booking.customer}
          onBack={back}
          onNext={next}
        />
      )}

      {step === 6 && <BookingSummary loading={loading} error={error} onBack={back} onNext={next} />}

      {step === 7 && <ConfirmBooking onNext={next} onBack={back} onGoToSlots={() => setStep(4)} />}

      {step === 8 && <h2>Booking Confirmed</h2>}
    </div>
  );
}
