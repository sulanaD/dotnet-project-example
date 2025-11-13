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
    /// Unit tests for the HelloWorld class demonstrating basic functionality testing
    /// </summary>
    [TestClass]
    public sealed class HelloWorldTests
    {
        [TestMethod]
        public void GetMessage_ShouldReturnHelloWorld()
        {
            // Arrange
            var helloWorld = new HelloWorld();

            // Act
            var result = helloWorld.GetMessage();

            // Assert
            Assert.AreEqual("Hello, World!", result);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(string));
        }

        [TestMethod]
        public void GetMessage_ShouldNotReturnEmptyString()
        {
            // Arrange
            var helloWorld = new HelloWorld();

            // Act
            var result = helloWorld.GetMessage();

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(result));
            Assert.IsTrue(result.Length > 0);
        }

        [TestMethod]
        public void GetMessage_ShouldBeConsistent()
        {
            // Arrange
            var helloWorld = new HelloWorld();

            // Act
            var result1 = helloWorld.GetMessage();
            var result2 = helloWorld.GetMessage();

            // Assert
            Assert.AreEqual(result1, result2);
        }
    }

    /// <summary>
    /// Unit tests for the ErrorViewModel class testing model behavior
    /// </summary>
    [TestClass]
    public sealed class ErrorViewModelTests
    {
        [TestMethod]
        public void RequestId_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var viewModel = new ErrorViewModel();
            var testRequestId = "test-request-123";

            // Act
            viewModel.RequestId = testRequestId;

            // Assert
            Assert.AreEqual(testRequestId, viewModel.RequestId);
        }

        [TestMethod]
        public void ShowRequestId_ShouldReturnTrue_WhenRequestIdIsNotEmpty()
        {
            // Arrange
            var viewModel = new ErrorViewModel { RequestId = "test-123" };

            // Act
            var result = viewModel.ShowRequestId;

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ShowRequestId_ShouldReturnFalse_WhenRequestIdIsNull()
        {
            // Arrange
            var viewModel = new ErrorViewModel { RequestId = null };

            // Act
            var result = viewModel.ShowRequestId;

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShowRequestId_ShouldReturnFalse_WhenRequestIdIsEmpty()
        {
            // Arrange
            var viewModel = new ErrorViewModel { RequestId = "" };

            // Act
            var result = viewModel.ShowRequestId;

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShowRequestId_ShouldReturnTrue_WhenRequestIdIsWhitespace()
        {
            // Arrange
            var viewModel = new ErrorViewModel { RequestId = "   " };

            // Act
            var result = viewModel.ShowRequestId;

            // Assert
            Assert.IsTrue(result, "Whitespace is considered a valid RequestId");
        }
    }

    /// <summary>
    /// Unit tests for the HomeController demonstrating controller testing with mocking
    /// </summary>
    [TestClass]
    public sealed class HomeControllerTests
    {
        private Mock<ILogger<HomeController>> _mockLogger = null!;
        private HomeController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();
            _controller = new HomeController(_mockLogger.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _controller?.Dispose();
        }

        [TestMethod]
        public void Index_ShouldReturnViewResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Privacy_ShouldReturnViewResult()
        {
            // Act
            var result = _controller.Privacy();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
        }

        [TestMethod]
        public void ContactUs_ShouldReturnViewResult()
        {
            // Act
            var result = _controller.ContactUs();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
        }

        [TestMethod]
        public void Error_ShouldReturnViewResultWithErrorViewModel()
        {
            // Arrange
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(x => x.TraceIdentifier).Returns("test-trace-id");
            _controller.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var result = _controller.Error();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.Model, typeof(ErrorViewModel));
            
            var model = viewResult.Model as ErrorViewModel;
            Assert.IsNotNull(model.RequestId);
            Assert.IsTrue(model.ShowRequestId);
        }

        [TestMethod]
        public void Error_ShouldUseActivityIdWhenAvailable()
        {
            // Arrange
            using var activity = new Activity("test-activity");
            activity.Start();

            // Act
            var result = _controller.Error();

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as ErrorViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(activity.Id, model.RequestId);
            
            activity.Stop();
        }

        [TestMethod]
        public void Constructor_ShouldThrowArgumentNullException_WhenLoggerIsNull()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => new HomeController(null!));
        }
    }

    /// <summary>
    /// Integration tests demonstrating edge cases and boundary conditions
    /// </summary>
    [TestClass]
    public sealed class IntegrationTests
    {
        [TestMethod]
        public void HelloWorld_ImplementsInterface()
        {
            // Arrange & Act
            IHelloWorld helloWorld = new HelloWorld();

            // Assert
            Assert.IsNotNull(helloWorld);
            Assert.IsInstanceOfType(helloWorld, typeof(IHelloWorld));
        }

        [TestMethod]
        public void ErrorViewModel_DefaultValues()
        {
            // Act
            var model = new ErrorViewModel();

            // Assert
            Assert.IsNull(model.RequestId);
            Assert.IsFalse(model.ShowRequestId);
        }

        [TestMethod]
        [DataRow("test-123")]
        [DataRow("another-test")]
        [DataRow("complex-request-id-with-dashes")]
        public void ErrorViewModel_WithVariousRequestIds(string requestId)
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = requestId };

            // Act & Assert
            Assert.AreEqual(requestId, model.RequestId);
            Assert.IsTrue(model.ShowRequestId);
        }

        [TestMethod]
        public void MultipleHelloWorldInstances_ShouldReturnSameMessage()
        {
            // Arrange
            var instance1 = new HelloWorld();
            var instance2 = new HelloWorld();

            // Act
            var message1 = instance1.GetMessage();
            var message2 = instance2.GetMessage();

            // Assert
            Assert.AreEqual(message1, message2);
            Assert.AreEqual("Hello, World!", message1);
            Assert.AreEqual("Hello, World!", message2);
        }
    }

    /// <summary>
    /// Performance and load tests for CI pipeline validation
    /// </summary>
    [TestClass]
    public sealed class PerformanceTests
    {
        [TestMethod]
        public void HelloWorld_Performance_ShouldCompleteQuickly()
        {
            // Arrange
            var helloWorld = new HelloWorld();
            var stopwatch = new Stopwatch();

            // Act
            stopwatch.Start();
            for (int i = 0; i < 1000; i++)
            {
                var result = helloWorld.GetMessage();
            }
            stopwatch.Stop();

            // Assert
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 100, 
                $"Performance test failed. Expected < 100ms, actual: {stopwatch.ElapsedMilliseconds}ms");
        }

        [TestMethod]
        public void ErrorViewModel_BulkCreation_ShouldPerformWell()
        {
            // Arrange
            var stopwatch = new Stopwatch();
            var models = new List<ErrorViewModel>();

            // Act
            stopwatch.Start();
            for (int i = 0; i < 10000; i++)
            {
                models.Add(new ErrorViewModel { RequestId = $"request-{i}" });
            }
            stopwatch.Stop();

            // Assert
            Assert.AreEqual(10000, models.Count);
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 500,
                $"Bulk creation test failed. Expected < 500ms, actual: {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}
