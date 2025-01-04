using System;
using System.Collections.Generic;

namespace TaskTip.Models.Entities;

public partial class BizRecordMenu
{
    public string Guid { get; set; } = null!;

    public string? Name { get; set; }

    public int? IsDirectory { get; set; }

    public string? ParentGuid { get; set; }
}
