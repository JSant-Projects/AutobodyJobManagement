using AutobodyJobManagement.Domain.Customer;
using AutobodyJobManagement.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace AutobodyJobManagement.Domain.JobOrder;

public class JobOrder
{
    public JobOrderId JobOrderId { get; }
    public VehicleId VehicleId { get; }
    public JobStatus JobStatus { get; private set; }
    public Estimate? CurrentEstimate { get; private set; }
    public readonly List<EstimateRevision> _estimateRevisions = new();
    public IReadOnlyList<EstimateRevision> EstimateRevisions => _estimateRevisions.AsReadOnly();

    private JobOrder() { }
    private JobOrder(VehicleId vehicleId, JobStatus jobStatus)
    {
        JobOrderId = new JobOrderId(Guid.NewGuid());
        VehicleId = vehicleId;
        JobStatus = jobStatus;  
    }

    public static JobOrder Create(VehicleId vehicleId)
    {
        Ensure.NotNull(vehicleId, "VehicleId can't be null");
        Ensure.NotEmptyGuid(vehicleId.Id, "VehicleId can't be empty");

        return new JobOrder(vehicleId, JobStatus.Draft);
    }

    public void CreateEstimate(
        IReadOnlyList<EstimateLaborLineData>? estimateLaborLines,
        IReadOnlyList<EstimatePartLineData>? estimatePartLines,
        string currency
        )
    {
        estimateLaborLines ??= Array.Empty<EstimateLaborLineData>();
        estimatePartLines ??= Array.Empty<EstimatePartLineData>();

        Ensure.NotNullOrWhiteSpace(currency);
        var normalizedCurrency = currency.Trim().ToUpperInvariant();

        if (CurrentEstimate is not null)
        {
            throw new DomainException("Estimate already exists. Use ReviseEstimate");
        }

        CurrentEstimate = Estimate.Create(estimateLaborLines, estimatePartLines, normalizedCurrency);

        JobStatus = JobStatus.Estimated;
    }

    public void UpdateEstimate(
        IReadOnlyList<EstimateLaborLineData>? updatedEstimateLaborLines,
        IReadOnlyList<EstimatePartLineData>? updatedEstimatePartLines,
        string currency,
        string? reason = null
        )
    {
        updatedEstimateLaborLines ??= Array.Empty<EstimateLaborLineData>();
        updatedEstimatePartLines ??= Array.Empty<EstimatePartLineData>();

        Ensure.NotNullOrWhiteSpace(currency);
        var normalizedCurrency = currency.Trim().ToUpperInvariant();

        if (CurrentEstimate is null)
        {
            throw new DomainException("No existing estimate to update. Use CreateEstimate");
        }

        var newEstimate = Estimate.Create(updatedEstimateLaborLines, updatedEstimatePartLines, normalizedCurrency);

        AddEstimateRevision(CurrentEstimate, reason);

        CurrentEstimate = newEstimate;

    }

    private void AddEstimateRevision(Estimate estimate, string? reason)
    {
        var next = _estimateRevisions.Count + 1;
        _estimateRevisions.Add(new EstimateRevision(next, estimate, DateTime.UtcNow, reason));
    }

    public void ApproveEstimate()
    {
        if (CurrentEstimate is null)
        {
            throw new DomainException("Cannot approve job without estimate");
        }

        if (JobStatus != JobStatus.Estimated)
        {
            throw new DomainException("Job must be estimated state to approve");
        }

        JobStatus = JobStatus.Approved;
    }

    public void StartRepair()
    {
        if (CurrentEstimate is null)
        {
            throw new DomainException("Job must have repair plan to start the repair");
        }

        if (JobStatus == JobStatus.WaitingForParts)
        {
            throw new DomainException("All required parts should be received before starting the repair");
        }

        JobStatus = JobStatus.InRepair;
    }
}
