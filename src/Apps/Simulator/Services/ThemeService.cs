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

    private bool _watching;

    public ThemeMode Mode { get; private set; } = ThemeMode.System;

    /// <summary>Whether dark is on screen now, including when it comes from the operating system.</summary>
    public bool IsDarkShown => Application.Current?.ActualThemeVariant == ThemeVariant.Dark;

    public event Action? Changed;

    public void Load()
    {
        var stored = _settings.Get(SettingKey, AppSettingsType.Other)?.Value;
        Mode = Enum.TryParse<ThemeMode>(stored, ignoreCase: true, out var parsed) ? parsed : ThemeMode.System;
        Apply();
        Watch();
    }

    private void Watch()
    {
        if (_watching || Application.Current is not { } app)
            return;

        _watching = true;
        app.ActualThemeVariantChanged += (_, _) => Changed?.Invoke();
    }

    public void Set(ThemeMode mode)
    {
        Mode = mode;
        _settings.Set(new AppSetting { Key = SettingKey, Value = mode.ToString() }, AppSettingsType.Other);
        Apply();
        Changed?.Invoke();
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
