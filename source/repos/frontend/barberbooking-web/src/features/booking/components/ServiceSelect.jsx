import { useServices } from "@/shared/api/services";

export function ServiceSelect({ barber, service, setService }) {
  const { data } = useServices(barber.id);

  return (
    <div>
      <h2 className="font-semibold mb-2">Choose Service</h2>
      {data.map(s => (
        <button
          key={s.id}
          onClick={() => setService(s)}
          className={`block w-full text-left p-3 border mb-2 ${
            service?.id === s.id ? "border-black" : ""
          }`}
        >
          {s.name} — {s.durationMinutes} min — ₺{s.price}
        </button>
      ))}
    </div>
  );
}