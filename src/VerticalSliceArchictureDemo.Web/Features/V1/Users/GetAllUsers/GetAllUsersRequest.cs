using System.Collections.Generic;
using MediatR;

namespace VerticalSliceArchictureDemo.Web.Features.V1.Users.GetAllUsers
{
    public class GetAllUsersRequest : IRequest<List<GetAllUsersResponse>>
    {
    }
}
