namespace AutobodyJobManagement.Domain.JobOrder;

public enum JobStatus
{
    Draft = 0,
    Estimated = 1,
    Approved = 2,
    WaitingForParts = 3,
    InRepair = 4,
    Painting = 5,
    Detailing = 6,
    ReadyForPickup = 7,
    Delivered = 8
}
