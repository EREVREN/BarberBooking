export function SlotGrid({ slots, selected, onSelect }) {
    if (!slots.length) {
        return <p>No available slots for this date</p>;
    }

    return (
        <div style={{ display: "grid", gridTemplateColumns: "repeat(4, 1fr)", gap: 12 }}>
            {slots.map((slot) => {
                const isSelected = selected?.id === slot.id;

                return (
                    <button
                        key={slot.id}
                        onClick={() => onSelect(slot)}
                        disabled={!slot.isAvailable}
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
                        {slot.startTime} – {slot.endTime}s
                    </button>
                );
            })}
        </div>
    );
}
