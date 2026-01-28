namespace Expenses.Domain.Entities
{
    public abstract class BaseEntity<TKey>
    {
        public TKey Id { get; set; } = default!;
        public long UserId { get; set; }
        public long TenantId { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public int Status { get; set; }

        public void Delete()
        {
            Status = 1;
        }
    }
}
