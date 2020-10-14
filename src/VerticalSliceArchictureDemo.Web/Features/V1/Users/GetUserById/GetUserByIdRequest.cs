using MediatR;
using Newtonsoft.Json;

namespace VerticalSliceArchictureDemo.Web.Features.V1.Users.GetUserById
{
    public class GetUserByIdRequest : IRequest<GetUserByIdResponse>
    {
        [JsonIgnore]
        public int Id { get; set; }
    }
}
