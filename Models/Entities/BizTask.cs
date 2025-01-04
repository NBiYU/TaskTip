using System;
using System.Collections.Generic;

namespace TaskTip.Models.Entities;

public partial class BizTask
{
    public string Guid { get; set; } = null!;

    public int? IsCompleted { get; set; }

    public DateTime? TaskTimePlan { get; set; }

    public DateTime? CompletedDateTime { get; set; }

    public string? EditTextTitle { get; set; }

    public string? EditTextText { get; set; }
}
