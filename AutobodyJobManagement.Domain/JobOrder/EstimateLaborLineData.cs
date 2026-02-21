using System;
using System.Collections.Generic;
using System.Text;

namespace AutobodyJobManagement.Domain.JobOrder;

public sealed record EstimateLaborLineData(string Description, decimal Hours, decimal HourlyRate);
