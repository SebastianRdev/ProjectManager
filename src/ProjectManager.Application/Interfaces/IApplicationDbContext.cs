using ProjectManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectManager.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Project> Projects { get; }
        DbSet<TaskItem> TaskItems { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
