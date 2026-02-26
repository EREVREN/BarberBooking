
import { useState } from "react";
import { BookingContext } from "./BookingContext";
import { useContext } from "react";

 
 const initialState = {
    barber: null,
    service: null,
    date: null,
    slot: null,
    customer: {
      firstname: "",
      lastname: "",
      phoneNumber: "",
      email: "",
      address: ""
    }
  };

  export default function BookingProvider({ children }) {
  
  
  const [booking, setBooking] = useState(initialState);


  return (
    <BookingContext.Provider value={{ booking, setBooking }}>
      {children}
    </BookingContext.Provider>
  );
}

  export function useBooking() {
  const ctx = useContext(BookingContext);  
    if (!ctx) {
    throw new Error("useBooking must be used within a BookingProvider");
    }
    return ctx;
}
