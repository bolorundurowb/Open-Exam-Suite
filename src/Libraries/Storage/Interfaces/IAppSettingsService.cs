using OpenExamSuite.Storage.Enums;
using OpenExamSuite.Storage.Models;

namespace OpenExamSuite.Storage.Interfaces;

public interface IAppSettingsService
{
    void Set(AppSetting setting, AppSettingsType type);

    AppSetting? Get(string key, AppSettingsType type);

    void Remove(string key, AppSettingsType type);

    void Clear(AppSettingsType type);

    List<AppSetting> GetAll(AppSettingsType type);
}
