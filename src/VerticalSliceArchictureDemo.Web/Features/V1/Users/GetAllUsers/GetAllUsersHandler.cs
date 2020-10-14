using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VerticalSliceArchictureDemo.Web.Persistence;

namespace VerticalSliceArchictureDemo.Web.Features.V1.Users.GetAllUsers
{
    public class GetAllUsersHandler : IRequestHandler<GetAllUsersRequest, List<GetAllUsersResponse>>
    {
        private readonly DemoDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetAllUsersHandler(DemoDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<List<GetAllUsersResponse>> Handle(GetAllUsersRequest request, CancellationToken cancellationToken)
            => await _dbContext.Users
                .AsNoTracking()
                .ProjectTo<GetAllUsersResponse>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
    }
}
