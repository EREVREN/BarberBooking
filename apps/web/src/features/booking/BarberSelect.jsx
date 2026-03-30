import { useBarbers } from "@/features/barber/hooks/useBarbers";

export function BarberSelect({ barber, setBarber }) {
  const { data, isLoading } = useBarbers();

  if (isLoading) return <p>Loading barbers...</p>;

  return (
    <div>
      <h2 className="font-semibold mb-2">Choose Barber</h2>
      <div className="grid grid-cols-2 gap-4">
        {data.map(b => (
          <button
            key={b.id}
            onClick={() => setBarber(b)}
            className={`p-4 border rounded ${
              barber?.id === b.id ? "border-black" : ""
            }`}
          >
            {b.name}
          </button>
        ))}
      </div>
    </div>
  );
}