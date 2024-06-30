global using Hospital_API.Model;
using Hospital_API.AutoMapper;
using Hospital_API.Services.Appointments;
using Hospital_API.Services.Doctors;
using Hospital_API.Services.Patients;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<HospitalDbContext>();

builder.Services.AddAutoMapper(typeof(AppointmentProfile), typeof(DepartmentProfile), typeof(DoctorProfile), typeof(HospitalProfile), typeof(PatientProfile), typeof(StaffProfile));


// Register data services
builder.Services.AddTransient<IAppointmentService, AppointmentService>();
builder.Services.AddTransient<IDoctorService, DoctorService>();
builder.Services.AddTransient<IPatientService, PatientService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
