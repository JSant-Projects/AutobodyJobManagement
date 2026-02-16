using AutobodyJobManagement.Domain.Customer;
using DomainJobOrder = AutobodyJobManagement.Domain.JobOrder.JobOrder;
using AutobodyJobManagement.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using AwesomeAssertions;
using AutobodyJobManagement.Domain.JobOrder;

namespace AutobodyJobManagement.Domain.Tests.JobOrder;

public class JobOrderSpecifications
{
    [Fact]
    public void Create_Should_Return_JobOrder_With_Draft_Status()
    {
        var vehicleId = new VehicleId(Guid.NewGuid());

        var result = DomainJobOrder.Create(vehicleId);
        
        result.Should().NotBeNull();
        result.JobStatus.Should().Be(JobStatus.Draft);
        result.VehicleId.Should().Be(vehicleId);
    }

    [Fact]
    public void Create_Should_Throw_ArgumentException_When_VehicleId_Is_Null()
    {
        Action act = () => DomainJobOrder.Create(null!);
        act.Should().ThrowExactly<ArgumentException>().WithMessage("VehicleId can't be null*");
    }

    [Fact]
    public void Create_Should_Throw_ArgumentException_When_VehicleId_Is_Empty()
    {
        Action act = () => DomainJobOrder.Create(new VehicleId(Guid.Empty));
        act.Should().ThrowExactly<ArgumentException>().WithMessage("VehicleId can't be empty*");
    }

    [Theory]
    [InlineData("Repair plan notes", 5)]
    [InlineData("Repair plan notes 1", 16)]
    [InlineData("Repair plan notes 2", 24)]
    public void CreateRepairPlan_Should_Return_RepairPlan_When_EstimatedDuration_Is_Valid(string notes, int hours)
    {
        var vehicleId = new VehicleId(Guid.NewGuid());
        var jobOrder = DomainJobOrder.Create(vehicleId);
        var estimatedDuration = TimeSpan.FromHours(hours);
        jobOrder.CreateRepairPlan(notes, estimatedDuration);
        jobOrder.RepairPlan.Should().NotBeNull();
        jobOrder.RepairPlan.Notes.Should().Be(notes);
        jobOrder.RepairPlan.EstimatedDuration.Should().Be(estimatedDuration);
    }



}
