using ProjectManager.Application.DTOs;
using ProjectManager.Application.Interfaces;
using ProjectManager.Application.Services;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Enums;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;
using MockQueryable.Moq;
using Microsoft.EntityFrameworkCore;

namespace ProjectManager.Tests
{
    public class ProjectServiceTests
    {
        private readonly Mock<IGenericRepository<Project>> _projectRepoMock;
        private readonly Mock<IGenericRepository<TaskItem>> _taskRepoMock;
        private readonly ProjectService _service;

        public ProjectServiceTests()
        {
            _projectRepoMock = new Mock<IGenericRepository<Project>>();
            _taskRepoMock = new Mock<IGenericRepository<TaskItem>>();
            _service = new ProjectService(_projectRepoMock.Object, _taskRepoMock.Object);
        }

        [Fact]
        public async Task ActivateProject_WithTasks_ShouldSucceed()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var project = new Project
            {
                Id = projectId,
                Name = "Test Project",
                Description = "Test",
                Status = ProjectStatus.Draft,
                Tasks = new List<TaskItem>
                {
                    new TaskItem { Id = Guid.NewGuid(), ProjectId = projectId, Title = "Task 1", Order = 1 }
                }
            };

            var projects = new List<Project> { project }.AsQueryable().BuildMockDbSet();
            _projectRepoMock.Setup(r => r.GetQueryable()).Returns(projects.Object);

            // Act
            await _service.ActivateAsync(projectId);

            // Assert
            Assert.Equal(ProjectStatus.Active, project.Status);
            _projectRepoMock.Verify(r => r.UpdateAsync(project), Times.Once);
        }

        [Fact]
        public async Task ActivateProject_WithoutTasks_ShouldFail()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var project = new Project
            {
                Id = projectId,
                Name = "Test Project",
                Description = "Test",
                Status = ProjectStatus.Draft,
                Tasks = new List<TaskItem>()
            };

            var projects = new List<Project> { project }.AsQueryable().BuildMockDbSet();
            _projectRepoMock.Setup(r => r.GetQueryable()).Returns(projects.Object);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ActivateAsync(projectId));
            Assert.Contains("no tasks", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CompleteProject_WithPendingTasks_ShouldFail()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var project = new Project
            {
                Id = projectId,
                Name = "Test Project",
                Description = "Test",
                Status = ProjectStatus.Active,
                Tasks = new List<TaskItem>
                {
                    new TaskItem { Id = Guid.NewGuid(), ProjectId = projectId, Title = "Task 1", Order = 1, IsCompleted = true },
                    new TaskItem { Id = Guid.NewGuid(), ProjectId = projectId, Title = "Task 2", Order = 2, IsCompleted = false }
                }
            };

            var projects = new List<Project> { project }.AsQueryable().BuildMockDbSet();
            _projectRepoMock.Setup(r => r.GetQueryable()).Returns(projects.Object);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CompleteAsync(projectId));
            Assert.Contains("pending tasks", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task DeleteProject_ShouldBeDelete()
        {
            // Arrange
            var projectId = Guid.NewGuid();

            // Act
            await _service.DeleteAsync(projectId);

            // Assert - Verify that physical delete was called, not soft delete
            _projectRepoMock.Verify(r => r.DeleteAsync(projectId), Times.Once);
        }
    }
}
