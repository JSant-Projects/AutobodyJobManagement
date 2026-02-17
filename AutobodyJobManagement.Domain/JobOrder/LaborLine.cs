using AutobodyJobManagement.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutobodyJobManagement.Domain.JobOrder;

public class LaborLine
{
    public string Description { get; }
    public LaborHours LaborHours { get; }

    public Money Rate { get; }
    public Money LineTotal => Rate.Multiply(LaborHours.Value);

    private LaborLine()
    {
        
    }

    private LaborLine(string description, LaborHours laborHours, Money rate)
    {
        Description = description;
        LaborHours = laborHours;
        Rate = rate;
    }

    internal static LaborLine Create(string description, LaborHours laborHours, Money rate)
    {
        Ensure.NotNullOrWhiteSpace(description, "Description cannot be null or empty");
        Ensure.NotNull(laborHours, "LaborHours cannot be null");
        Ensure.NotNull(rate, "Rate cannot be null");

        return new LaborLine(description, laborHours, rate);
    }
}
