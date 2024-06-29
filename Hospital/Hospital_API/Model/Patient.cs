using System;
using System.Collections.Generic;

namespace Hospital_API.Model;

public partial class Patient
{
    public int PatientId { get; set; }

    public string? PatientFirstName { get; set; }

    public string? PatientLastName { get; set; }

    public string? PatientAddress { get; set; }

    public string? PatientPhoneNumber { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
