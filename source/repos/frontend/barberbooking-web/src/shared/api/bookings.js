export async function confirmBooking(payload) {
    const res = await fetch(
        "http://localhost:5273/api/appointments/confirm",
        {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(payload)
        }
    );

    if (!res.ok) {
        const body = await res.json().catch(() => null);

        const err = new Error(body?.error || "Failed to confirm booking");
    
        err.status = res.status;
        throw err;
    }
    return res.json();
}
