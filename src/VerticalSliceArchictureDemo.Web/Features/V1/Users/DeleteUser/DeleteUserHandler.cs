using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VerticalSliceArchictureDemo.Web.Common.Http.Exceptions;
using VerticalSliceArchictureDemo.Web.Persistence;

namespace VerticalSliceArchictureDemo.Web.Features.V1.Users.DeleteUser
{
    public class DeleteUserHandler : IRequestHandler<DeleteUserRequest>
    {
        private readonly DemoDbContext _dbContext;

        public DeleteUserHandler(DemoDbContext dbContext)
            => _dbContext = dbContext;

        public async Task<Unit> Handle(DeleteUserRequest request, CancellationToken cancellationToken)
        {
            var entry = await _dbContext.Users
                .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                .ConfigureAwait(false);

            if (entry == null)
            {
                throw new HttpNotFoundException();
            }

            _dbContext.Users.Remove(entry);

            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}