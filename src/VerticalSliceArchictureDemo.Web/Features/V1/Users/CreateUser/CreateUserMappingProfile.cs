using AutoMapper;
using VerticalSliceArchictureDemo.Web.Domain.Entities;

namespace VerticalSliceArchictureDemo.Web.Features.V1.Users.CreateUser
{
    public class CreateUserMappingProfile : Profile
    {
        public CreateUserMappingProfile()
            => CreateMap<User, CreateUserResponse>();
    }
}