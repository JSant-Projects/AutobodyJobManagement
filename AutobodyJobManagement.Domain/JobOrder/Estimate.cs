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

    internal static Estimate Create(IReadOnlyList<LaborLine> laborLines, IReadOnlyList<PartLine> partLines, string currency = "CAD")
    {
        if (laborLines.Count == 0 && partLines.Count == 0)
        {
            throw new DomainException("Estimate must contain either labor or part lines");
        }

        var laborCopy = laborLines.ToList().AsReadOnly();
        var partCopy = partLines.ToList().AsReadOnly();

        return new Estimate(laborCopy, partCopy, currency);
    }

}
