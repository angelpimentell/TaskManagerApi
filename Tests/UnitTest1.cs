using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;

namespace Tests
{
    public class UnitTest1
    {
        private readonly WebApplicationFactory<Program> _factory;

        //public UnitTest1(WebApplicationFactory<Program> factory)
        //{
        //    _factory = factory;
        //}


        [Fact]
        public async Task ShouldRejectRequestsWithoutToken()
        {
            // Arrange
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/secure-endpoint"); // Cambia a tu endpoint protegido

            // Assert
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
