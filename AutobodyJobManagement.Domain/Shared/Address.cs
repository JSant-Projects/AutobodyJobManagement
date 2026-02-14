using System;
using System.Collections.Generic;
using System.Text;

namespace AutobodyJobManagement.Domain.Shared;

public record Address(string Street, string City, string Province, string PostalCode);
