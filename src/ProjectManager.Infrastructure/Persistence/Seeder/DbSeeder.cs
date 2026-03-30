using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Enums;
using ProjectManager.Infrastructure.Persistence;

namespace ProjectManager.Infrastructure.Persistence.Seeder
{
    public static class DbSeeder
    {
        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var adminEmail = "admin@projectmanager.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var user = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, "Admin123!");
            }
        }
        public static async Task SeedProjectsAndTasksAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            if (await context.Projects.AnyAsync())
            {
                return; // DB has data
            }

            var projects = new List<Project>
            {
                new Project { Id = Guid.NewGuid(), Name = "Planificación de Marketing 2024", Description = "Diseño de campañas trimestrales para redes sociales.", Status = ProjectStatus.Active },
                new Project { Id = Guid.NewGuid(), Name = "Desarrollo E-commerce", Description = "Migración de plataforma a microservicios.", Status = ProjectStatus.Draft },
                new Project { Id = Guid.NewGuid(), Name = "Auditoría de Sistemas", Description = "Evaluación de seguridad y rendimiento anual.", Status = ProjectStatus.Completed },
                new Project { Id = Guid.NewGuid(), Name = "Lanzamiento Producto X", Description = "Estrategia GTM para el nuevo gadget tecnológico.", Status = ProjectStatus.Active },
                new Project { Id = Guid.NewGuid(), Name = "Rediseño Web Corporativa", Description = "Actualización de UI/UX a tendencias modernas.", Status = ProjectStatus.Draft },
                new Project { Id = Guid.NewGuid(), Name = "Capacitación de Personal", Description = "Nuevos módulos de entrenamiento en IA.", Status = ProjectStatus.Active },
                new Project { Id = Guid.NewGuid(), Name = "Optimización SEO", Description = "Mejora del ranking orgánico en motores de búsqueda.", Status = ProjectStatus.Completed },
                new Project { Id = Guid.NewGuid(), Name = "Infraestructura Cloud", Description = "Despliegue de clusters Kubernetes en AWS.", Status = ProjectStatus.Active },
                new Project { Id = Guid.NewGuid(), Name = "App Móvil Delivery", Description = "Version 2.0 con nuevas integraciones de pago.", Status = ProjectStatus.Draft },
                new Project { Id = Guid.NewGuid(), Name = "Gestión de Residuos", Description = "Iniciativa de sostenibilidad empresarial.", Status = ProjectStatus.Active },
                new Project { Id = Guid.NewGuid(), Name = "Eventos de Networking", Description = "Organización de webinars y workshops presenciales.", Status = ProjectStatus.Completed },
                new Project { Id = Guid.NewGuid(), Name = "Soporte Técnico Premium", Description = "Implementación de sistema de tickets 24/7.", Status = ProjectStatus.Active }
            };

            foreach (var project in projects)
            {
                context.Projects.Add(project);

                var taskCount = new Random().Next(3, 8);
                for (int i = 0; i < taskCount; i++)
                {
                    context.TaskItems.Add(new TaskItem
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = project.Id,
                        Title = $"Tarea {i + 1} para {project.Name}",
                        Priority = (TaskPriority)new Random().Next(0, 3),
                        Order = i + 1,
                        IsCompleted = new Random().Next(0, 2) == 1
                    });
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
