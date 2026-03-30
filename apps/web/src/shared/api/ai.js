export async function reserveAi({
  transcript,
  context,
  barberId,
  serviceDurationMinutes,
  previousResponseId,
}) {
  const res = await fetch("/api/ai/reserve", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
      transcript,
      context,
      barberId,
      serviceDurationMinutes,
      previousResponseId,
    }),
  });

  if (!res.ok) {
    const text = await res.text().catch(() => "");
    throw new Error(text || `AI request failed (${res.status})`);
  }

  return res.json();
}
