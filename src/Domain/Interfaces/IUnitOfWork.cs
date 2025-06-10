namespace ConcertTicketSystem.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IEventRepository Events { get; }
        ITicketTypeRepository TicketTypes { get; }
        ITicketRepository Tickets { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
