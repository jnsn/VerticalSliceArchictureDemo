using MediatR;
using Newtonsoft.Json;

namespace VerticalSliceArchictureDemo.Web.Features.V1.Users.UpdateUser
{
    public class UpdateUserRequest : IRequest<UpdateUserResponse>
    {
        [JsonIgnore]
        public int Id { get; set; }

        public string Username { get; set; }
    }
}
