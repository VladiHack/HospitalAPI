using System;
using System.Collections.Generic;

namespace Hospital_API.Model;

public partial class Appointment
{
    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public DateOnly? Date { get; set; }

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;
}
