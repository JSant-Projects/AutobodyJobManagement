using AutobodyJobManagement.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AutobodyJobManagement.Domain.Customer;

public class Vehicle
{
    private Vehicle() { }
    public VehicleId VehicleId { get; }
    public string VIN { get; }
    public string Make { get; }
    public string Model { get; }
    public string Trim { get; }
    public int Year { get; }

    private Vehicle(string vin, string make, string model, string trim, int year)
    {
        VehicleId = new VehicleId(Guid.NewGuid());
        VIN = vin;
        Make = make;
        Model = model;
        Trim = trim;
        Year = year;
    }


    internal static Vehicle Create(string vin, string make, string model, string trim, int year)
    {
        Ensure.NotNullOrWhiteSpace(vin);
        Ensure.NotNullOrWhiteSpace(make);
        Ensure.NotNullOrWhiteSpace(model);
        int validYearTo = DateTime.UtcNow.Year + 1;
        Ensure.NotOutOfRange(year, 1900, validYearTo);

        return new Vehicle(vin, make, model, trim, year);

    }

}
