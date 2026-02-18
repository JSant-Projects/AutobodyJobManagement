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
        IReadOnlyList<(string description, decimal laborHours, decimal hourlyRate)>? estimateLaborLines,
        IReadOnlyList<(string partNumber, string description, int quantity, decimal unitPrice)>? estimatePartLines,
        string currency
        )
    {
        estimateLaborLines ??= Array.Empty<(string, decimal, decimal)>();
        estimatePartLines ??= Array.Empty<(string, string, int, decimal)>();

        Ensure.NotNullOrWhiteSpace(currency);
        currency = currency.Trim().ToUpperInvariant();

        if (CurrentEstimate is not null)
        {
            throw new DomainException("Estimate already exists. Use ReviseEstimate");
        }

        var laborLines = new List<LaborLine>();
        var partLines = new List<PartLine>();

        // Build laborlines
        foreach (var line in estimateLaborLines) 
        {
            var laborHours = LaborHours.Create(line.laborHours);
            var hourlyRate = Money.Create(currency, line.hourlyRate);
            laborLines.Add(LaborLine.Create(line.description, laborHours, hourlyRate));
        }

        // Build partlines
        foreach (var line in estimatePartLines)
        {
            var unitPrice = Money.Create(currency, line.unitPrice);
            partLines.Add(PartLine.Create(line.partNumber, line.description, line.quantity, unitPrice));

        }

        CurrentEstimate = Estimate.Create(laborLines, partLines, currency);

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
