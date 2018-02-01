using System.Collections.Generic;

namespace Storage.Interfaces
{
    public interface ISettings
    {
        void Save();
        void Clear();
        List<ISettings> GetAll();
    }
}