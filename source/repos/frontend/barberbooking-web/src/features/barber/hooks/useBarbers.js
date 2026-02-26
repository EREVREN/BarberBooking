import { useEffect, useState } from 'react';
import { fetchBarbers } from '@/shared/api/barbers';

export function useBarbers() {
    const [data, setData] = useState([]);
    const [isLoading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        fetchBarbers()
            .then(setData)
            .catch(setError)
            .finally(() => setLoading(false));
    }, []);

    return { data, isLoading, error };
}