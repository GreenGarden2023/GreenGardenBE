﻿using GreeenGarden.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.GenericRepository
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
#pragma warning disable CS8605 // Unboxing a possibly null value.
            return (Guid)entity.GetType().GetProperty("Id").GetValue(entity);
#pragma warning restore CS8605 // Unboxing a possibly null value.
        }


        public async Task Update()
        {
            _ = await context.SaveChangesAsync();
        }


    }
}

