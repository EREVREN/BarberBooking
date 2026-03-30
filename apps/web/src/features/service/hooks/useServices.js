import { useEffect, useState } from "react";
import { fetchBarberServices } from "@/shared/api/services";

export function useServices(barberId) {
  const [data, setData] = useState([]);
  const [isLoading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (!barberId) return;

    setLoading(true);

    fetchBarberServices(barberId)
      .then(setData)
      .catch(setError)
      .finally(() => setLoading(false));
  }, [barberId]);

  return { data, isLoading, error };
}