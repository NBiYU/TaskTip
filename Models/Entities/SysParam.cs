using System;
using System.Collections.Generic;

namespace TaskTip.Models.Entities;

public partial class SysParam
{
    public string Key { get; set; } = null!;

    public string? Value { get; set; }

    public string? LastUpdate { get; set; }

    public string? Description { get; set; }
}
