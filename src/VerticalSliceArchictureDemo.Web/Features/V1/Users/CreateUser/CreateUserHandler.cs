using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using NodaTime;
using VerticalSliceArchictureDemo.Web.Domain.Entities;
using VerticalSliceArchictureDemo.Web.Persistence;

namespace VerticalSliceArchictureDemo.Web.Features.V1.Users.CreateUser
{
    public class CreateUserHandler : IRequestHandler<CreateUserRequest, CreateUserResponse>
    {
        private readonly IClock _clock;
        private readonly DemoDbContext _dbContext;
        private readonly IMapper _mapper;

        public CreateUserHandler(DemoDbContext dbContext, IMapper mapper, IClock clock)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _clock = clock;
        }

        public async Task<CreateUserResponse> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {
            var entry = new User
            {
                Username = request.Username,
                CreatedOn = _clock.GetCurrentInstant().ToDateTimeUtc(),
                LastModifiedOn = _clock.GetCurrentInstant().ToDateTimeUtc()
            };

            await _dbContext.Users.AddAsync(entry, cancellationToken).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return _mapper.Map<CreateUserResponse>(entry);
        }
    }
}
