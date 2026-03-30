import { useEffect, useState } from "react";

export function useBarbers() {
    const [data, setData] = useState([]);   // ← MUST be array
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        // TEMP MOCK until controller exists
        setTimeout(() => {
            setData([
                { id: "1", name: "Barber One" },
                { id: "2", name: "Barber Two" }
            ]);
            setIsLoading(false);
        }, 500);
    }, []);

    return { data, isLoading, error };
}