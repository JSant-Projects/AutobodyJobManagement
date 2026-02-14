using AutobodyJobManagement.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutobodyJobManagement.Domain.Customer;

public class Customer
{
    public CustomerId CustomerId { get; }
    public PersonalInfo PersonalInfo { get; }
    public Address Address { get; }

    private List<Vehicle> _vehicles = new();

    public IReadOnlyList<Vehicle> Vehicles => _vehicles.ToList();

    private Customer() { }
    private Customer(PersonalInfo personalInfo, Address address)
    {
        CustomerId = new CustomerId(Guid.NewGuid());
        PersonalInfo = personalInfo;
        Address = address;
    }

    public static Customer Create(PersonalInfo personalInfo, Address address)
    {
        Ensure.NotNull(personalInfo, "Personal info can't be null");
        Ensure.NotNull(address, "Address can't be null");

        return new Customer(personalInfo, address); 
    }

    public void AddVehicle(string vin, string make, string model, string trim, int year)
    {
        var newVehicle = Vehicle.Create(vin, make, model, trim, year);

        _vehicles.Add(newVehicle);
    }
}
