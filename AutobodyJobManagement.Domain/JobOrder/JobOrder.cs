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

        var laborLines = new List<LaborLine>(estimateLaborLines.Count);
        var partLines = new List<PartLine>(estimatePartLines.Count);

        // Build laborlines
        foreach (var line in estimateLaborLines) 
        {
            var laborHours = LaborHours.Create(line.Hours);
            var hourlyRate = Money.Create(normalizedCurrency, line.HourlyRate);
            laborLines.Add(LaborLine.Create(line.Description, laborHours, hourlyRate));
        }

        // Build partlines
        foreach (var line in estimatePartLines)
        {
            var unitPrice = Money.Create(normalizedCurrency, line.UnitPrice);
            partLines.Add(PartLine.Create(line.PartNumber, line.Description, line.Quantity, unitPrice));

        }

        CurrentEstimate = Estimate.Create(laborLines, partLines, normalizedCurrency);

        JobStatus = JobStatus.Estimated;
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
