using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using VerticalSliceArchictureDemo.Web.Common.Http.Exceptions;
using VerticalSliceArchictureDemo.Web.Persistence;

namespace VerticalSliceArchictureDemo.Web.Features.V1.Users.UpdateUser
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserRequest, UpdateUserResponse>
    {
        private readonly IClock _clock;
        private readonly DemoDbContext _dbContext;
        private readonly IMapper _mapper;

        public UpdateUserHandler(DemoDbContext dbContext, IMapper mapper, IClock clock)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _clock = clock;
        }

        public async Task<UpdateUserResponse> Handle(UpdateUserRequest request, CancellationToken cancellationToken)
        {
            var entry = await _dbContext.Users
                .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                .ConfigureAwait(false);

            if (entry == null)
            {
                throw new HttpNotFoundException();
            }

            entry.Username = request.Username;
            entry.LastModifiedOn = _clock.GetCurrentInstant().ToDateTimeUtc();

            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return _mapper.Map<UpdateUserResponse>(entry);
        }
    }
}
