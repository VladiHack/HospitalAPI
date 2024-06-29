using System;
using System.Collections.Generic;

namespace Hospital_API.Model;

public partial class Doctor
{
    public int DoctorId { get; set; }

    public string? DoctorFirstName { get; set; }

    public string? DoctorLastName { get; set; }

    public int? DepartmentId { get; set; }

    public string? DoctorPhoneNumber { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Department? Department { get; set; }
}
