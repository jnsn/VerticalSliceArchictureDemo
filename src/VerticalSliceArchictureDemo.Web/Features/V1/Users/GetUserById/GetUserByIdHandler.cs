using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VerticalSliceArchictureDemo.Web.Common.Http.Exceptions;
using VerticalSliceArchictureDemo.Web.Persistence;

namespace VerticalSliceArchictureDemo.Web.Features.V1.Users.GetUserById
{
    public class GetUserByIdHandler : IRequestHandler<GetUserByIdRequest, GetUserByIdResponse>
    {
        private readonly DemoDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetUserByIdHandler(DemoDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<GetUserByIdResponse> Handle(GetUserByIdRequest request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .AsNoTracking()
                .Where(x => x.Id == request.Id)
                .ProjectTo<GetUserByIdResponse>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (user == null)
            {
                throw new HttpNotFoundException();
            }

            return user;
        }
    }
}
