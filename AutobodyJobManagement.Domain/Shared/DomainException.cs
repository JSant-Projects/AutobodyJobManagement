using System;
using System.Collections.Generic;
using System.Text;

namespace AutobodyJobManagement.Domain.Shared;

public sealed class DomainException : Exception
{
    public DomainException(string message)
        : base(message)
    {
    }
}
