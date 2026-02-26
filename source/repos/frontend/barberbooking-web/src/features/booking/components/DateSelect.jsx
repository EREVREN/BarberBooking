export function DateSelect({ date, setDate }) {
  return (
    <div>
      <h2 className="font-semibold mb-2">Select Date</h2>
      <input
        type="date"
        value={date || ""}
        onChange={e => setDate(e.target.value)}
        className="border p-2"
      />
    </div>
  );
}