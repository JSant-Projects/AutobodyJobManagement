using AutobodyJobManagement.Domain.Shared;
using System.Runtime.Versioning;
using System.Text;

namespace AutobodyJobManagement.Domain.JobOrder;

public class Estimate
{
    public IReadOnlyList<LaborLine> LaborLines { get; }
    public IReadOnlyList<PartLine> PartLines { get; }

    public string Currency { get; }

    public Money LaborCost =>
        LaborLines
        .Select(x => x.LineTotal)
        .Aggregate(Money.Zero(Currency), (acc, next) => acc.Add(next));


    public Money PartsCost =>
        PartLines
        .Select(x => x.LineTotal)
        .Aggregate(Money.Zero(Currency), (acc, next) => acc.Add(next));
    public Money TotalCost => LaborCost.Add(PartsCost);

    private Estimate(IReadOnlyList<LaborLine> laborLines, IReadOnlyList<PartLine> partLines, string currency)
    {
        LaborLines = laborLines;
        PartLines = partLines;
        Currency = currency;
    }

    internal static Estimate Create(
        IReadOnlyList<EstimateLaborLineData> estimateLaborLines, 
        IReadOnlyList<EstimatePartLineData> estimatePartLines, 
        string currency = "CAD")
    {
        if (estimateLaborLines.Count == 0 && estimatePartLines.Count == 0)
        {
            throw new DomainException("Estimate must contain either labor or part lines");
        }

        var estimateLaborLinesCopy = estimateLaborLines.ToList().AsReadOnly();
        var estimatePartLinesCopy = estimatePartLines.ToList().AsReadOnly();

        var laborLines = new List<LaborLine>(estimateLaborLinesCopy.Count);
        var partLines = new List<PartLine>(estimatePartLinesCopy.Count);

        // Build laborlines
        foreach (var line in estimateLaborLinesCopy)
        {
            var laborHours = LaborHours.Create(line.Hours);
            var hourlyRate = Money.Create(currency, line.HourlyRate);
            laborLines.Add(LaborLine.Create(line.Description, laborHours, hourlyRate));
        }

        // Build partlines
        foreach (var line in estimatePartLines)
        {
            var unitPrice = Money.Create(currency, line.UnitPrice);
            partLines.Add(PartLine.Create(line.PartNumber, line.Description, line.Quantity, unitPrice));

        }


        return new Estimate(laborLines, partLines, currency);
    }

}
