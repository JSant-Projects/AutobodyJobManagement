using AutobodyJobManagement.Domain.Shared;

namespace AutobodyJobManagement.Domain.JobOrder;

public class RequiredPart
{
    public string PartNumber { get; }
    public string? Description { get; }
    public int Quantity { get; }
    public string Supplier { get; }
    public bool Received { get; private set; }
    public string? Notes { get; }

    public DateTime DateOrdered { get; }
    public DateTime? DateReceived => Received ? DateTime.UtcNow: null;

    private RequiredPart() { }

    private RequiredPart(string partNumber, string description, int quantity, string supplier, DateTime dateOrdered, bool receieved) 
    {
        PartNumber = partNumber;
        Description = description;
        Quantity = quantity;
        Supplier = supplier;
        DateOrdered = dateOrdered;
        Received = receieved;
    }
    internal static RequiredPart Create(string partNumber, string description, int quantity, string supplier, DateTime dateOrdered)
    {
        return new RequiredPart(partNumber, description, quantity, supplier, dateOrdered, false);
    }

    public void MarkAsReceived()
    {
        if (Received)
        {
            throw new DomainException("Part is already marked as received.");
        }
        Received = true;
    }
}