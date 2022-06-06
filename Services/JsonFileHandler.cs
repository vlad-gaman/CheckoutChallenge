using Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;

namespace Services
{
    public class JsonFileHandler<T> : IFileHandler<T> where T : class
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly IFileSystem _fileSystem;

        public JsonFileHandler(IFileSystem fileSystem)
        {
            _semaphore = new SemaphoreSlim(1);
            _fileSystem = fileSystem;
        }

        public bool AddObject(string jsonFilePath, T anObject)
        {
            bool successful = true;
            _semaphore.Wait();

            var list = GetAllWithNoSemaphore(jsonFilePath).ToList();
            if (list.Contains(anObject))
            {
                successful = false;
            }
            else
            {
                list.Add(anObject);
                var jsonList = JsonConvert.SerializeObject(list);
                _fileSystem.File.WriteAllText(jsonFilePath, jsonList);
            }
            _semaphore.Release();
            return successful;
        }

        public bool UpdateObject(string jsonFilePath, T anObject)
        {
            bool successful = true;
            _semaphore.Wait();

            var list = GetAllWithNoSemaphore(jsonFilePath).ToList();
            if (list.Contains(anObject))
            {
                list.Remove(anObject);
                list.Add(anObject);
                var jsonList = JsonConvert.SerializeObject(list);
                _fileSystem.File.WriteAllText(jsonFilePath, jsonList);
            }
            else
            {
                successful = false;
            }

            _semaphore.Release();
            return successful;
        }

        public bool DeleteObject(string jsonFilePath, T anObject)
        {
            bool successful = true;
            _semaphore.Wait();

            var list = GetAllWithNoSemaphore(jsonFilePath).ToList();
            if (list.Contains(anObject))
            {
                list.Remove(anObject);
                var jsonList = JsonConvert.SerializeObject(list);
                _fileSystem.File.WriteAllText(jsonFilePath, jsonList);
            }
            else
            {
                successful = false;
            }

            _semaphore.Release();
            return successful;
        }

        public IEnumerable<T> GetAll(string jsonFilePath)
        {
            _semaphore.Wait();
            var list = GetAllWithNoSemaphore(jsonFilePath);
            _semaphore.Release();
            return list;
        }

        private IEnumerable<T> GetAllWithNoSemaphore(string jsonFilePath)
        {
            if (_fileSystem.File.Exists(jsonFilePath))
            {
                var json = _fileSystem.File.ReadAllText(jsonFilePath);
                return JsonConvert.DeserializeObject<List<T>>(json);
            }
            else
            {
                return new List<T>();
            }
        }
    }
}
