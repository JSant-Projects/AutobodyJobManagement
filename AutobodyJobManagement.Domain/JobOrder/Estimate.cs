using AutobodyJobManagement.Domain.Shared;
using System.Text;

namespace AutobodyJobManagement.Domain.JobOrder;

public class Estimate
{
    public EstimateId EstimateId { get; }
    private readonly List<LaborLine> _laborLines = new();
    private readonly List<PartLine> _partLines = new();

    public IReadOnlyList<LaborLine> LaborLines => _laborLines;
    public IReadOnlyList<PartLine> PartLines => _partLines;

    public string Currency { get; }

    public Money LaborCost =>
        _laborLines
        .Select(x => x.LineTotal)
        .Aggregate(Money.Zero(Currency), (acc, next) => acc.Add(next));


    public Money PartsCost =>
        _partLines
        .Select(x => x.LineTotal)
        .Aggregate(Money.Zero(Currency), (acc, next) => acc.Add(next));
    public Money TotalCost => LaborCost.Add(PartsCost);

    private Estimate(string currency)
    {
        EstimateId = new EstimateId(Guid.NewGuid());
        Currency = currency;
    }

    internal static Estimate Create(string currency = "CAD")
    {
        Ensure.CharactersExactLength(currency, 3, "Currency code must be a 3-character ISO code.");
        return new Estimate(currency);
    }

    internal void AddLaborLine(string description, decimal laborHours, decimal hourlyRateAmount)
    {
        var hours = LaborHours.Create(laborHours);
        var hourlyRate = Money.Create(Currency, hourlyRateAmount);

        var laborLine = LaborLine.Create(description, hours, hourlyRate);
        _laborLines.Add(laborLine);
    }

    internal void AddPartLine(string partNumber, string description, int quantity, decimal unitPriceAmount)
    {
        var unitPrice = Money.Create(Currency, unitPriceAmount);
        var partLine = PartLine.Create(partNumber, description, quantity, unitPrice);
        _partLines.Add(partLine);
    }

}
