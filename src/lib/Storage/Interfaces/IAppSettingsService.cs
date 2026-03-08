using Storage.Enums;
using Storage.Models;

namespace Storage.Interfaces;

public interface IAppSettingsService
{
    void Add(AppSetting settings, AppSettingsType type);

    void Clear(AppSettingsType type);

    List<AppSetting> GetAll(AppSettingsType type);
}