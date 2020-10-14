using AutoMapper;
using VerticalSliceArchictureDemo.Web.Domain.Entities;

namespace VerticalSliceArchictureDemo.Web.Features.V1.Users.GetUserById
{
    public class GetUserByIdMappingProfile : Profile
    {
        public GetUserByIdMappingProfile()
            => CreateMap<User, GetUserByIdResponse>();
    }
}
