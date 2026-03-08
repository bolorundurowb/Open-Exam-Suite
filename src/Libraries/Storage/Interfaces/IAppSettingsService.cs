using OpenExamSuite.Storage.Enums;
using OpenExamSuite.Storage.Models;

namespace OpenExamSuite.Storage.Interfaces;

public interface IAppSettingsService
{
    void Add(AppSetting settings, AppSettingsType type);

    void Clear(AppSettingsType type);

    List<AppSetting> GetAll(AppSettingsType type);
}