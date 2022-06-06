using Interfaces.Dtos;
using System.Collections.Generic;

namespace Interfaces
{
    public interface IFileHandler<T> where T : class
    {
        bool AddObject(string jsonFilePath, T anObject);
        bool UpdateObject(string jsonFilePath, T anObject);
        bool DeleteObject(string jsonFilePath, T anObject);
        IEnumerable<T> GetAll(string jsonFilePath);
    }
}
