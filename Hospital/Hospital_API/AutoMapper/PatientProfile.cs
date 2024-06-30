using AutoMapper;
using Hospital_API.Dto;

namespace Hospital_API.AutoMapper
{
    public class PatientProfile : Profile
    {
        public PatientProfile()
        {
            CreateMap<PatientDTO, Patient>()
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PatientFirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.PatientLastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.PatientAddress, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.PatientPhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));
        }
    }
}
