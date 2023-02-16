using System;
using System.Threading.Tasks;
using GreeenGarden.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.GenericRepository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly GreenGardenDbContext context;
        private DbSet<T> _entities;

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
            await _entities.AddAsync(entity);
            await Update();
            return (Guid)entity.GetType().GetProperty("Id").GetValue(entity);
        }
    

        public async Task Update()
            {
                await context.SaveChangesAsync();
            }

        
    }
}

