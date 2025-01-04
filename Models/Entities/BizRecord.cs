using System;
using System.Collections.Generic;

namespace TaskTip.Models.Entities;

public partial class BizRecord
{
    public string Guid { get; set; } = null!;

    public string? Title { get; set; }

    public string? Text { get; set; }
}
