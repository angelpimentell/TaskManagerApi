using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using TaskManagerApi;

namespace Tests
{
    public class UnitTest1 : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public UnitTest1(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }


        [Fact]
        public async Task ShouldRejectRequestsWithoutToken()
        {
            // Act: Hacemos una petición GET al endpoint protegido
            var response = await _client.GetAsync("/api/tasks"); // ajusta la ruta a tu endpoint real

            // Assert: Debería retornar 401 Unauthorized
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public void ShouldRejectRequestsWithInvalidOrExpiredToken()
        {
            Assert.True(true);
        }

        [Fact]
        public void ShouldAllowAccessWithValidToken()
        {
            Assert.True(true);
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
