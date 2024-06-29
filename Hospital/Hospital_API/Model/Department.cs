using System;
using System.Collections.Generic;

namespace Hospital_API.Model;

public partial class Department
{
    public int DepartmentId { get; set; }

    public int? HospitalId { get; set; }

    public string? DepartmentName { get; set; }

    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();

    public virtual Hospital? Hospital { get; set; }

    public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();
}
