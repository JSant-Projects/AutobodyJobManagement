using AutobodyJobManagement.Domain.JobOrder;
using AwesomeAssertions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutobodyJobManagement.Domain.Tests.JobOrder;

public class LaborHoursSpecification
{
    [Theory]
    [InlineData(5)]
    [InlineData(5.5)]
    public void Create_Should_Return_LaborHours_When_Value_Is_Valid(decimal hours)
    {
        var result = LaborHours.Create(hours);
        result.Should().BeOfType<LaborHours>();
        result.Should().NotBeNull();
        result.Value.Should().Be(hours);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-0.01)]
    public void Create_Should_Throw_ArgumentException_When_Value_Is_Negative(decimal value)
    {
        Action act = () => LaborHours.Create(value);
        act.Should().ThrowExactly<ArgumentOutOfRangeException>().WithMessage("Labor hours cannot be negative*");
    }
}
