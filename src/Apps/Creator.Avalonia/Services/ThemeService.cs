using Avalonia;
using Avalonia.Styling;
using OpenExamSuite.Storage.Enums;
using OpenExamSuite.Storage.Interfaces;
using OpenExamSuite.Storage.Models;

namespace OpenExamSuite.Creator.Services;

public enum ThemeMode
{
    System,
    Light,
    Dark
}

public sealed class ThemeService
{
    private const string Key = "CreatorTheme";
    private readonly IAppSettingsService _settings;

    public ThemeMode Mode { get; private set; } = ThemeMode.System;

    public event Action? Changed;

    public ThemeService(IAppSettingsService settings)
    {
        _settings = settings;
    }

    public void Load()
    {
        var stored = _settings.Get(Key, AppSettingsType.Creator);
        if (stored != null && Enum.TryParse<ThemeMode>(stored.Value, out var mode))
            Set(mode, false);
        else
            Apply(ThemeMode.System);
    }

    public void Toggle()
    {
        var next = Mode switch
        {
            ThemeMode.System => ThemeMode.Light,
            ThemeMode.Light => ThemeMode.Dark,
            ThemeMode.Dark => ThemeMode.System,
            _ => ThemeMode.System
        };
        Set(next);
    }

    public void Set(ThemeMode mode, bool persist = true)
    {
        Mode = mode;
        Apply(mode);
        if (persist)
            _settings.Set(new AppSetting { Key = Key, Value = mode.ToString() }, AppSettingsType.Creator);
        Changed?.Invoke();
    }

    private static void Apply(ThemeMode mode)
    {
        if (Application.Current == null)
            return;

        Application.Current.RequestedThemeVariant = mode switch
        {
            ThemeMode.Light => ThemeVariant.Light,
            ThemeMode.Dark => ThemeVariant.Dark,
            _ => ThemeVariant.Default
        };
    }
}
