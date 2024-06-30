namespace Hospital_API.Services.Patients
{
    public interface IPatientService
    {
        Task<bool> ExistsByIdAsync(int id);
    }
}
