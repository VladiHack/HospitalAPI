using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Hospital_API.Model;

public partial class HospitalDbContext : DbContext
{
    public HospitalDbContext()
    {
    }

    public HospitalDbContext(DbContextOptions<HospitalDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<Hospital> Hospitals { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQLExpress;Database=HospitalDB;Trusted_Connection=true;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => new { e.PatientId, e.DoctorId });

            entity.ToTable("Appointment");

            entity.Property(e => e.PatientId).HasColumnName("Patient_ID");
            entity.Property(e => e.DoctorId).HasColumnName("Doctor_ID");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Docto__571DF1D5");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Patie__5629CD9C");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("PK__Departme__151675D110E10F3D");

            entity.ToTable("Department");

            entity.Property(e => e.DepartmentId).HasColumnName("Department_ID");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Department_Name");
            entity.Property(e => e.HospitalId).HasColumnName("Hospital_ID");

            entity.HasOne(d => d.Hospital).WithMany(p => p.Departments)
                .HasForeignKey(d => d.HospitalId)
                .HasConstraintName("FK__Departmen__Hospi__4BAC3F29");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DoctorId).HasName("PK__Doctor__E59B530F36D746AF");

            entity.ToTable("Doctor");

            entity.Property(e => e.DoctorId).HasColumnName("Doctor_ID");
            entity.Property(e => e.DepartmentId).HasColumnName("Department_ID");
            entity.Property(e => e.DoctorFirstName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Doctor_First_Name");
            entity.Property(e => e.DoctorLastName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Doctor_Last_Name");
            entity.Property(e => e.DoctorPhoneNumber)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasColumnName("Doctor_Phone_Number");

            entity.HasOne(d => d.Department).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK__Doctor__Departme__5165187F");
        });

        modelBuilder.Entity<Hospital>(entity =>
        {
            entity.HasKey(e => e.HospitalId).HasName("PK__Hospital__09C0BD853C9EEBDE");

            entity.ToTable("Hospital");

            entity.Property(e => e.HospitalId).HasColumnName("Hospital_ID");
            entity.Property(e => e.HospitalAddress)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Hospital_Address");
            entity.Property(e => e.HospitalName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Hospital_Name");
            entity.Property(e => e.HospitalPhoneNumber)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Hospital_Phone_Number");
            entity.Property(e => e.State)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("PK__Patient__C1A88B590D5F965F");

            entity.ToTable("Patient");

            entity.Property(e => e.PatientId).HasColumnName("Patient_ID");
            entity.Property(e => e.PatientAddress)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Patient_Address");
            entity.Property(e => e.PatientFirstName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Patient_First_Name");
            entity.Property(e => e.PatientLastName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Patient_Last_Name");
            entity.Property(e => e.PatientPhoneNumber)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasColumnName("Patient_Phone_Number");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("PK__Staff__32D1F3C3EED4DCB3");

            entity.Property(e => e.StaffId).HasColumnName("Staff_ID");
            entity.Property(e => e.DepartmentId).HasColumnName("Department_ID");
            entity.Property(e => e.StaffAddress)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Staff_Address");
            entity.Property(e => e.StaffFirstName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Staff_First_Name");
            entity.Property(e => e.StaffLastName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Staff_Last_Name");
            entity.Property(e => e.StaffPhoneNumber)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasColumnName("Staff_Phone_Number");

            entity.HasOne(d => d.Department).WithMany(p => p.Staff)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK__Staff__Departmen__4E88ABD4");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
