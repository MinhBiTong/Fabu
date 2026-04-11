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
            CreateMap<RechargePlan, RechargePlanResponse>().ReverseMap();
            CreateMap<Coupon, CouponApplyResult>().ReverseMap();
            CreateMap<Payment, PaymentResponse>().ReverseMap();
            CreateMap<Transaction,  TransactionResponse>().ReverseMap();
            CreateMap<Account, AccountResponse>().ReverseMap();
            
        }
    }
}
