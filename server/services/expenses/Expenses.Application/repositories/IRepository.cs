namespace Expenses.Application.repositories
{
    public interface IRepository<T, TKey> where T : class
    {
        Task<T?> GetByIdAsync(TKey id);
        Task<List<T>> GetAllAsync();
        Task<List<T>> GetAllWithPredicate(Predicate<T> predicate);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}
