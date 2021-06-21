using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TaskManager.Core.Enums;
using TaskManager.Core.Interfaces;

namespace TaskManager.Core.UnitTests
{
    public class OperatingSystemTest
    {
        private OperatingSystem _operatingSystem;

        [SetUp]
        public void Setup()
        {
            var settings = new Mock<ISettings>();
            settings.SetupGet(s => s.ProcessesMaximumCapacity).Returns(4);
            
            _operatingSystem = new OperatingSystem(
                settings.Object,
                new Mock<ILogger<IOperatingSystem>>().Object);
        }

        [Test]
        public void List_SortById_Success()
        {
            //Arrange
            AddProcesses();
            
            //Act
            var list = _operatingSystem.List(SortBy.Id);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(4, list.Count);
                Assert.AreEqual(0, list.FirstOrDefault()?.Id);
            });
        }
        
        [Test]
        public void List_SortByPriority_Success()
        {
            //Arrange
            AddProcesses();

            //Act
            var list = _operatingSystem.List(SortBy.Priority);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(4, list.Count);
                Assert.AreEqual(Priority.Low, list.FirstOrDefault()?.Priority);
            });
        }
        
        [Test]
        public void List_SortByCreationTime_Success()
        {
            //Arrange
            AddProcesses();

            //Act
            var list = _operatingSystem.List(SortBy.CreationTime);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(4, list.Count);
                Assert.AreEqual(3, list.FirstOrDefault()?.Id);
            });
        }
        
        [Test]
        public void Add_AddMethodDefault_FailToAdd()
        {
            //Arrange
            AddProcesses();

            //Act
            bool result = _operatingSystem.Add(AddMethod.Default, new Process(4, Priority.Low));

            //Assert
            Assert.Multiple(() =>
            {
                Assert.IsFalse(result);
            });
        }
        
        [Test]
        public void Add_AddMethodFifo_RemovedOldestAndAddedNew()
        {
            //Arrange
            AddProcesses();

            //Act
            bool result = _operatingSystem.Add(AddMethod.Fifo, new Process(4, Priority.Low));
            var list = _operatingSystem.List(SortBy.Id);
            //Assert
            Assert.Multiple(() =>
            {
                Assert.IsTrue(result);
                Assert.IsFalse(list.Any(p => p.Id == 3));
            });
        }
        
        [Test]
        public void Add_AddMethodPriority_RemovedOldestAndAddedNew()
        {
            //Arrange
            AddProcesses();

            //Act
            var result = _operatingSystem.Add(AddMethod.Priority, new Process(4, Priority.High));
            var list = _operatingSystem.List(SortBy.Id);
            //Assert
            Assert.Multiple(() =>
            {
                Assert.IsTrue(result);
                Assert.IsFalse(list.Any(p => p.Id == 0));
            });
        }

        private void AddProcesses()
        {
            _operatingSystem.Add(AddMethod.Default, new Process(3, Priority.High));
            _operatingSystem.Add(AddMethod.Default, new Process(1, Priority.Low));
            _operatingSystem.Add(AddMethod.Default, new Process(2, Priority.Low));
            _operatingSystem.Add(AddMethod.Default, new Process(0, Priority.Medium));
        }
    }
}