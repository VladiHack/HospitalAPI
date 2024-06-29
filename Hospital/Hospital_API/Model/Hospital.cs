using Hospital_API.Dto;
using System;
using System.Collections.Generic;

namespace Hospital_API.Model;

public partial class Hospital
{
    public int HospitalId { get; set; }

    public string? HospitalName { get; set; }

    public string? HospitalAddress { get; set; }

    public string? HospitalPhoneNumber { get; set; }

    public string? State { get; set; }

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();
}
