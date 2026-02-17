using AutobodyJobManagement.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutobodyJobManagement.Domain.JobOrder;

public class LaborLine
{
    public string Description { get; }
    public LaborHours LaborHours { get; private set; }

    public Money HourlyRate { get; private set; }
    public Money LineTotal => HourlyRate.Multiply(LaborHours.Value);

    private LaborLine()
    {
        
    }

    private LaborLine(string description, LaborHours laborHours, Money hourlyRate)
    {
        Description = description;
        LaborHours = laborHours;
        HourlyRate = hourlyRate;
    }

    internal static LaborLine Create(string description, LaborHours laborHours, Money hourlyRate)
    {
        Ensure.NotNullOrWhiteSpace(description, "Description cannot be null or empty");
        Ensure.NotNull(laborHours, "LaborHours cannot be null");
        Ensure.NotNull(hourlyRate, "Rate cannot be null");

        return new LaborLine(description, laborHours, hourlyRate);
    }

    internal void AddHours(LaborHours additionalHours)
    {
        Ensure.NotNull(additionalHours);
        LaborHours = LaborHours.Add(additionalHours); // implement Add on the VO
    }

    internal void UpdateHourlyRate(Money newRate)
    {
        Ensure.NotNull(newRate);
        HourlyRate = newRate;
    }
}
