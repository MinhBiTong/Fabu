using Application.DTOs.Requests.AccountRequest;
using Application.DTOs.Requests.CustomerRequest;
using Application.DTOs.Requests.CustomerServiceRequest;
using Application.DTOs.Requests.PaymentRequest;
using Application.DTOs.Requests.TransactionRequest;
using Application.DTOs.Requests.UserRequest;
using Application.DTOs.Responses;
using Application.DTOs.Responses.AccountResponse;
using Application.DTOs.Responses.CouponResponse;
using Application.DTOs.Responses.PaymentResponse;
using Application.DTOs.Responses.TransactionResponse;
using Application.DTOs.Responses.UserResponse;
using Application.Features.RechargePlans.Commands;
using Application.Features.RechargePlans.Dtos;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapper
{
    public class MappingProfile : Profile 
    {
        public MappingProfile()
        {
            CreateMap<Customer, CustomerResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ReverseMap();

            // Map từ Request sang Entity (để lưu vào DB)
            CreateMap<CustomerCreateRequest, Customer>();
            CreateMap<CustomerUpdateRequest, Customer>();
            CreateMap<TransactionCreateRequest, Transaction>();
            CreateMap<CustomerServiceCreateRequest, Domain.Entities.CustomerService>();
            CreateMap<CreateUserRequest, User>();
            CreateMap<UpdateUserRequest, User>();
            CreateMap<PaymentCreateRequest, Domain.Entities.Payment>();
            CreateMap<PaymentUpdateRequest, Domain.Entities.Payment>();
            CreateMap<AccountCreateRequest, Account>();


            CreateMap<User, UserResponse>().ReverseMap();
            CreateMap<Account, AccountResponse>().ReverseMap();
            CreateMap<RechargePlan, RechargePlanResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => (int)src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.PlanName))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.Points, opt => opt.MapFrom(src => (int)src.BonusAmount))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
            CreateMap<RechargePlan, RechargePlanReadDto>();
            CreateMap<CreateRechargePlanCommand, RechargePlan>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? string.Empty));
            CreateMap<UpdateRechargePlanCommand, RechargePlan>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? string.Empty));
            CreateMap<Coupon, CouponApplyResult>().ReverseMap();
            CreateMap<Payment, PaymentResponse>().ReverseMap();
            CreateMap<Transaction, TransactionResponse>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
                .ForMember(dest => dest.CouponUsageCount, opt => opt.MapFrom(src => src.CouponUsages == null ? 0 : src.CouponUsages.Count));
            CreateMap<Account, AccountResponse>().ReverseMap();
            
        }
    }
}
