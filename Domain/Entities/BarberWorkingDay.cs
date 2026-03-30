
using BarberBooking.Domain.Common;
using BarberBooking.Domain.ValueObjects;


namespace BarberBooking.Domain.Entities;

    public class BarberWorkingDay : BaseEntity
{
    public Guid BarberId { get; private set; }

    // Weekly schedule
    public DayOfWeek DayOfWeek { get; private set; }

    // Optional: specific date override (NULL = weekly)
    public DateTime? Date { get; private set; }
    // public DateTime StartTime { get; private set; }
    //  public DateTime EndTime { get; private set; }

    public WorkingHours WorkingHours { get; private set; }

    protected BarberWorkingDay() { }

    public TimeRange ToTimeRange(DateTime date)
    {
        return new TimeRange(
            date.Date.Add(WorkingHours.Start),
            date.Date.Add(WorkingHours.End)
        );
    }

    public BarberWorkingDay(Guid barberId, DayOfWeek dayOfWeek, DateTime date, WorkingHours workingHours)
    {                           //DateTime start, DateTime end
        BarberId = barberId;
        DayOfWeek = dayOfWeek;
        Date = date;
        WorkingHours = workingHours;
        //StartTime = start;
        // EndTime = end;
    }

    public static BarberWorkingDay Create(
       Guid barberId,
       DayOfWeek dayOfWeek,
       DateTime date,
       WorkingHours workingHours)
    {

        return new BarberWorkingDay(
        barberId,
        dayOfWeek,
        date,
        workingHours
        );
        }
    } 



