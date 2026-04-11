using Application.DTOs.Requests.ServiceRequest;
using Application.DTOs.Response;
using Application.DTOs.Responses;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class ServiceMappingProfile : Profile
    {
        public ServiceMappingProfile()
        {
            CreateMap<Service, ServiceResponse>();
            CreateMap<ServiceCreateRequest, Service>();
            CreateMap<Feedback, FeedbackResponse>();
        }
    }
}