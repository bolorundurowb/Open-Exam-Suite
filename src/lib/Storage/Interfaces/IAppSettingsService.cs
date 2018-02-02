using System.Collections.Generic;
using Storage.Enums;
using Storage.Models;

namespace Storage.Interfaces
{
    public interface IAppSettingsService
    {
        void Add(AppSetting settings, AppSettingsType type);
        void Clear(AppSettingsType type);
        IEnumerable<AppSetting> GetAll(AppSettingsType type);
    }
}