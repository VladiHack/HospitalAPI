using AutoMapper;
using Hospital_API.Dto;

namespace Hospital_API.AutoMapper
{
    public class AppointmentProfile : Profile
    {
        public AppointmentProfile()
        {
            CreateMap<AppointmentDTO, Appointment>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateOnly.Parse(src.Date)));
        }
    }
}
