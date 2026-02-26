export function DatePicker({ dates, selected, onSelect }) {
    if (!dates?.length) {
        return <p>No available dates</p>;
    }

    return (
        <div>
            <h2>Select a date</h2>

            <ul>
                {dates.map((date) => (
                    <li key={date}>
                        <button
                            onClick={() => onSelect(date)}
                            style={{
                                fontWeight: selected === date ? "bold" : "normal",
                            }}
                        >
                            {new Date(date).toDateString()}
                        </button>
                    </li>
                ))}
            </ul>
        </div>
    );
}