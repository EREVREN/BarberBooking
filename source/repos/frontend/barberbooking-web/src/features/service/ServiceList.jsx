import { useServices } from "./hooks/useServices";

export function ServiceList({ barberId, onSelect }) {
  const { data, isLoading, error } = useServices(barberId);

  if (!barberId) return <p>Select a barber first</p>;
  if (isLoading) return <p>Loading services...</p>;
  if (error) return <p>Failed to load services</p>;

  return (
    <div>
      <h2>Select Service</h2>
      <ul>
        {data.map(service => (
          <li key={service.id}>
            <button onClick={() => onSelect(service)}>
              {service.name} – {service.durationMinutes} min – ₺{service.price}
            </button>
          </li>
        ))}
      </ul>
    </div>
  );
}