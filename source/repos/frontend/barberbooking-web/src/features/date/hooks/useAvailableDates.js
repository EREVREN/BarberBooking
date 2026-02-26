import { useEffect, useState } from "react";
import { fetchAvailableDates } from "@/shared/api/slots";

export function useAvailableDates(barberId, serviceId) {
    const [data, setData] = useState([]);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        if (!barberId || !serviceId) return;

        setLoading(true);

        fetchAvailableDates(barberId, serviceId)
            .then(setData)
            .finally(() => setLoading(false));
    }, [barberId, serviceId]);

    return { data, loading };
}