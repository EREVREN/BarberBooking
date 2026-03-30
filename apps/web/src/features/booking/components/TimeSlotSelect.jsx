import { useAvailableSlots } from "@/shared/api/slots";

export function TimeSlotSelect({ barber, service, date, slot, setSlot }) {
  const { data } = useAvailableSlots(
    barber.id,
    service.id,
    date
  );

  return (
    <div>
      <h2 className="font-semibold mb-2">Available Times</h2>
      <div className="grid grid-cols-3 gap-3">
        {data.map(s => (
          <button
            key={s.start}
            onClick={() => setSlot(s)}
            className={`p-2 border ${
              slot?.start === s.start ? "border-black" : ""
            }`}
          >
            {s.start} - {s.end}
          </button>
        ))}
      </div>
    </div>
  );
}