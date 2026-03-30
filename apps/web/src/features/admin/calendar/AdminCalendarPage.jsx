import { useState } from "react";
import { useAdminBookings } from "./hooks/useAdminBookings";
import { DayTimeline } from "./DayTimeline";
import { WeekTimeline } from "./WeekTimeline";

export default function AdminCalendarPage() {
  const [view, setView] = useState("day");
  const [date, setDate] = useState(new Date());

  const from = view === "day"
    ? startOfDay(date)
    : startOfWeek(date);

  const to = view === "day"
    ? endOfDay(date)
    : endOfWeek(date);

  const { data = [] } = useAdminBookings({ from, to });

  return (
    <div>
      <CalendarToolbar view={view} onChangeView={setView} />

      {view === "day" && <DayTimeline bookings={data} />}
      {view === "week" && <WeekTimeline bookings={data} />}
    </div>
  );
}