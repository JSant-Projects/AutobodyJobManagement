namespace AutobodyJobManagement.Domain.JobOrder;

public sealed class EstimateRevision
{
    public int RevisionNumber { get; }
    public Estimate Estimate { get; }
    public DateTime ChangedAtUtc { get; }
    public string? Reason { get; }

    internal EstimateRevision(
        int revisionNumber,
        Estimate estimate,
        DateTime changedAtUtc,
        string? reason)
    {
        RevisionNumber = revisionNumber;
        Estimate = estimate;
        ChangedAtUtc = changedAtUtc;
        Reason = reason;
    }
}
