import { useState } from 'react';
import { createBarber } from '@/shared/api/barbers';

export function BarberCreate() {
    const [name, setName] = useState('');

    async function submit(e) {
        e.preventDefault();
        await createBarber(name);
        setName('');
    }

    return (
        <form onSubmit={submit}>
            <h2>Create Barber</h2>
            <input
                value={name}
                onChange={e => setName(e.target.value)}
                placeholder="Barber name"
            />
            <button type="submit">Save</button>
        </form>
    );
}
