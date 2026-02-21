namespace AutobodyJobManagement.Domain.JobOrder;

public sealed record EstimatePartLineData(string PartNumber, string Description, int Quantity, decimal UnitPrice);
