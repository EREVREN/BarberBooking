export function DayTimeline({ bookings }) {
  return (
    <div className="day-timeline">
      {bookings.map(a => (
        <div
          key={a.id}
          className="booking-block"
          style={{
            top: timeToPixels(a.startTime),
            height: durationToPixels(a.startTime, a.endTime)
          }}
        >
          <strong>{a.barberName}</strong>
          <div>{formatTime(a.startTime)} – {formatTime(a.endTime)}</div>
          <div>{a.customerName}</div>
        </div>
      ))}
    </div>
  );
}