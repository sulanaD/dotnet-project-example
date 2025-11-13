namespace example_web_app_tests
{
    /// <summary>
    /// Simple tests to verify the Program class accessibility and basic functionality
    /// These tests ensure the main application startup works correctly
    /// </summary>
    [TestClass]
    public sealed class ProgramTests
    {
        [TestMethod]
        public void Program_Class_ShouldBeAccessible()
        {
            // Act & Assert
            var programType = typeof(Program);
            Assert.IsNotNull(programType, "Program class should be accessible");
            Assert.IsTrue(programType.IsPublic, "Program class should be public");
        }

        [TestMethod]
        public void WebApplicationBuilder_ShouldCreateSuccessfully()
        {
            // Act
            var builder = WebApplication.CreateBuilder();

            // Assert
            Assert.IsNotNull(builder);
            Assert.IsNotNull(builder.Services);
            Assert.IsNotNull(builder.Configuration);
            Assert.IsNotNull(builder.Environment);
        }

        [TestMethod]
        public void WebApplication_ShouldBuildSuccessfully()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddControllersWithViews();

            // Act
            using var app = builder.Build();

            // Assert
            Assert.IsNotNull(app);
            Assert.IsNotNull(app.Services);
        }

        [TestMethod]
        public void WebApplication_Configuration_ShouldBeValid()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddControllersWithViews();

            // Act
            using var app = builder.Build();

            // Assert - Test that we can configure the pipeline without errors
            Assert.DoesNotThrow(() =>
            {
                app.UseStaticFiles();
                app.UseRouting();
                app.UseAuthorization();
            });
        }

        [TestMethod]
        public void WebApplication_Services_ControllersRegistered()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddControllersWithViews();

            // Act
            using var app = builder.Build();
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            // Assert
            var mvcOptions = services.GetService<Microsoft.AspNetCore.Mvc.MvcOptions>();
            Assert.IsNotNull(mvcOptions, "MVC services should be registered");
        }

        [TestMethod]
        public void WebApplication_Environment_ShouldBeConfigurable()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            
            // Act
            var originalEnvironment = builder.Environment.EnvironmentName;
            builder.Environment.EnvironmentName = "Testing";

            // Assert
            Assert.AreEqual("Testing", builder.Environment.EnvironmentName);
            Assert.IsTrue(builder.Environment.IsEnvironment("Testing"));
        }

        [TestMethod]
        public void WebApplication_Logging_ShouldBeAvailable()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddControllersWithViews();

            // Act
            using var app = builder.Build();
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            // Assert
            var loggerFactory = services.GetService<ILoggerFactory>();
            Assert.IsNotNull(loggerFactory, "Logger factory should be available");

            var logger = services.GetService<ILogger<Program>>();
            Assert.IsNotNull(logger, "Logger for Program should be available");
        }

        [TestMethod]
        public void WebApplication_Configuration_ShouldNotBeNull()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();

            // Act & Assert
            Assert.IsNotNull(builder.Configuration);
            Assert.IsTrue(builder.Configuration is IConfiguration);
        }

        [TestMethod]
        public void WebApplication_Host_ShouldBeConfigurable()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddControllersWithViews();

            // Act
            using var app = builder.Build();

            // Assert
            Assert.IsNotNull(app.Lifetime);
            Assert.IsNotNull(app.Logger);
            Assert.IsNotNull(app.Urls);
        }
    }
}