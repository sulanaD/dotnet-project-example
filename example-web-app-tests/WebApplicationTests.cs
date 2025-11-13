using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace example_web_app_tests
{
    /// <summary>
    /// Integration tests using TestServer to test the complete application pipeline
    /// These tests ensure the web application works end-to-end
    /// </summary>
    [TestClass]
    public sealed class WebApplicationTests
    {
        private WebApplicationFactory<Program>? _factory;
        private HttpClient? _client;

        [TestInitialize]
        public void Setup()
        {
            _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureServices(services =>
                {
                    // Add any test-specific services here
                });
            });
            _client = _factory.CreateClient();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _client?.Dispose();
            _factory?.Dispose();
        }

        [TestMethod]
        public async Task HomePage_ShouldReturnSuccessStatusCode()
        {
            // Act
            var response = await _client!.GetAsync("/");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [TestMethod]
        public async Task HomePage_ShouldContainExpectedContent()
        {
            // Act
            var response = await _client!.GetAsync("/");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(content.Length > 0, "Home page should contain content");
        }

        [TestMethod]
        public async Task PrivacyPage_ShouldReturnSuccessStatusCode()
        {
            // Act
            var response = await _client!.GetAsync("/Home/Privacy");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task ContactUsPage_ShouldReturnSuccessStatusCode()
        {
            // Act
            var response = await _client!.GetAsync("/Home/ContactUs");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task NonExistentPage_ShouldReturn404()
        {
            // Act
            var response = await _client!.GetAsync("/NonExistent/Page");

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task ErrorPage_ShouldReturnSuccessStatusCode()
        {
            // Act
            var response = await _client!.GetAsync("/Home/Error");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task MultipleRequests_ShouldAllSucceed()
        {
            // Arrange
            var urls = new[] { "/", "/Home/Privacy", "/Home/ContactUs" };
            var tasks = new List<Task<HttpResponseMessage>>();

            // Act
            foreach (var url in urls)
            {
                tasks.Add(_client!.GetAsync(url));
            }
            var responses = await Task.WhenAll(tasks);

            // Assert
            Assert.AreEqual(urls.Length, responses.Length);
            foreach (var response in responses)
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task ApplicationStartup_ShouldConfigureServicesCorrectly()
        {
            // Arrange
            using var scope = _factory!.Services.CreateScope();
            var services = scope.ServiceProvider;

            // Act & Assert
            var environment = services.GetService<IWebHostEnvironment>();
            Assert.IsNotNull(environment, "Environment service should be registered");
            Assert.AreEqual("Testing", environment.EnvironmentName);
        }
    }
}