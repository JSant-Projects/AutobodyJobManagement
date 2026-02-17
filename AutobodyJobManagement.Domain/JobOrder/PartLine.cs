using AutobodyJobManagement.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutobodyJobManagement.Domain.JobOrder;

public class PartLine
{
    public string PartNumber { get; }
    public string Description { get; }
    public int Quantity { get; } 
    public Money UnitPrice { get; }
    public Money LineTotal => UnitPrice.Multiply(Quantity);

    private PartLine()
    {

    }

    private PartLine(string partNumber, string description, int quantity, Money unitPrice)
    {
        PartNumber = partNumber;
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public static PartLine Create(string partNumber, string description, int quantity, Money unitPrice)
    {
        Ensure.NotNullOrWhiteSpace(partNumber, "Part number cannot be null or empty");
        Ensure.NotNullOrWhiteSpace(description, "Description cannot be null or empty");
        Ensure.NonNegativeInteger(quantity, "Quantity must be greater than zero");
        Ensure.NotNull(unitPrice, "Unit price cannot be null");

        return new PartLine(partNumber, description, quantity, unitPrice);
    }
}
