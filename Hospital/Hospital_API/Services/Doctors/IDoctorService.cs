namespace Hospital_API.Services.Doctors
{
    public interface IDoctorService
    {
        Task<bool> ExistsByIdAsync(int id);
    }
}
