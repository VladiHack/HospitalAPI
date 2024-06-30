using AutoMapper;
using Hospital_API.Dto;

namespace Hospital_API.AutoMapper
{
    public class DoctorProfile : Profile
    {
        public DoctorProfile()
        {
            CreateMap<DoctorDTO, Doctor>()
                .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.DoctorFirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.DoctorLastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.DoctorPhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));
        }
    }
}
