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
    public static IEnumerable<object[]> CreateEstimateTestData()
    {
        // Scenario 1 (your original)
        yield return new object[]
        {
            new List<(string, decimal, decimal)>
            {
                ("Body", 5.5m, 75.5m),
                ("Paint", 3m, 75.5m)
            },
            new List<(string, string, int, decimal)>
            {
                ("ABCDE123", "LF Door Moulding", 1, 25.50m)
            },
            641.75m,
            25.50m,
            667.25m
        };

        // Scenario 2 – Single labor, multiple parts
        yield return new object[]
        {
            new List<(string, decimal, decimal)>
            {
                ("Frame Repair", 2m, 100m) // 200
            },
            new List<(string, string, int, decimal)>
            {
                ("XYZ123", "Front Bumper", 2, 50m),   // 100
                ("ABC789", "Headlight", 1, 120m)     // 120
            },
            200m,
            220m,
            420m
        };

        // Scenario 3 – Multiple labor, multiple parts, mixed rates
        yield return new object[]
        {
            new List<(string, decimal, decimal)>
            {
                ("Body Work", 4m, 80m),   // 320
                ("Paint", 2.5m, 90m),     // 225
                ("Polish", 1m, 60m)       // 60
            },
            new List<(string, string, int, decimal)>
            {
                ("PART1", "Door Handle", 2, 30m),   // 60
                ("PART2", "Mirror", 1, 75m)         // 75
            },
            605m,
            135m,
            740m
        };

        // Scenario 4 – Multiple labor, No, mixed rates
        yield return new object[]
        {
            new List<(string, decimal, decimal)>
            {
                ("Body Work", 4m, 80m),   // 320
                ("Paint", 2.5m, 90m),     // 225
                ("Polish", 1m, 60m)       // 60
            },
            null!,
            605m,
            0,
            605m
        };
    }


    [Fact]
    public void Create_Should_Return_JobOrder_With_Draft_Status()
    {
        var vehicleId = new VehicleId(Guid.NewGuid());

        var result = DomainJobOrder.Create(vehicleId);
        
        result.Should().NotBeNull();
        result.JobStatus.Should().Be(JobStatus.Draft);
        result.VehicleId.Should().Be(vehicleId);
    }

    [Theory]
    [MemberData(nameof(CreateEstimateTestData))]
    public void CreateEstimate_Should_Create_CurrrentEstimate_When_LaborLines_And_PartLine_Are_Valid(
        IReadOnlyList<(string description, decimal laborHours, decimal hourlyRate)> laborLines,
        IReadOnlyList<(string partNumber, string description, int quantity, decimal unitPrice)> partLines,
        decimal expectedLaborCost,
        decimal expectedPartsCost,
        decimal expectedTotalCost) 
    {

        var vehicleId = new VehicleId(Guid.NewGuid());

        var jobOrder = DomainJobOrder.Create(vehicleId);

        jobOrder.CreateEstimate(laborLines, partLines, "CAD");

        jobOrder.CurrentEstimate.Should().NotBeNull();
        jobOrder.CurrentEstimate.LaborCost.Amount.Should().Be(expectedLaborCost);
        jobOrder.CurrentEstimate.PartsCost.Amount.Should().Be(expectedPartsCost);
        jobOrder.CurrentEstimate.TotalCost.Amount.Should().Be(expectedTotalCost);
    }

    [Fact]
    public void CreateEstimate_Should_Throw_DomainException_When_LaborLines_And_PartLines_Are_Null()
    {
        var vehicleId = new VehicleId(Guid.NewGuid());

        var jobOrder = DomainJobOrder.Create(vehicleId);

        Action act = () => jobOrder.CreateEstimate(null, null, "CAD");

        act.Should().ThrowExactly<DomainException>().WithMessage("Estimate must contain either labor or part lines");
    }

    [Fact] 
    public void CreateEstimate_Should_Throw_DomainException_When_Estimate_Already_Exist()
    { 
        IReadOnlyList<(string description, decimal laborHours, decimal hourlyRate)> laborLines = 
            [
                ("Body", 5.5m, 75.5m), 
                ("Paint", 3, 75.5m),
            ]; 
        IReadOnlyList<(string partNumber, string description, int quantity, decimal unitPrice)> partLines = 
            [
                ("ABCDE123", "LF Door Moulding", 1, 25.50m)
            ]; var vehicleId = new VehicleId(Guid.NewGuid());
        
        var jobOrder = DomainJobOrder.Create(vehicleId); 
        jobOrder.CreateEstimate(laborLines, partLines, "CAD");

        Action act = () => jobOrder.CreateEstimate(laborLines, partLines, "CAD");

        act.Should().ThrowExactly<DomainException>().WithMessage("Estimate already exists. Use ReviseEstimate");

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



}
