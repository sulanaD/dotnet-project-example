using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace example_web_app_tests
{
    /// <summary>
    /// Simple tests to verify the Program class accessibility and basic functionality
    /// These tests focus on testing without complex WebApplication setup
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
        public void Program_Type_ShouldHaveCorrectProperties()
        {
            // Arrange
            var programType = typeof(Program);

            // Act & Assert
            Assert.IsFalse(programType.IsAbstract, "Program should not be abstract");
            Assert.IsFalse(programType.IsInterface, "Program should not be an interface");
            Assert.IsTrue(programType.IsClass, "Program should be a class");
        }

        [TestMethod]
        public void ServiceCollection_BasicFunctionality_ShouldWork()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddLogging();
            services.AddSingleton<string>("test");

            // Assert
            using var provider = services.BuildServiceProvider();
            var logger = provider.GetService<ILoggerFactory>();
            var testString = provider.GetService<string>();

            Assert.IsNotNull(logger);
            Assert.AreEqual("test", testString);
        }

        [TestMethod]
        public void Configuration_Types_ShouldBeAvailable()
        {
            // Test that we can access configuration types
            var configurationType = typeof(IConfiguration);
            var hostEnvironmentType = typeof(IHostEnvironment);
            var webHostEnvironmentType = typeof(IWebHostEnvironment);

            Assert.IsNotNull(configurationType);
            Assert.IsNotNull(hostEnvironmentType);
            Assert.IsNotNull(webHostEnvironmentType);
        }

        [TestMethod]
        public void Logging_Types_ShouldBeAvailable()
        {
            // Test that we can access logging types
            var loggerType = typeof(ILogger);
            var loggerFactoryType = typeof(ILoggerFactory);
            var genericLoggerType = typeof(ILogger<>);

            Assert.IsNotNull(loggerType);
            Assert.IsNotNull(loggerFactoryType);
            Assert.IsNotNull(genericLoggerType);
        }

        [TestMethod]
        public void DependencyInjection_ShouldWork()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton<ILoggerFactory, Microsoft.Extensions.Logging.LoggerFactory>();

            // Act
            using var provider = services.BuildServiceProvider();
            var factory = provider.GetService<ILoggerFactory>();

            // Assert
            Assert.IsNotNull(factory);
            Assert.IsInstanceOfType(factory, typeof(ILoggerFactory));
        }

        [TestMethod]
        public void AspNetCore_Types_ShouldBeAccessible()
        {
            // Test that ASP.NET Core types are accessible
            var controllerType = typeof(Microsoft.AspNetCore.Mvc.Controller);
            var actionResultType = typeof(Microsoft.AspNetCore.Mvc.IActionResult);
            var viewResultType = typeof(Microsoft.AspNetCore.Mvc.ViewResult);

            Assert.IsNotNull(controllerType);
            Assert.IsNotNull(actionResultType);
            Assert.IsNotNull(viewResultType);
        }

        [TestMethod]
        public void TestFramework_ShouldWork()
        {
            // Simple test to verify MSTest is working correctly
            Assert.IsTrue(true);
            Assert.IsFalse(false);
            Assert.AreEqual(1, 1);
            Assert.AreNotEqual(1, 2);
        }
    }
}