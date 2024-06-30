using AutoMapper;
using Hospital_API.Dto;

namespace Hospital_API.AutoMapper
{
    public class HospitalProfile : Profile
    {
        public HospitalProfile()
        {
            CreateMap<HospitalDTO, Hospital>()
                .ForMember(dest => dest.HospitalId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.HospitalName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.HospitalAddress, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.HospitalPhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));
        }
    }
}
