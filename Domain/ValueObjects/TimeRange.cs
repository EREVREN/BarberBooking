namespace BarberBooking.Domain.ValueObjects;

public sealed record TimeRange(DateTime Start, DateTime End)
{
    public bool Overlaps(TimeRange other)
    {
        return Start < other.End && other.Start < End;
    }
}