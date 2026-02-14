using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace AutobodyJobManagement.Domain.JobOrder;

public class RepairPlan
{
    private List<RequiredPart> _requiredParts = new();
    public string? Notes { get; }
    public TimeSpan EstimatedDuration { get; }
    
    public IReadOnlyList<RequiredPart> RequiredParts => _requiredParts.ToList();

    private RepairPlan() { }
    private RepairPlan(string notes, TimeSpan estimatedDuration)
    {
        Notes = notes;
        EstimatedDuration = estimatedDuration;
    }

    internal static RepairPlan Create(string notes, TimeSpan estimatedDuration)
    {
        return new RepairPlan(notes, estimatedDuration);
    }

    internal void AddRequiredParts(string partNumber, string description, int quantity, string supplier, DateTime dateOrdered)
    {
        var requiredPart = RequiredPart.Create(partNumber, description, quantity, supplier, dateOrdered);
        _requiredParts.Add(requiredPart);
    }
}
