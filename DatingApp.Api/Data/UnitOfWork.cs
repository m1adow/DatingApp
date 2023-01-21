using AutoMapper;
using DatingApp.Api.Interfaces;

namespace DatingApp.Api.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public IUserRepository UserRepository => new UserRepository(this.context, this.mapper);

        public IMessageRepository MessageRepository => new MessageRepository(this.context, this.mapper);

        public ILikesRepository LikesRepository => new LikesRepository(this.context);

        public UnitOfWork(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<bool> CompleteAsync()
        {
            return await this.context.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return this.context.ChangeTracker.HasChanges();
        }
    }
}

