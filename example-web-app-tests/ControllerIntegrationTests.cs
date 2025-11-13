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
    /// Comprehensive controller tests with proper mocking and validation
    /// These tests ensure controllers behave correctly under various conditions
    /// </summary>
    [TestClass]
    public sealed class ControllerIntegrationTests
    {
        private Mock<ILogger<HomeController>> _mockLogger = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();
        }

        [TestMethod]
        public void HomeController_Index_ReturnsCorrectViewResult()
        {
            // Arrange
            var controller = new HomeController(_mockLogger.Object);

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsNull(viewResult.ViewName); // Default view name
            Assert.IsNull(viewResult.Model); // No model expected
        }

        [TestMethod]
        public void HomeController_Privacy_ReturnsCorrectViewResult()
        {
            // Arrange
            var controller = new HomeController(_mockLogger.Object);

            // Act
            var result = controller.Privacy();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsNull(viewResult.ViewName); // Default view name
        }

        [TestMethod]
        public void HomeController_ContactUs_ReturnsCorrectViewResult()
        {
            // Arrange
            var controller = new HomeController(_mockLogger.Object);

            // Act
            var result = controller.ContactUs();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsNull(viewResult.ViewName); // Default view name
        }

        [TestMethod]
        public void HomeController_Error_WithActivity_ReturnsViewWithModel()
        {
            // Arrange
            var controller = new HomeController(_mockLogger.Object);
            
            // Create an activity to test the Activity.Current path
            using var activity = new Activity("TestActivity");
            activity.Start();

            // Act
            var result = controller.Error();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.Model, typeof(ErrorViewModel));
            
            var model = viewResult.Model as ErrorViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(activity.Id, model.RequestId);
            Assert.IsTrue(model.ShowRequestId);

            activity.Stop();
        }

        [TestMethod]
        public void HomeController_Error_WithHttpContext_ReturnsViewWithTraceId()
        {
            // Arrange
            var controller = new HomeController(_mockLogger.Object);
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(x => x.TraceIdentifier).Returns("trace-123");
            
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            // Stop any current activity to ensure we use HttpContext.TraceIdentifier
            Activity.Current?.Stop();

            // Act
            var result = controller.Error();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.Model, typeof(ErrorViewModel));
            
            var model = viewResult.Model as ErrorViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual("trace-123", model.RequestId);
            Assert.IsTrue(model.ShowRequestId);
        }

        [TestMethod]
        public void HomeController_Error_ResponseCacheAttribute_IsCorrect()
        {
            // Arrange
            var controllerType = typeof(HomeController);
            var errorMethod = controllerType.GetMethod("Error");

            // Act
            var responseCacheAttribute = errorMethod?.GetCustomAttributes(typeof(ResponseCacheAttribute), false)
                .FirstOrDefault() as ResponseCacheAttribute;

            // Assert
            Assert.IsNotNull(responseCacheAttribute, "Error method should have ResponseCache attribute");
            Assert.AreEqual(0, responseCacheAttribute.Duration);
            Assert.AreEqual(ResponseCacheLocation.None, responseCacheAttribute.Location);
            Assert.IsTrue(responseCacheAttribute.NoStore);
        }

        [TestMethod]
        public void HomeController_AllActions_ReturnViewResults()
        {
            // Arrange
            var controller = new HomeController(_mockLogger.Object);
            
            // Setup HttpContext for Error action
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(x => x.TraceIdentifier).Returns("test-trace");
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            var actions = new Func<IActionResult>[]
            {
                () => controller.Index(),
                () => controller.Privacy(),
                () => controller.ContactUs(),
                () => controller.Error()
            };

            // Act & Assert
            foreach (var action in actions)
            {
                var result = action();
                Assert.IsInstanceOfType(result, typeof(ViewResult), 
                    $"Action {action.Method.Name} should return ViewResult");
            }
        }

        [TestMethod]
        public void HomeController_Constructor_ValidatesLogger()
        {
            // Act & Assert
            var exception = Assert.ThrowsException<ArgumentNullException>(() => new HomeController(null!));
            Assert.AreEqual("logger", exception.ParamName);
        }

        [TestMethod]
        public void HomeController_Logger_IsUsedCorrectly()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<HomeController>>();
            var controller = new HomeController(mockLogger.Object);

            // Act
            controller.Index();
            controller.Privacy();
            controller.ContactUs();

            // Assert - Logger should be available but not necessarily called for these simple actions
            Assert.IsNotNull(controller);
            mockLogger.VerifyNoOtherCalls(); // These actions don't log anything
        }

        [TestMethod]
        public void HomeController_Dispose_ShouldNotThrow()
        {
            // Arrange
            var controller = new HomeController(_mockLogger.Object);

            // Act & Assert
            try
            {
                controller.Dispose();
                Assert.IsTrue(true, "Dispose should not throw");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Dispose should not throw: {ex.Message}");
            }
        }

        [TestMethod]
        public void HomeController_MultipleInstances_ShouldWork()
        {
            // Arrange & Act
            var controllers = Enumerable.Range(0, 5)
                .Select(_ => new HomeController(_mockLogger.Object))
                .ToList();

            // Assert
            Assert.AreEqual(5, controllers.Count);
            foreach (var controller in controllers)
            {
                var result = controller.Index();
                Assert.IsInstanceOfType(result, typeof(ViewResult));
                controller.Dispose();
            }
        }

        [TestMethod]
        public void HomeController_ConcurrentAccess_ShouldBeThreadSafe()
        {
            // Arrange
            var controller = new HomeController(_mockLogger.Object);
            var tasks = new List<Task<IActionResult>>();

            // Act
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(() => controller.Index()));
                tasks.Add(Task.Run(() => controller.Privacy()));
                tasks.Add(Task.Run(() => controller.ContactUs()));
            }

            var results = Task.WhenAll(tasks).Result;

            // Assert
            Assert.AreEqual(30, results.Length);
            foreach (var result in results)
            {
                Assert.IsInstanceOfType(result, typeof(ViewResult));
            }

            controller.Dispose();
        }
    }
}