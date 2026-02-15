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
    public RepairPlan? RepairPlan { get; private set; }

    public Estimate? Estimate { get; private set; }

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

    public void CreateEstimate(Money laborCost, Money partsCost)
    {
        Estimate = Estimate.Create(laborCost, partsCost);
        JobStatus = JobStatus.Estimated;
    }

    public void ApproveEstimate()
    {
        if (Estimate is null)
        {
            throw new DomainException("Cannot approve job without estimate");
        }

        if (JobStatus != JobStatus.Estimated)
        {
            throw new DomainException("Job must be estimated state to approve");
        }

        JobStatus = JobStatus.Approved;
    }

    public void CreateRepairPlan(string notes, TimeSpan estimatedDuration)
    {
        RepairPlan = RepairPlan.Create(notes, estimatedDuration);
    }

    public void AddRequiredPart(string partNumber, string description, int quantity, string supplier)
    {
        if (RepairPlan is null)
        {
            throw new DomainException("Job must have repair plan to add required parts");
        }

        var date = DateTime.UtcNow;
        RepairPlan.AddRequiredParts(partNumber, description, quantity, supplier, date);
        JobStatus = JobStatus.WaitingForParts;
    }

    public void StartRepair()
    {
        if (RepairPlan is null)
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
