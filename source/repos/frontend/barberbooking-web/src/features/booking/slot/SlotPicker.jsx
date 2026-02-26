import { useEffect, useState } from "react";
import { fetchAvailableSlots } from "@/shared/api/slots";

export function SlotPicker({
  barberId,
  serviceId,
  date,
  selected,
  onSelect,
  disabled
}) {
  const [slots, setSlots] = useState([]);

  useEffect(() => {
    if (!barberId || !serviceId || !date) return;

    fetchAvailableSlots(barberId, serviceId, date)
      .then(setSlots);
  }, [barberId, serviceId, date]);

  if (!slots.length) {
    return <p>No available slots for this date</p>;
  }

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