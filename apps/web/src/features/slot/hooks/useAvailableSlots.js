import { useEffect, useState } from "react";
import { fetchAvailableSlots } from "@/shared/api/slots";

export function useAvailableSlots(barberId, serviceId, date) {
    const [data, setData] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    useEffect(() => {
        if (!barberId || !serviceId || !date) return;

        setLoading(true);

        fetchAvailableSlots(barberId, serviceId, date)
            .then(setData)
            .catch(setError)
            .finally(() => setLoading(false));
    }, [barberId, serviceId, date]);

    return { data, loading, error };
}