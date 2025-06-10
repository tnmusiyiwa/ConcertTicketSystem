using AutoMapper;
using ConcertTicketSystem.Application.DTOs;
using ConcertTicketSystem.Domain.Entities;

namespace ConcertTicketSystem.Infrastructure.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Event mappings
            CreateMap<Event, EventDto>()
                .ForMember(dest => dest.TicketTypes, opt => opt.MapFrom(src => src.TicketTypes));
            CreateMap<CreateEventDto, Event>();
            CreateMap<UpdateEventDto, Event>();

            // TicketType mappings
            CreateMap<TicketType, TicketTypeDto>()
                .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.Event.Name));
            CreateMap<CreateTicketTypeDto, TicketType>();
            CreateMap<UpdateTicketTypeDto, TicketType>();

            // Ticket mappings
            CreateMap<Ticket, TicketDto>()
                .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.Event.Name))
                .ForMember(dest => dest.TicketTypeName, opt => opt.MapFrom(src => src.TicketType.Name))
                .ForMember(dest => dest.Venue, opt => opt.MapFrom(src => src.Event.Venue))
                .ForMember(dest => dest.EventDate, opt => opt.MapFrom(src => src.Event.EventDate));
            CreateMap<ReserveTicketDto, Ticket>();
        }
    }
}
