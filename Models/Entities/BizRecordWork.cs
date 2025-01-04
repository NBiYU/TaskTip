using System;
using System.Collections.Generic;

namespace TaskTip.Models.Entities;

public partial class BizRecordWork
{
    public string Guid { get; set; } = null!;

    public string? RecordDate { get; set; } = string.Empty;

    public string? WorkTime { get; set; } = string.Empty;

    public string? StartTime { get; set; } = string.Empty;

    public string? EndTime { get; set; } = string.Empty;
}
