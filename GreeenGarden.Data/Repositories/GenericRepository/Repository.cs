using GreeenGarden.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly GreenGardenDbContext context;
        private readonly DbSet<T> _entities;

        public Repository(GreenGardenDbContext context)
        {
            this.context = context;
            _entities = context.Set<T>();
        }

        public async Task<T?> Get(Guid id)
        {
            return await _entities.FindAsync(id);
        }

        public async Task<Guid> Insert(T entity)
        {
            _ = await _entities.AddAsync(entity);
            await Update();
            return (Guid)entity.GetType().GetProperty("Id").GetValue(entity);
        }


        public async Task Update()
        {
            _ = await context.SaveChangesAsync();
        }


    }
}

