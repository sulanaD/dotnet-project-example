using example_web_app.Classes;
using example_web_app.Controllers;
using example_web_app.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics;

namespace example_web_app_tests
{
    /// <summary>
    /// Simplified test suite focused on core functionality
    /// Ensures reliable test execution in CI pipeline
    /// </summary>
    [TestClass]
    public sealed class SimplifiedTests
    {
        [TestMethod]
        public void HelloWorld_GetMessage_ReturnsCorrectValue()
        {
            // Arrange
            var helloWorld = new HelloWorld();

            // Act
            var result = helloWorld.GetMessage();

            // Assert
            Assert.AreEqual("Hello, World!", result);
        }

        [TestMethod]
        public void ErrorViewModel_ShowRequestId_WorksCorrectly()
        {
            // Test with null
            var model1 = new ErrorViewModel { RequestId = null };
            Assert.IsFalse(model1.ShowRequestId);

            // Test with empty
            var model2 = new ErrorViewModel { RequestId = "" };
            Assert.IsFalse(model2.ShowRequestId);

            // Test with value
            var model3 = new ErrorViewModel { RequestId = "test-123" };
            Assert.IsTrue(model3.ShowRequestId);

            // Test with whitespace - this should return true per the implementation
            var model4 = new ErrorViewModel { RequestId = "   " };
            Assert.IsTrue(model4.ShowRequestId);
        }

        [TestMethod]
        public void HomeController_BasicActions_ReturnViewResults()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<HomeController>>();
            var controller = new HomeController(mockLogger.Object);

            // Test basic actions that don't need HttpContext
            var indexResult = controller.Index();
            var privacyResult = controller.Privacy();
            var contactResult = controller.ContactUs();

            // Assert
            Assert.IsInstanceOfType(indexResult, typeof(ViewResult));
            Assert.IsInstanceOfType(privacyResult, typeof(ViewResult));
            Assert.IsInstanceOfType(contactResult, typeof(ViewResult));
        }

        [TestMethod]
        public void HomeController_ErrorAction_WithHttpContext_Works()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<HomeController>>();
            var controller = new HomeController(mockLogger.Object);
            
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(x => x.TraceIdentifier).Returns("test-trace-123");
            
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            // Act
            var result = controller.Error();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.Model, typeof(ErrorViewModel));

            var model = viewResult.Model as ErrorViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual("test-trace-123", model.RequestId);
        }

        [TestMethod]
        public void HomeController_Constructor_ValidatesNullLogger()
        {
            // Act & Assert
            var exception = Assert.ThrowsException<ArgumentNullException>(() => new HomeController(null!));
            Assert.AreEqual("logger", exception.ParamName);
        }

        [TestMethod]
        public void HelloWorld_Interface_IsImplementedCorrectly()
        {
            // Arrange & Act
            IHelloWorld helloWorld = new HelloWorld();
            var message = helloWorld.GetMessage();

            // Assert
            Assert.IsNotNull(message);
            Assert.AreEqual("Hello, World!", message);
        }

        [TestMethod]
        public void Program_Class_IsAccessible()
        {
            // Act
            var programType = typeof(Program);

            // Assert
            Assert.IsNotNull(programType);
            Assert.IsTrue(programType.IsPublic);
        }

        [TestMethod]
        public void ErrorViewModel_Properties_WorkCorrectly()
        {
            // Arrange
            var model = new ErrorViewModel();

            // Test default values
            Assert.IsNull(model.RequestId);
            Assert.IsFalse(model.ShowRequestId);

            // Test setting value
            model.RequestId = "test-request-id";
            Assert.AreEqual("test-request-id", model.RequestId);
            Assert.IsTrue(model.ShowRequestId);
        }

        [TestMethod]
        public void HelloWorld_Performance_IsAcceptable()
        {
            // Arrange
            var helloWorld = new HelloWorld();
            var stopwatch = new Stopwatch();

            // Act
            stopwatch.Start();
            for (int i = 0; i < 1000; i++)
            {
                helloWorld.GetMessage();
            }
            stopwatch.Stop();

            // Assert
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 100, 
                $"Performance test failed. Expected < 100ms, actual: {stopwatch.ElapsedMilliseconds}ms");
        }

        [TestMethod]
        public void Dependencies_AreAccessible()
        {
            // Test that we can access all required types
            var controllerType = typeof(Microsoft.AspNetCore.Mvc.Controller);
            var loggerType = typeof(ILogger);
            var helloWorldType = typeof(HelloWorld);

            Assert.IsNotNull(controllerType);
            Assert.IsNotNull(loggerType);
            Assert.IsNotNull(helloWorldType);
        }

        [TestMethod]
        public void HomeController_ResponseCacheAttribute_IsPresent()
        {
            // Arrange
            var controllerType = typeof(HomeController);
            var errorMethod = controllerType.GetMethod("Error");

            // Act
            var attribute = errorMethod?.GetCustomAttributes(typeof(ResponseCacheAttribute), false)
                .FirstOrDefault() as ResponseCacheAttribute;

            // Assert
            Assert.IsNotNull(attribute);
            Assert.AreEqual(0, attribute.Duration);
            Assert.AreEqual(ResponseCacheLocation.None, attribute.Location);
            Assert.IsTrue(attribute.NoStore);
        }
    }
}