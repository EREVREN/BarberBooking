
import { useState } from "react";

export function useBooking() {
  const [barber, setBarber] = useState(null);
  const [service, setService] = useState(null);
  const [date, setDate] = useState(null);
  const [slot, setSlot] = useState(null);
  const [customer, setCustomer] = useState({
    name: "",
    phone: ""
  });
  const [confirmed, setConfirmed] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  return {
    barber, setBarber,
    service, setService,
    date, setDate,
    slot, setSlot,  
    customer, setCustomer,
    confirmed, setConfirmed,
    loading, setLoading,
    error, setError
  };
}
