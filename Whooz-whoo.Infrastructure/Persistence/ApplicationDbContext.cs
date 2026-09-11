using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Whooz_whoo.Domain.Common;
using Whooz_whoo.Domain.Entities;
using Whooz_whoo.Domain.Interfaces;

namespace Whooz_whoo.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        private readonly IDomainEventDispatcher _domainEventDispatcher;
        private readonly ICurrentUserService _currentUserService;
        private IDbContextTransaction? _currentTransaction;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IDomainEventDispatcher domainEventDispatcher,
            ICurrentUserService currentUserService) : base(options)
        {
            _domainEventDispatcher = domainEventDispatcher;
            _currentUserService = currentUserService;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<EventAnalytics> EventAnalytics { get; set; }

        public new async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddTimestamps();
            var result = await base.SaveChangesAsync(cancellationToken);
            await DispatchDomainEvents();
            return result;
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await DispatchDomainEvents();
            await base.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _currentTransaction = await Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _currentTransaction?.CommitAsync(cancellationToken);
           // await _currentTransaction?.DisposeAsync();
            _currentTransaction = null;
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _currentTransaction?.RollbackAsync(cancellationToken);
           // await _currentTransaction?.DisposeAsync();
            _currentTransaction = null;
        }

        public IRepository<T> GetRepository<T>() where T : BaseEntity
        {
            return new EfRepository<T>(this);
        }

        private void AddTimestamps()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;
                if (entry.State == EntityState.Added)
                {
                    //.CreatedAt = DateTime.UtcNow;
                }
                //entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        private async Task DispatchDomainEvents()
        {
            var domainEntities = ChangeTracker
                .Entries<AggregateRoot>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
                .Select(x => x.Entity)
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(x => x.DomainEvents)
                .ToList();

            domainEntities.ForEach(entity => entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await _domainEventDispatcher.DispatchAsync(domainEvent);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // Global query filter for soft delete
            modelBuilder.Entity<Event>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
            modelBuilder.Entity<Membership>().HasQueryFilter(m => !m.IsDeleted);

            base.OnModelCreating(modelBuilder);
        }
    }
}
