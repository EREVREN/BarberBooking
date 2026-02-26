export function WeekTimeline({ bookings }) {
  const days = groupByDay(bookings);

  return (
    <div className="week-grid">
      {Object.entries(days).map(([day, items]) => (
        <div key={day} className="week-day-column">
          <h4>{day}</h4>
          <DayTimeline bookings={items} />
        </div>
      ))}
    </div>
  );
}