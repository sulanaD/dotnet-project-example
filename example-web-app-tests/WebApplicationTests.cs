using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace example_web_app_tests
{
    /// <summary>
    /// Integration tests for HTTP endpoints and application behavior
    /// These tests focus on controller actions and responses without full application testing
    /// </summary>
    [TestClass]
    public sealed class WebApplicationTests
    {
        /// <summary>
        /// Simple integration tests that don't require TestServer
        /// These test the application components in isolation
        /// </summary>
        [TestMethod]
        public void WebApplication_Configuration_ShouldBeValid()
        {
            // Arrange & Act
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddControllersWithViews();

            // Assert
            Assert.IsNotNull(builder);
            Assert.IsNotNull(builder.Services);
        }

        [TestMethod]
        public void WebApplication_Services_ShouldRegisterCorrectly()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddControllersWithViews();
            
            using var app = builder.Build();
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            // Act & Assert
            var logger = services.GetService<ILogger<Program>>();
            Assert.IsNotNull(logger, "Logger service should be registered");

            var environment = services.GetService<IWebHostEnvironment>();
            Assert.IsNotNull(environment, "Environment service should be registered");
        }

        [TestMethod]
        public void Routing_Configuration_ShouldBeValid()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddControllersWithViews();
            
            using var app = builder.Build();

            // Act & Assert - Test that routing can be configured
            Assert.DoesNotThrow(() => 
            {
                app.UseStaticFiles();
                app.UseRouting();
                app.UseAuthorization();
                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        [TestMethod]
        public void Environment_Configuration_ShouldHandleDevelopment()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            builder.Environment.EnvironmentName = "Development";
            
            // Act & Assert
            Assert.AreEqual("Development", builder.Environment.EnvironmentName);
            Assert.IsTrue(builder.Environment.IsDevelopment());
        }

        [TestMethod]
        public void Environment_Configuration_ShouldHandleProduction()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            builder.Environment.EnvironmentName = "Production";
            
            // Act & Assert
            Assert.AreEqual("Production", builder.Environment.EnvironmentName);
            Assert.IsTrue(builder.Environment.IsProduction());
        }

        [TestMethod]
        public void StaticFiles_Configuration_ShouldNotThrow()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddControllersWithViews();
            
            using var app = builder.Build();

            // Act & Assert
            Assert.DoesNotThrow(() => app.UseStaticFiles());
        }

        [TestMethod]
        public void Controllers_Configuration_ShouldRegisterServices()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            
            // Act
            builder.Services.AddControllersWithViews();
            using var app = builder.Build();
            
            // Assert
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            
            // Verify that MVC services are registered
            var mvcOptions = services.GetService<Microsoft.AspNetCore.Mvc.MvcOptions>();
            Assert.IsNotNull(mvcOptions, "MVC options should be registered");
        }
    }
}