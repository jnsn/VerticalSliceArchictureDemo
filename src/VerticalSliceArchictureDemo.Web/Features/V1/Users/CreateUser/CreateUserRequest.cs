using MediatR;

namespace VerticalSliceArchictureDemo.Web.Features.V1.Users.CreateUser
{
    public class CreateUserRequest : IRequest<CreateUserResponse>
    {
        public string Username { get; set; }
    }
}