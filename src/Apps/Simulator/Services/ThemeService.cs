using Avalonia;
using Avalonia.Styling;
using OpenExamSuite.Storage.Enums;
using OpenExamSuite.Storage.Interfaces;
using OpenExamSuite.Storage.Models;

namespace OpenExamSuite.Simulator.Services;

public enum ThemeMode
{
    System,
    Light,
    Dark
}

/// <summary>
/// Light and dark follow the operating system by default, with a manual override that is remembered.
/// </summary>
public sealed class ThemeService
{
    public const string SettingKey = "Simulator.Theme";

    private readonly IAppSettingsService _settings;

    public ThemeService(IAppSettingsService settings)
    {
        _settings = settings;
    }

    public ThemeMode Mode { get; private set; } = ThemeMode.System;

    public event Action? Changed;

    public void Load()
    {
        var stored = _settings.Get(SettingKey, AppSettingsType.Other)?.Value;
        Mode = Enum.TryParse<ThemeMode>(stored, ignoreCase: true, out var parsed) ? parsed : ThemeMode.System;
        Apply();
    }

    public void Set(ThemeMode mode)
    {
        Mode = mode;
        _settings.Set(new AppSetting { Key = SettingKey, Value = mode.ToString() }, AppSettingsType.Other);
        Apply();
        Changed?.Invoke();
    }

    /// <summary>Flips between light and dark based on what is currently shown.</summary>
    public void Toggle()
    {
        var showingDark = Application.Current?.ActualThemeVariant == ThemeVariant.Dark;
        Set(showingDark ? ThemeMode.Light : ThemeMode.Dark);
    }

    private void Apply()
    {
        if (Application.Current is not { } app)
            return;

        app.RequestedThemeVariant = Mode switch
        {
            ThemeMode.Light => ThemeVariant.Light,
            ThemeMode.Dark => ThemeVariant.Dark,
            _ => ThemeVariant.Default
        };
    }
}
