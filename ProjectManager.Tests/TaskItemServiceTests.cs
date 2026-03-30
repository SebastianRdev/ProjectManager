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

namespace ProjectManager.Tests
{
    public class TaskItemServiceTests
    {
        private readonly Mock<IGenericRepository<TaskItem>> _taskRepoMock;
        private readonly Mock<IGenericRepository<Project>> _projectRepoMock;
        private readonly TaskItemService _service;

        public TaskItemServiceTests()
        {
            _taskRepoMock = new Mock<IGenericRepository<TaskItem>>();
            _projectRepoMock = new Mock<IGenericRepository<Project>>();
            _service = new TaskItemService(_taskRepoMock.Object, _projectRepoMock.Object);
        }

        [Fact]
        public async Task CreateTask_WithDuplicateOrder_ShouldFail()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var project = new Project { Id = projectId, Name = "Test", Description = "Test" };
            _projectRepoMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);

            var existingTasks = new List<TaskItem>
            {
                new TaskItem { Id = Guid.NewGuid(), ProjectId = projectId, Title = "Existing Task", Order = 1 }
            };
            _taskRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<TaskItem, bool>>>()))
                .ReturnsAsync(existingTasks);

            var dto = new CreateTaskItemDto { Title = "New Task", Order = 1, Priority = "Medium" };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(projectId, dto));
            Assert.Contains("order already exists", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CreateTask_WithUniqueOrder_ShouldSucceed()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var project = new Project { Id = projectId, Name = "Test", Description = "Test" };
            _projectRepoMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);

            _taskRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<TaskItem, bool>>>()))
                .ReturnsAsync(new List<TaskItem>());

            var dto = new CreateTaskItemDto { Title = "New Task", Order = 1, Priority = "High" };

            // Act
            var result = await _service.CreateAsync(projectId, dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Task", result.Title);
            Assert.Equal("High", result.Priority);
            Assert.Equal(1, result.Order);
            _taskRepoMock.Verify(r => r.AddAsync(It.IsAny<TaskItem>()), Times.Once);
        }

        [Fact]
        public async Task CompleteTask_ShouldSetIsCompletedTrue()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new TaskItem { Id = taskId, Title = "Task", IsCompleted = false };
            _taskRepoMock.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync(task);

            // Act
            await _service.CompleteAsync(taskId);

            // Assert
            Assert.True(task.IsCompleted);
            _taskRepoMock.Verify(r => r.UpdateAsync(task), Times.Once);
        }
    }
}
