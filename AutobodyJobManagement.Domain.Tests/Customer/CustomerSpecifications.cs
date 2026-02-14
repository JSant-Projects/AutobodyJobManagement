using AutobodyJobManagement.Domain.Shared;
using DomainCustomer = AutobodyJobManagement.Domain.Customer.Customer;
using System;
using System.Collections.Generic;
using System.Text;
using AwesomeAssertions;

namespace AutobodyJobManagement.Domain.Tests.Customer;

public class CustomerSpecifications
{
    [Theory]
    [InlineData("jayson@sample.com", "123 Sesame St", "JollyTown", "CA", "12345", "Jayson", "Santiago", "587-333-4444")]
    [InlineData("belle@sample.com", "456 Sesame St", "JollyTown", "CA", "12345", "Belle", "Santiago", "587-333-4444")]
    [InlineData("cathy@sample.com", "456 Sesame St", "JollyTown", "CA", "12345", "Cathy", "Santiago", "587-333-4444")]
    public void Create_Should_Return_Customer_When_PersonalInfo_Email_And_Address_Are_Valid(
        string inputEmail, 
        string inputStreet,
        string inputCity,
        string inputProvince,
        string inputPostaCode,
        string inputFirstName,
        string inputLastName,
        string inputPhone)
    {
        var email = Email.Create(inputEmail);
        var address = new Address(inputStreet, inputCity, inputProvince, inputPostaCode);
        var personalInfoWithEmail = PersonalInfo.Create(inputFirstName, inputLastName, inputPhone, email);

        var result = DomainCustomer.Create(personalInfoWithEmail, address);
        var expectedFullName = $"{inputFirstName} {inputLastName}";
        result.PersonalInfo.FullName.Should().NotBeNull().And.Be(expectedFullName);
        result.Address.Should().NotBeNull().And.BeEquivalentTo(address);
    }

    [Fact]
    public void Create_Should_Throw_ArgumentException_When_PersonalInfo_Is_Null()
    {
        var address = new Address("123 Sesame St", "JollyTown", "CA", "12345");

        Action act = () => DomainCustomer.Create(null, address);
        act.Should().ThrowExactly<ArgumentException>().WithMessage("Personal info can't be null*");
    }

    [Fact]
    public void Create_Should_Throw_ArgumentException_When_Address_Is_Null()
    {

        var email = Email.Create("jayson@sample.com");
        var personalInfoWithEmail = PersonalInfo.Create("Jayson", "Santiago", "587-333-4444", email);
        Action act = () => DomainCustomer.Create(personalInfoWithEmail, null);
        act.Should().ThrowExactly<ArgumentException>().WithMessage("Address can't be null*");
    }
}
