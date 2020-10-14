using AutoMapper;
using VerticalSliceArchictureDemo.Web.Domain.Entities;

namespace VerticalSliceArchictureDemo.Web.Features.V1.Users.GetAllUsers
{
    public class GetAllUsersMappingProfile : Profile
    {
        public GetAllUsersMappingProfile()
            => CreateMap<User, GetAllUsersResponse>();
    }
}
