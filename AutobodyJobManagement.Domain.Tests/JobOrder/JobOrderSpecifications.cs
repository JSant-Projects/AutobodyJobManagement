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
            new List<EstimateLaborLineData>
            {
                new EstimateLaborLineData("Body", 5.5m, 75.5m),
                new EstimateLaborLineData("Paint", 3m, 75.5m)
            },
            new List<EstimatePartLineData>
             {
                new EstimatePartLineData("ABCDE123", "LF Door Moulding", 1, 25.50m)
             },
            641.75m,
            25.50m,
            667.25m
        };

        // Scenario 2 – Single labor, multiple parts
        yield return new object[]
        {
            new List<EstimateLaborLineData>
            {
                new EstimateLaborLineData("Frame Repair", 2m, 100m) // 200
            },
            new List<EstimatePartLineData>
            {
                new EstimatePartLineData("XYZ123", "Front Bumper", 2, 50m), // 100
                new EstimatePartLineData("ABC789", "Headlight", 1, 120m) // 120
            },
            200m,
            220m,
            420m
        };

        // Scenario 3 – Multiple labor, multiple parts, mixed rates
        yield return new object[]
        {
            new List<EstimateLaborLineData>
            {
                new EstimateLaborLineData("Body Work", 4m, 80m), // 320
                new EstimateLaborLineData("Paint", 2.5m, 90m),  // 225
                new EstimateLaborLineData("Polish", 1m, 60m) // 60
            },

            new List<EstimatePartLineData>
            {
                new EstimatePartLineData("PART1", "Door Handle", 2, 30m),   // 60
                new EstimatePartLineData("PART2", "Mirror", 1, 75m)         // 75
            },
            605m,
            135m,
            740m
        };

        // Scenario 4 – Multiple labor, No, mixed rates
        yield return new object[]
        {
            new List<EstimateLaborLineData>
            {
                new EstimateLaborLineData("Body Work", 4m, 80m), // 320
                new EstimateLaborLineData("Paint", 2.5m, 90m),  // 225
                new EstimateLaborLineData("Polish", 1m, 60m) // 60
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
        IReadOnlyList<EstimateLaborLineData> laborLines,
        IReadOnlyList<EstimatePartLineData> partLines,
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
    public void UpdateEstimate_Should_Update_CurrrentEstimate_When_LaborLines_And_PartLine_Are_Valid()
    {
        var vehicleId = new VehicleId(Guid.NewGuid());
        var jobOrder = DomainJobOrder.Create(vehicleId);
        IReadOnlyList<EstimateLaborLineData> laborLines =
            [
                new EstimateLaborLineData("Body", 5.5m, 75.5m),
                new EstimateLaborLineData("Paint", 3, 75.5m),
            ];
        IReadOnlyList<EstimatePartLineData> partLines =
            [
                new EstimatePartLineData("ABCDE123", "LF Door Moulding", 1, 25.50m)
            ];
        jobOrder.CreateEstimate(laborLines, partLines, "CAD");
        // Update with new lines
        IReadOnlyList<EstimateLaborLineData> updatedLaborLines =
            [
                new EstimateLaborLineData("Body", 6m, 80m), // Updated hours and rate
                new EstimateLaborLineData("Paint", 4m, 85m) // Updated hours and rate
            ];
        IReadOnlyList<EstimatePartLineData> updatedPartLines =
            [
                new EstimatePartLineData("ABCDE123", "LF Door Moulding", 2, 25.50m) // Updated quantity
            ];
        jobOrder.UpdateEstimate(updatedLaborLines, updatedPartLines, "CAD");
        jobOrder.CurrentEstimate.Should().NotBeNull();
        jobOrder.CurrentEstimate.LaborCost.Amount.Should().Be(6 * 80 + 4 * 85); // 480 + 340 = 820
        jobOrder.CurrentEstimate.PartsCost.Amount.Should().Be(2 * 25.50m); // 51
        jobOrder.CurrentEstimate.TotalCost.Amount.Should().Be(820 + 51); // 871
    }

    [Fact]
    public void UpdateEstimate_Should_Throw_DomainException_When_Estimate_Does_Not_Exist()
    {
        var vehicleId = new VehicleId(Guid.NewGuid());
        var jobOrder = DomainJobOrder.Create(vehicleId);
        Action act = () => jobOrder.UpdateEstimate(null, null, "CAD");
        act.Should().ThrowExactly<DomainException>().WithMessage("No existing estimate to update. Use CreateEstimate");
    }

    [Fact]
    public void UpdateEstimate_Should_Add_EstimateRevision_To_EstimateRevisions()
    {
        var vehicleId = new VehicleId(Guid.NewGuid());
        var jobOrder = DomainJobOrder.Create(vehicleId);
        IReadOnlyList<EstimateLaborLineData> laborLines =
            [
                new EstimateLaborLineData("Body", 5.5m, 75.5m),
                new EstimateLaborLineData("Paint", 3, 75.5m),
            ];
        IReadOnlyList<EstimatePartLineData> partLines =
            [
                new EstimatePartLineData("ABCDE123", "LF Door Moulding", 1, 25.50m)
            ];
        jobOrder.CreateEstimate(laborLines, partLines, "CAD");
        // Update with new lines
        IReadOnlyList<EstimateLaborLineData> updatedLaborLines =
            [
                new EstimateLaborLineData("Body", 6m, 80m), // Updated hours and rate
                new EstimateLaborLineData("Paint", 4m, 85m) // Updated hours and rate
            ];
        IReadOnlyList<EstimatePartLineData> updatedPartLines =
            [
                new EstimatePartLineData("ABCDE123", "LF Door Moulding", 2, 25.50m) // Updated quantity
            ];
        jobOrder.UpdateEstimate(updatedLaborLines, updatedPartLines, "CAD");
        jobOrder.EstimateRevisions.Should().HaveCount(1);
        var revision = jobOrder.EstimateRevisions[0];
        revision.RevisionNumber.Should().Be(1);
        revision.Estimate.LaborCost.Amount.Should().Be(5.5m * 75.5m + 3 * 75.5m); // Original labor cost
        revision.Estimate.PartsCost.Amount.Should().Be(1 * 25.50m); // Original parts cost
        revision.Estimate.TotalCost.Amount.Should().Be(5.5m * 75.5m + 3 * 75.5m + 1 * 25.50m); // Original total cost
        revision.Reason.Should().BeNull();
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
        IReadOnlyList<EstimateLaborLineData> laborLines = 
            [
                new EstimateLaborLineData("Body", 5.5m, 75.5m), 
                new EstimateLaborLineData("Paint", 3, 75.5m),
            ];
        IReadOnlyList<EstimatePartLineData> partLines = 
            [
                new EstimatePartLineData("ABCDE123", "LF Door Moulding", 1, 25.50m)
            ]; 
        
        var vehicleId = new VehicleId(Guid.NewGuid());
        
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
