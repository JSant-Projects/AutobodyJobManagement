using AutobodyJobManagement.Domain.Shared;

namespace AutobodyJobManagement.Domain.JobOrder;

public sealed record LaborHours
{
    public decimal Value { get; }

    private LaborHours(decimal value)
    {
        Value = decimal.Round(value, 2);
    }

    public static LaborHours Create(decimal value)
    {
        Ensure.PositiveDecimal(value, "Labor hours cannot be negative");
        return new LaborHours(value);
    }
}