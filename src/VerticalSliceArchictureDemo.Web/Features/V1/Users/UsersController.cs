using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VerticalSliceArchictureDemo.Web.Features.V1.Users.CreateUser;
using VerticalSliceArchictureDemo.Web.Features.V1.Users.DeleteUser;
using VerticalSliceArchictureDemo.Web.Features.V1.Users.GetAllUsers;
using VerticalSliceArchictureDemo.Web.Features.V1.Users.GetUserById;
using VerticalSliceArchictureDemo.Web.Features.V1.Users.UpdateUser;

namespace VerticalSliceArchictureDemo.Web.Features.V1.Users
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
            => _mediator = mediator;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<GetAllUsersResponse>>> GetAll()
            => await _mediator.Send(new GetAllUsersRequest()).ConfigureAwait(false);

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetUserByIdResponse>> GetById([FromRoute] int id)
            => await _mediator.Send(new GetUserByIdRequest {Id = id}).ConfigureAwait(false);

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<CreateUserResponse>> Create([FromBody] CreateUserRequest request)
            => await _mediator.Send(request).ConfigureAwait(false);

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<UpdateUserResponse>> Update([FromRoute] int id, [FromBody] UpdateUserRequest request)
        {
            request.Id = id;

            return await _mediator.Send(request).ConfigureAwait(false);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task Delete([FromRoute] int id)
            => await _mediator.Send(new DeleteUserRequest {Id = id}).ConfigureAwait(false);
    }
}
