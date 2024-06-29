using System;
using System.Collections.Generic;

namespace Hospital_API.Model;

public partial class Staff
{
    public int StaffId { get; set; }

    public int? DepartmentId { get; set; }

    public string? StaffFirstName { get; set; }

    public string? StaffLastName { get; set; }

    public string? StaffAddress { get; set; }

    public string? StaffPhoneNumber { get; set; }

    public virtual Department? Department { get; set; }
}
