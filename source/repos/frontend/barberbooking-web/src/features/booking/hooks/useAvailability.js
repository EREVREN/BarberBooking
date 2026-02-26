import { useEffect, useState } from "react";
import { fetchAvailability } from "@/shared/api/availability";

export function useAvailability(barberId, date, duration) {
  const [slots, setSlots] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (!barberId || !date || !duration) {
      setSlots([]);
      return;
    }

    let cancelled = false;

    async function load() {
      setLoading(true);
      setError(null);

      try {
        const data = await fetchAvailability({
          barberId,
          date,
          duration
        });

        if (!cancelled) {
          setSlots(data.slots || []);
        }
      } catch (err) {
        if (!cancelled) {
          setError(err.message);
        }
      } finally {
        if (!cancelled) {
          setLoading(false);
        }
      }
    }

    load();
    return () => { cancelled = true; };
  }, [barberId, date, duration]);

  return { slots, loading, error };
}