using MediatR;
using Newtonsoft.Json;

namespace VerticalSliceArchictureDemo.Web.Features.V1.Users.DeleteUser
{
    public class DeleteUserRequest : IRequest
    {
        [JsonIgnore]
        public int Id { get; set; }
    }
}
