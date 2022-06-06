using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using Services;
using ServicesTests.Helpers;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using Xunit;

namespace ServicesTests
{
    public class JsonFileHandlerShould
    {
        private readonly Fixture _fixture;
        private readonly IFileSystem _fileSystem;
        private readonly JsonFileHandler<TestObjectClass> _service;

        public JsonFileHandlerShould()
        {
            _fixture = new Fixture();
            _fileSystem = Substitute.For<IFileSystem>();
            _service = new JsonFileHandler<TestObjectClass>(_fileSystem);
        }

        [Fact]
        public void GetAll_ReturnsEmptyListWhenThereIsNoFile()
        {
            //setup
            var path = _fixture.Create<string>();
            _fileSystem.File.Exists(path).Returns(false);
            //execution
            var list = _service.GetAll(path);
            //validation
            list.Should().BeEmpty();
            //cleanup
        }

        [Fact]
        public void GetAll_ReturnsListWhenThereIsAValidJsonFile()
        {
            //setup
            var path = _fixture.Create<string>();
            var testObjects = _fixture.CreateMany<TestObjectClass>();
            _fileSystem.File.Exists(path).Returns(true);
            _fileSystem.File.ReadAllText(path).Returns(JsonConvert.SerializeObject(testObjects));
            //execution
            var list = _service.GetAll(path);
            //validation
            list.Should().BeEquivalentTo(testObjects);
            //cleanup
        }

        [Fact]
        public void AddObject_IsSuccessful()
        {
            //setup
            var path = _fixture.Create<string>();
            _fileSystem.File.Exists(path).Returns(true);
            _fileSystem.File.ReadAllText(path).Returns(JsonConvert.SerializeObject(new List<TestObjectClass>()));
            var objectToAdd = _fixture.Create<TestObjectClass>();
            //execution
            var list = _service.AddObject(path, objectToAdd);
            //validation
            list.Should().BeTrue();
            _fileSystem.File.Received(1).WriteAllText(path, JsonConvert.SerializeObject(new List<TestObjectClass>() { objectToAdd }));
            //cleanup
        }

        [Fact]
        public void AddObject_IsUnsuccessful()
        {
            //setup
            var path = _fixture.Create<string>();
            _fileSystem.File.Exists(path).Returns(true);
            var objectsToAdd = _fixture.CreateMany<TestObjectClass>().ToList();
            _fileSystem.File.ReadAllText(path).Returns(JsonConvert.SerializeObject(objectsToAdd));
            //execution
            var list = _service.AddObject(path, objectsToAdd[0]);
            //validation
            list.Should().BeFalse();
            _fileSystem.File.Received(0).WriteAllText(Arg.Any<string>(), Arg.Any<string>());
            //cleanup
        }

        [Fact]
        public void UpdateObject_IsSuccessful()
        {
            //setup
            var path = _fixture.Create<string>();
            _fileSystem.File.Exists(path).Returns(true);
            var objectsToAdd = _fixture.CreateMany<TestObjectClass>().ToList();
            _fileSystem.File.ReadAllText(path).Returns(JsonConvert.SerializeObject(objectsToAdd));
            var expectedObjects = objectsToAdd;
            var objectToUpdate = expectedObjects[0];
            expectedObjects.Remove(objectToUpdate);
            objectToUpdate.Name = _fixture.Create<string>();
            expectedObjects.Add(objectToUpdate);
            //execution
            var list = _service.UpdateObject(path, objectToUpdate);
            //validation
            list.Should().BeTrue();
            _fileSystem.File.Received(1).WriteAllText(path, JsonConvert.SerializeObject(expectedObjects));
            //cleanup
        }

        [Fact]
        public void UpdateObject_IsUnsuccessful()
        {
            //setup
            var path = _fixture.Create<string>();
            _fileSystem.File.Exists(path).Returns(true);
            _fileSystem.File.ReadAllText(path).Returns(JsonConvert.SerializeObject(new List<TestObjectClass>()));
            var objectToUpdate = _fixture.Create<TestObjectClass>();
            //execution
            var list = _service.UpdateObject(path, objectToUpdate);
            //validation
            list.Should().BeFalse();
            _fileSystem.File.Received(0).WriteAllText(Arg.Any<string>(), Arg.Any<string>());
            //cleanup
        }

        [Fact]
        public void DeleteObject_IsSuccessful()
        {
            //setup
            var path = _fixture.Create<string>();
            _fileSystem.File.Exists(path).Returns(true);
            var objectsToAdd = _fixture.CreateMany<TestObjectClass>().ToList();
            _fileSystem.File.ReadAllText(path).Returns(JsonConvert.SerializeObject(objectsToAdd));
            var expectedObjects = objectsToAdd;
            var objectToDelete = expectedObjects[0];
            expectedObjects.Remove(objectToDelete);
            //execution
            var list = _service.DeleteObject(path, objectToDelete);
            //validation
            list.Should().BeTrue();
            _fileSystem.File.Received(1).WriteAllText(path, JsonConvert.SerializeObject(expectedObjects));
            //cleanup
        }

        [Fact]
        public void DeleteObject_IsUnsuccessful()
        {
            //setup
            var path = _fixture.Create<string>();
            _fileSystem.File.Exists(path).Returns(true);            
            _fileSystem.File.ReadAllText(path).Returns(JsonConvert.SerializeObject(new List<TestObjectClass>()));
            var objectToDelete = _fixture.Create<TestObjectClass>();
            //execution
            var list = _service.DeleteObject(path, objectToDelete);
            //validation
            list.Should().BeFalse();
            _fileSystem.File.Received(0).WriteAllText(Arg.Any<string>(), Arg.Any<string>());
            //cleanup
        }
    }
}
