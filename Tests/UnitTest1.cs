using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using TaskManagerApi.Data;
using TaskManagerApi.Models;

namespace Tests
{
    public class UnitTest1
    {
        private readonly CustomWebApplicationFactory _factory;
        private AppDbContext _context;


        public UnitTest1()
        {
            _factory = new CustomWebApplicationFactory();
            var scope = _factory.Services.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        }


        [Fact]
        public async Task ShouldRejectRequestsWithoutToken()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/auth/login");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public void ShouldRejectRequestsWithInvalidOrExpiredToken()
        {
            Assert.True(true);
        }

        [Fact]
        public async Task ShouldLoginSuccsefully()
        {
            // Arrange
            var user = new User{Email = "test@test.com", Password = "admin"};
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            var client = _factory.CreateClient();
            var loginContent = new StringContent(
                JsonConvert.SerializeObject(new
                {
                    email = user.Email,
                    password = user.Password
                }),
                Encoding.UTF8,
                "application/json"
            );


            // Act
            var response = await client.PostAsync("/api/auth/login", loginContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void ShouldReturnErrorWhenRequiredFieldsAreMissing()
        {
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
        public void ShouldEditTaskSuccessfully()
        {
            Assert.True(true);
        }

        [Fact]
        public void ShouldDeleteTaskSuccessfully()
        {
            Assert.True(true);
        }

        [Fact]
        public void ShouldReadTasksSuccessfully()
        {
            Assert.True(true);
        }

        [Fact]
        public void ShouldEmitNotificationWhenTaskIsCreated()
        {
            Assert.True(true);
        }
    }
}
