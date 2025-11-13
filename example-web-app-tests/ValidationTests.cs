using example_web_app.Classes;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace example_web_app_tests
{
    /// <summary>
    /// Validation tests to ensure data integrity and business rules
    /// These tests help achieve better code coverage and validate edge cases
    /// </summary>
    [TestClass]
    public sealed class ValidationTests
    {
        [TestMethod]
        public void HelloWorld_MessageLength_ShouldBeReasonable()
        {
            // Arrange
            var helloWorld = new HelloWorld();

            // Act
            var message = helloWorld.GetMessage();

            // Assert
            Assert.IsTrue(message.Length >= 5, "Message should be at least 5 characters long");
            Assert.IsTrue(message.Length <= 100, "Message should not exceed 100 characters");
        }

        [TestMethod]
        public void HelloWorld_Message_ShouldContainExpectedWords()
        {
            // Arrange
            var helloWorld = new HelloWorld();

            // Act
            var message = helloWorld.GetMessage();

            // Assert
            Assert.IsTrue(message.Contains("Hello"), "Message should contain 'Hello'");
            Assert.IsTrue(message.Contains("World"), "Message should contain 'World'");
        }

        [TestMethod]
        public void HelloWorld_Message_ShouldNotContainUnexpectedCharacters()
        {
            // Arrange
            var helloWorld = new HelloWorld();

            // Act
            var message = helloWorld.GetMessage();

            // Assert
            Assert.IsFalse(message.Contains("<"), "Message should not contain HTML tags");
            Assert.IsFalse(message.Contains(">"), "Message should not contain HTML tags");
            Assert.IsFalse(message.Contains("script"), "Message should not contain script tags");
        }

        [TestMethod]
        public void IHelloWorld_Interface_ShouldHaveCorrectMethodSignature()
        {
            // Arrange
            var interfaceType = typeof(IHelloWorld);

            // Act
            var methods = interfaceType.GetMethods();
            var getMessageMethod = methods.FirstOrDefault(m => m.Name == "GetMessage");

            // Assert
            Assert.IsNotNull(getMessageMethod, "Interface should have GetMessage method");
            Assert.AreEqual(typeof(string), getMessageMethod.ReturnType, "GetMessage should return string");
            Assert.AreEqual(0, getMessageMethod.GetParameters().Length, "GetMessage should have no parameters");
        }

        [TestMethod]
        public void HelloWorld_Class_ShouldImplementInterface()
        {
            // Arrange
            var helloWorldType = typeof(HelloWorld);
            var interfaceType = typeof(IHelloWorld);

            // Act
            var implementsInterface = interfaceType.IsAssignableFrom(helloWorldType);

            // Assert
            Assert.IsTrue(implementsInterface, "HelloWorld should implement IHelloWorld interface");
        }

        [TestMethod]
        public void HelloWorld_Class_ShouldBePublic()
        {
            // Arrange
            var helloWorldType = typeof(HelloWorld);

            // Act & Assert
            Assert.IsTrue(helloWorldType.IsPublic, "HelloWorld class should be public");
            Assert.IsFalse(helloWorldType.IsAbstract, "HelloWorld class should not be abstract");
            Assert.IsFalse(helloWorldType.IsSealed, "HelloWorld class should not be sealed");
        }

        [TestMethod]
        public void HelloWorld_GetMessage_ShouldNotThrowException()
        {
            // Arrange
            var helloWorld = new HelloWorld();

            // Act & Assert
            try
            {
                var result = helloWorld.GetMessage();
                Assert.IsNotNull(result, "GetMessage should return a value");
            }
            catch (Exception ex)
            {
                Assert.Fail($"GetMessage should not throw: {ex.Message}");
            }
        }

        [TestMethod]
        public void HelloWorld_Assembly_ShouldHaveCorrectTypes()
        {
            // Arrange
            var assembly = typeof(HelloWorld).Assembly;

            // Act
            var types = assembly.GetTypes().Where(t => t.Namespace == "example_web_app.Classes").ToList();

            // Assert
            Assert.IsTrue(types.Any(t => t.Name == "HelloWorld"), "Assembly should contain HelloWorld class");
            Assert.IsTrue(types.Any(t => t.Name == "IHelloWorld"), "Assembly should contain IHelloWorld interface");
        }

        [TestMethod]
        public void Namespace_ShouldFollowConvention()
        {
            // Arrange
            var helloWorldType = typeof(HelloWorld);
            var interfaceType = typeof(IHelloWorld);

            // Act & Assert
            Assert.AreEqual("example_web_app.Classes", helloWorldType.Namespace);
            Assert.AreEqual("example_web_app.Classes", interfaceType.Namespace);
        }
    }

    /// <summary>
    /// Security-focused tests to ensure the application handles edge cases safely
    /// These help improve code coverage and test security aspects
    /// </summary>
    [TestClass]
    public sealed class SecurityTests
    {
        [TestMethod]
        public void HelloWorld_Message_ShouldBeSafeForDisplay()
        {
            // Arrange
            var helloWorld = new HelloWorld();

            // Act
            var message = helloWorld.GetMessage();

            // Assert
            Assert.IsFalse(message.Contains("'"), "Message should not contain single quotes that could cause XSS");
            Assert.IsFalse(message.Contains("\""), "Message should not contain double quotes that could cause XSS");
            Assert.IsFalse(message.Contains("javascript:"), "Message should not contain javascript protocol");
            Assert.IsFalse(message.Contains("vbscript:"), "Message should not contain vbscript protocol");
        }

        [TestMethod]
        public void HelloWorld_Message_ShouldNotLeakInformation()
        {
            // Arrange
            var helloWorld = new HelloWorld();

            // Act
            var message = helloWorld.GetMessage();

            // Assert
            Assert.IsFalse(message.ToLower().Contains("password"), "Message should not contain sensitive keywords");
            Assert.IsFalse(message.ToLower().Contains("secret"), "Message should not contain sensitive keywords");
            Assert.IsFalse(message.ToLower().Contains("token"), "Message should not contain sensitive keywords");
            Assert.IsFalse(message.ToLower().Contains("key"), "Message should not contain sensitive keywords");
        }

        [TestMethod]
        public void HelloWorld_Implementation_ShouldBeThreadSafe()
        {
            // Arrange
            var helloWorld = new HelloWorld();
            var tasks = new List<Task<string>>();
            const int numberOfTasks = 100;

            // Act
            for (int i = 0; i < numberOfTasks; i++)
            {
                tasks.Add(Task.Run(() => helloWorld.GetMessage()));
            }
            var results = Task.WhenAll(tasks).Result;

            // Assert
            Assert.AreEqual(numberOfTasks, results.Length);
            Assert.IsTrue(results.All(r => r == "Hello, World!"), "All results should be identical and correct");
        }

        [TestMethod]
        public void HelloWorld_MultipleInstances_ShouldNotInterfere()
        {
            // Arrange
            var instances = Enumerable.Range(0, 10).Select(_ => new HelloWorld()).ToList();

            // Act
            var results = instances.Select(instance => instance.GetMessage()).ToList();

            // Assert
            Assert.AreEqual(10, results.Count);
            Assert.IsTrue(results.All(r => r == "Hello, World!"), "All instances should return the same message");
        }

        [TestMethod]
        public void HelloWorld_MemoryUsage_ShouldBeMinimal()
        {
            // Arrange
            var initialMemory = GC.GetTotalMemory(true);

            // Act
            var instances = new List<HelloWorld>();
            for (int i = 0; i < 1000; i++)
            {
                instances.Add(new HelloWorld());
                _ = instances[i].GetMessage();
            }

            var finalMemory = GC.GetTotalMemory(false);
            var memoryUsed = finalMemory - initialMemory;

            // Assert
            Assert.IsTrue(memoryUsed < 1024 * 1024, // Less than 1MB
                $"Memory usage should be minimal. Used: {memoryUsed} bytes");
        }
    }
}