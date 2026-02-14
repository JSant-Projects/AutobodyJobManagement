using AutobodyJobManagement.Domain.Shared;

namespace AutobodyJobManagement.Domain.JobOrder;

public class Estimate
{
    public Money LaborCost { get; }
    public Money PartsCost { get; }
    public Money TotalCost { get; }

    public Estimate(Money laborCost, Money partsCost)
    {
        LaborCost = laborCost;
        PartsCost = partsCost;
        TotalCost = LaborCost.Add(PartsCost);
    }

    internal static Estimate Create(Money laborCost, Money partsCost)
    {
        Ensure.NotNull(laborCost);
        Ensure.NotNull(partsCost);

        return new Estimate(laborCost, partsCost);
        
    } 
}