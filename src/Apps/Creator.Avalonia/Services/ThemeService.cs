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

    private bool _watching;

    public ThemeMode Mode { get; private set; } = ThemeMode.System;

    /// <summary>Whether dark is on screen now, including when it comes from the operating system.</summary>
    public bool IsDarkShown => Application.Current?.ActualThemeVariant == ThemeVariant.Dark;

    public event Action? Changed;

    public ThemeService(IAppSettingsService settings)
    {
        _settings = settings;
    }

    public void Load()
    {
        var stored = _settings.Get(Key, AppSettingsType.Other)
            ?? _settings.Get(Key, AppSettingsType.Creator);
        if (stored != null && Enum.TryParse<ThemeMode>(stored.Value, out var mode))
            Set(mode);
        else
            Apply(ThemeMode.System);
        Watch();
    }

    private void Watch()
    {
        if (_watching || Application.Current is not { } app)
            return;

        _watching = true;
        app.ActualThemeVariantChanged += (_, _) => Changed?.Invoke();
    }

    public void Set(ThemeMode mode, bool persist = true)
    {
        Mode = mode;
        Apply(mode);
        if (persist)
        {
            _settings.Set(new AppSetting { Key = Key, Value = mode.ToString() }, AppSettingsType.Other);
            _settings.Remove(Key, AppSettingsType.Creator);
        }
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
