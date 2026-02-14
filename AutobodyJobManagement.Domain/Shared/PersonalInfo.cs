using System;
using System.Collections.Generic;
using System.Text;

namespace AutobodyJobManagement.Domain.Shared;

public record PersonalInfo
{
    public string FirstName { get; }
    public string LastName { get; }
    public string PhoneNumber { get; }
    public Email Email { get; }
    public string FullName => $"{FirstName} {LastName}";
    private PersonalInfo(string firstName, string lastName, string phoneNumber, Email email)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        Email = email;
    }

    public static PersonalInfo Create(string firstName, string lastName, string phoneNumber, Email email)
    {
        Ensure.NotNullOrWhiteSpace(firstName, "First name cannot be null or empty");
        Ensure.NotNullOrWhiteSpace(lastName, "Last name cannot be null or empty");
        Ensure.NotNullOrWhiteSpace(phoneNumber, "Phone number cannot be null or empty");
        Ensure.NotNull(email, "Email cannot be null");
        return new PersonalInfo(firstName, lastName, phoneNumber, email);
    }

}
