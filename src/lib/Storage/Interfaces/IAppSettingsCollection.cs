using System.Collections.Generic;
using Storage.Models;

namespace Storage.Interfaces
{
    public interface IAppSettingsCollection
    {
        void Add(AppSetting settings);
        void Clear();
        IEnumerable<AppSetting> GetAll();
    }
}