using AutoMapper;
using VerticalSliceArchictureDemo.Web.Domain.Entities;

namespace VerticalSliceArchictureDemo.Web.Features.V1.Users.UpdateUser
{
    public class UpdateUserMappingProfile : Profile
    {
        public UpdateUserMappingProfile()
            => CreateMap<User, UpdateUserResponse>();
    }
}
