using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using TaskManagerApi.Data;
using TaskManagerApi.Models;
using TaskManagerApi.Services;
using Threading = System.Threading.Tasks;
using Task = TaskManagerApi.Models.Task<string>;
using Microsoft.AspNetCore.SignalR.Client;

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

            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }


        [Fact]
        public async Threading.Task GetTasks_ShouldReturnUnauthorized_WhenUserIsUnauthenticated()
        {
            // Act
            var response = await _unauthenticatedUser.GetAsync("/api/Tasks");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Threading.Task Login_ShouldReturnOk_WhenCredentialsAreValid()
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
        public async Threading.Task PostTask_ShouldReturnBadRequest_WhenRequiredFieldsAreMissing()
        {
            // Arrange
            var body = new StringContent(
                JsonConvert.SerializeObject(new { AdditionalData = "2025-01-01" }),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _authenticatedUser.PostAsync("/api/Tasks", body);

            // Assert
            var contentResponse = await response.Content.ReadAsStringAsync();

            Assert.Equal("{\"message\":\"\",\"success\":false,\"errors\":[\"The field 'name', 'description', 'dueDate', 'status' is required\"],\"statusCode\":500}", contentResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Threading.Task PostTask_ShouldReturnBadRequest_WhenDueDateIsInvalid()
        {
            // Arrange
            var body = new StringContent(
                JsonConvert.SerializeObject(new
                {
                    Name = "Completar reporte",
                    Description = "Terminar el reporte mensual de ventas",
                    DueDate = DateTime.Now.AddDays(-5).Date.ToString("yyyy-MM-dd'T'HH:mm:ss"),
                    Status = "En progreso",
                }),
                Encoding.UTF8, "application/json"
            );

            // Act
            var response = await _authenticatedUser.PostAsync("/api/Tasks", body);

            // Assert
            var contentResponse = await response.Content.ReadAsStringAsync();

            Assert.Equal("{\"message\":\"\",\"success\":false,\"errors\":[\"The field 'dueDate' must be a future date\"],\"statusCode\":500}", contentResponse);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Threading.Task PostTask_ShouldReturnCorrectValue_WhenDueDateIsInFuture()
        {
            // Arrange
            var task = new Task
            {
                Name = "test@test.com",
                Description = "admin",
                Status = "Test",
                DueDate = DateTime.Now.AddDays(5)
            };
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            var body = new StringContent(
                JsonConvert.SerializeObject(task),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _authenticatedUser.PostAsync("/api/Tasks", body);

            // Assert
            Assert.Equal(5, _context.Tasks.Find(task.Id).RemainingDays);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Threading.Task PostTask_ShouldReturnCreated_WhenDataIsValid()
        {
            // Arrange
            var bodyData = new
            {
                Name = "Completar reporte",
                Description = "Terminar el reporte mensual de ventas",
                DueDate = DateTime.Now.AddDays(5).Date.ToString("yyyy-MM-dd'T'HH:mm:ss"),
                Status = "En progreso",
            };

            var body = new StringContent(
                JsonConvert.SerializeObject(bodyData),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _authenticatedUser.PostAsync("/api/Tasks", body);

            // Assert
            var contentResponse = await response.Content.ReadAsStringAsync();
            var dueDateStr = DateTime.Now.AddDays(5).Date.ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff");
            Assert.Equal("{\"data\":{\"id\":1,\"name\":\"Completar reporte\",\"description\":\"Terminar el reporte mensual de ventas\",\"dueDate\":\"" + dueDateStr + "\",\"status\":\"En progreso\",\"additionalData\":\"Medium Priority\",\"remainingDays\":5},\"success\":true,\"message\":\"Successfully created!\",\"statusCode\":201}", contentResponse);

            Task task = _context.Tasks.Find(1);
            Assert.Equal(bodyData.Name, task.Name);
            Assert.Equal(bodyData.Description, task.Description);
            Assert.True(DateTime.Now.AddDays(5).Date == task.DueDate);
            Assert.Equal(bodyData.Status, task.Status);
            Assert.Equal(5, task.RemainingDays);
            Assert.Equal("Medium Priority", task.AdditionalData);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Threading.Task PutTask_ShouldReturnOk_WhenDataIsValid()
        {
            var task = new Task
            {
                Name = "Test",
                Description = "admin",
                Status = "Test",
                DueDate = DateTime.Now.AddDays(1),
            };
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            var bodyData = new
            {
                Name = "Completar reporte",
                Description = "Terminar el reporte mensual de ventas",
                DueDate = DateTime.Now.AddDays(5).Date.ToString("yyyy-MM-dd'T'HH:mm:ss"),
                Status = "En progreso",
            };
            var body = new StringContent(
                JsonConvert.SerializeObject(bodyData),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _authenticatedUser.PutAsync($"/api/Tasks/{task.Id}", body);

            // Assert
            var contentResponse = await response.Content.ReadAsStringAsync();
            var dueDateStr = DateTime.Now.AddDays(5).Date.ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff");
            Assert.Equal("{\"data\":{\"id\":1,\"name\":\"Completar reporte\",\"description\":\"Terminar el reporte mensual de ventas\",\"dueDate\":\"" + dueDateStr + "\",\"status\":\"En progreso\",\"additionalData\":\"Medium Priority\",\"remainingDays\":5},\"success\":true,\"message\":\"Successfully updated!\",\"statusCode\":200}", contentResponse);

            _context.Entry(task).Reload();
            Assert.Equal(bodyData.Name, task.Name);
            Assert.Equal(bodyData.Description, task.Description);
            Assert.True(DateTime.Now.AddDays(5).Date == task.DueDate);
            Assert.Equal(bodyData.Status, task.Status);
            Assert.Equal(5, task.RemainingDays);
            Assert.Equal("Medium Priority", task.AdditionalData);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Threading.Task DeleteTask_ShouldReturnNoContent_WhenTaskExistsAndUserIsAuthorized()
        {
            // Arrange
            var task = new Task
            {
                Name = "test@test.com",
                Description = "admin",
                Status = "Test",
                DueDate = DateTime.Now.AddDays(3),
            };
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Act
            var response = await _authenticatedUser.DeleteAsync($"/api/Tasks/{task.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(0, _context.Tasks.Count());
        }

        [Fact]
        public async Threading.Task GetTask_ShouldReturnOkAndTaskList_WhenUserIsAuthenticated()
        {
            // Arrange
            var task = new Task
            {
                Name = "test@test.com",
                Description = "admin",
                Status = "Test",
                DueDate = DateTime.Now.AddDays(3).Date,
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
            var contentResponse = await response.Content.ReadAsStringAsync();
            var dueDateStr = DateTime.Now.AddDays(3).Date.ToString("yyyy-MM-dd'T'HH:mm:ss.fffffffK");
            Assert.Equal("{\"data\":{\"id\":1,\"name\":\"test@test.com\",\"description\":\"admin\",\"dueDate\":\"" + dueDateStr + "\",\"status\":\"Test\",\"additionalData\":null,\"remainingDays\":3},\"success\":true,\"message\":\"Successfully read!\",\"statusCode\":200}", contentResponse);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Threading.Task PostTask_ShouldPublishTaskCreatedEvent_WhenTaskIsCreated()
        {
            // Arrange
            var baseUri = _authenticatedUser.BaseAddress!.ToString().TrimEnd('/');
            var hubUrl = $"{baseUri}/notificationHub";
            var token = _authenticatedUser.DefaultRequestHeaders.Authorization.ToString().Replace("Bearer ", "");
            var welcomeReceived = new TaskCompletionSource<string>();
            var taskCreatedReceived = new TaskCompletionSource<string>();
            var bodyData = new
            {
                Name = "Completar reporte",
                Description = "Terminar el reporte mensual de ventas",
                DueDate = DateTime.Now.AddDays(5).Date.ToString("yyyy-MM-dd'T'HH:mm:ss"),
                Status = "En progreso",
            };

            var body = new StringContent(
                JsonConvert.SerializeObject(bodyData),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var connection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                options.AccessTokenProvider = () => Threading.Task.FromResult(token);
                options.HttpMessageHandlerFactory = _ => _factory.Server.CreateHandler();
            })
            .WithAutomaticReconnect()
            .Build();


            connection.On<string>("Global", message =>
            {
                if (message == "Connected to socket!")
                    welcomeReceived.TrySetResult(message);
                else if (message == "Task created!")
                    taskCreatedReceived.TrySetResult(message);
            });

            await connection.StartAsync();

            var response = await _authenticatedUser.PostAsync("/api/Tasks", body);

            var welcomeDone = await Threading.Task.WhenAny(welcomeReceived.Task, Threading.Task.Delay(3000));
            var taskCreatedDone = await Threading.Task.WhenAny(taskCreatedReceived.Task, Threading.Task.Delay(3000));


            // Assert
            Assert.True(welcomeDone == welcomeReceived.Task);
            Assert.True(taskCreatedDone == taskCreatedReceived.Task);
            Assert.Equal("Connected to socket!", welcomeReceived.Task.Result);
            Assert.Equal("Task created!", taskCreatedReceived.Task.Result);
        }
    }
}
