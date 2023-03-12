namespace GreeenGarden.Data.Repositories.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<T?> Get(Guid id);
        Task<Guid> Insert(T entity);
        // Update / Delete = Enum status equal 0
        Task Update();

    }
}


