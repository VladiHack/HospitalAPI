using AutoMapper;
using Hospital_API.Dto;

namespace Hospital_API.AutoMapper
{
    public class StaffProfile : Profile
    {
        public StaffProfile()
        {
            CreateMap<StaffDTO, Staff>()
                .ForMember(dest => dest.StaffId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.StaffFirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.StaffLastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.StaffAddress, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.StaffPhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));
        }
    }
}
