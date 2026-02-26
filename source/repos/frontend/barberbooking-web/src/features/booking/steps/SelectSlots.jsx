
import { useEffect, useState } from "react";
import fetchAvailableSlots from "@/shared/api/availability";
import { useBooking } from "../BookingProvider";

export function SelectSlots({ onNext, onBack, 
  selected,
  disabled}) {

  const { booking, setBooking } = useBooking();
  const [slots, setSlots] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
 
  
  useEffect(() => {
    if (!booking.barber?.id || !booking.service?.id || !booking.date) return;

    setLoading(true);
    setError(null);
    fetchAvailableSlots({
      barberId: booking.barber.id,
      date: booking.date,
      serviceDurationMinutes: booking.service.durationMinutes
    })
      .then(res => setSlots(Array.isArray(res) ? res : (res?.slots ?? [])))
      .catch(e => setError(e.message))
      .finally(() => setLoading(false));
     
  }, [booking.barber?.id, booking.service?.id, booking.date]);

if (loading) {
    return <p className="text-gray-500">Loading available slots…</p>;
  }
if (error) {
    return <p className="text-red-600">{error}</p>;
  }

 if (!slots.length) {
    return (
      <div>
        <p>No available slots for this date</p>
        <button onClick={onBack}>Back</button>
      </div>
    );
  } 

    const getStyle = (slot, isSelected) => ({
    ...baseStyle,
    border: isSelected ? "2px solid black" : "1px solid #ccc",
    background: !slot.isAvailable
      ? "#eee"
      : isSelected
      ? "#000"
      : "#fff",
    color: isSelected ? "#fff" : "#000",
    cursor: slot.isAvailable ? "pointer" : "not-allowed",
  }); 
   const baseStyle = {
            padding: 10,
            borderRadius: 8
       };        
   
return (
  
    <div>
      
     <h2 className="text-2xl font-semibold mb-4">Select Time Slot</h2>
     
      <div style={{ display: "grid", gridTemplateColumns: "repeat(4, 1fr)", gap: 12 }}>
      
      {slots.map(slot => {
      const isSelected = selected?.start === slot.start;
      const startTime = `${slot.start.split("T")[1].substring(0, 5)}`;
      const endTime = `${slot.end.split("T")[1].substring(0, 5)}`;
      return (

        <div >
        
           <button  
            key={slot.start}
            disabled={disabled || !slot.isAvailable}
            onClick={() => {
              setBooking(prev => ({ ...prev, slot }));
              onNext();
            }}
            style={getStyle(slot, isSelected)}
            >
              {startTime} - {endTime}           
           </button>
         </div>
    
        );
        }
      )}
      </div>
       <button className="w-full bg-black text-white py-3 rounded-lg mt-4 hover:bg-gray-800 transition disabled:opacity-50 disabled:cursor-not-allowed"
       onClick={onBack}>Back</button>
      
     </div> 
     
  );
  
 
}








/*

import { useEffect, useState } from "react";
import fetchAvailableSlots from "@/shared/api/availability";
import { useBooking } from "../BookingProvider";


export function SelectSlot({
  barberId,
  serviceId,
  date,
  selected,
  onSelect,
  disabled
}) {
  const [slots, setSlots] = useState([]);
  const { booking, setBooking } = useBooking();

  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!barberId || !serviceId || !date) return;

    fetchAvailableSlots(barberId, serviceId, date)
      .then(setSlots);
  }, [barberId, serviceId, date]);

  

  return (
    <div style={{ display: "grid", gridTemplateColumns: "repeat(4, 1fr)", gap: 12 }}>
      {slots.map(slot => {
        const isSelected = selected?.startTime === slot.startTime;

        return (
          <button
            key={slot.startTime}
            disabled={disabled || !slot.isAvailable}
            onClick={() => onSelect(slot)}
            style={{
              padding: 10,
              borderRadius: 8,
              border: isSelected ? "2px solid black" : "1px solid #ccc",
              background: !slot.isAvailable
                ? "#eee"
                : isSelected
                ? "#000"
                : "#fff",
              color: isSelected ? "#fff" : "#000",
              cursor: slot.isAvailable ? "pointer" : "not-allowed",
            }}
          >
            {slot.startTime} – {slot.endTime}
          </button>
        );
      })}
    </div>
  );
}
*/
