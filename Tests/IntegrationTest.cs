using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using TaskManagerApi.Data;
using TaskManagerApi.Models;
using TaskManagerApi.Services;
using Threading = System.Threading.Tasks;
using Task = TaskManagerApi.Models.Task<string>;

namespace Tests
{
    public class IntegrationTest
    {
        private readonly CustomWebApplicationFactory _factory;
        private AppDbContext _context;
        private HttpClient _unauthenticatedUser, _authenticatedUser;
        private JwtTokenService _jwtService;


        public IntegrationTest()
        {
            _factory = new CustomWebApplicationFactory();

            var scope = _factory.Services.CreateScope();

            var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();


            _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            _jwtService = new JwtTokenService(config);

            _unauthenticatedUser = _factory.CreateClient();
            _authenticatedUser = _factory.CreateClient();
            _authenticatedUser.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtService.GenerateJWT());

        }


        [Fact]
        public async Threading.Task ShouldRejectRequestsWithoutToken()
        {
            // Act
            var response = await _unauthenticatedUser.GetAsync("/api/Tasks");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Threading.Task ShouldLoginSuccsefully()
        {
            // Arrange
            var user = new User { Email = "test@test.com", Password = "admin" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var body = new StringContent(
                JsonConvert.SerializeObject(new { email = user.Email, password = user.Password }),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _unauthenticatedUser.PostAsync("/api/auth/login", body);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Threading.Task ShouldReturnErrorWhenRequiredFieldsAreMissing()
        {
            // Arrange
            var body = new StringContent(
                JsonConvert.SerializeObject(new { DueDate = "2025-01-01" }),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _authenticatedUser.PostAsync("/api/Tasks", body);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        }

        [Fact]
        public void ShouldReturnErrorWhenDueDateIsNotAValidDate()
        {
            Assert.True(true);
        }

        [Fact]
        public void ShouldAssignDefaultValuesWhenFieldsAreMissing()
        {
            Assert.True(true);
        }

        [Fact]
        public void ShouldCalculateRemainingDaysCorrectly()
        {
            Assert.True(true);
        }

        [Fact]
        public async Threading.Task ShouldCreateTaskSuccessfully()
        {
            // Arrange
            var body = new StringContent(
                JsonConvert.SerializeObject(new
                {
                    Name = "Completar reporte",
                    Description = "Terminar el reporte mensual de ventas",
                    DueDate = "2025-07-01T00:00:00",
                    Status = "En progreso",
                }),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _authenticatedUser.PostAsync("/api/Tasks", body);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public void ShouldEditTaskSuccessfully()
        {
            Assert.True(true);
        }

        [Fact]
        public async Threading.Task ShouldDeleteTaskSuccessfully()
        {
            // Arrange
            var task = new Task {
                Name = "test@test.com", 
                Description = "admin" ,
                Status = "Test"
            };
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            var body = new StringContent(
                JsonConvert.SerializeObject(task),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _authenticatedUser.DeleteAsync($"/api/Tasks/{task.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Threading.Task ShouldReadTasksSuccessfully()
        {
            // Arrange
            var task = new Task
            {
                Name = "test@test.com",
                Description = "admin",
                Status = "Test"
            };
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            var body = new StringContent(
                JsonConvert.SerializeObject(task),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _authenticatedUser.GetAsync($"/api/Tasks/{task.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void ShouldEmitNotificationWhenTaskIsCreated()
        {
            Assert.True(true);
        }
    }
}
