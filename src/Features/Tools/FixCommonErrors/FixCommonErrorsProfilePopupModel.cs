﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace SubtitleAlchemist.Features.Tools.FixCommonErrors;

public partial class FixCommonErrorsProfilePopupModel : ObservableObject
{
    public FixCommonErrorsProfilePopup? Popup { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<string> ProfileNames { get; set; } = new();

    [ObservableProperty]
    public partial string? SelectedProfileName { get; set; }

    [ObservableProperty]
    public partial string EditProfileName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Status { get; set; } = string.Empty;

    [RelayCommand]
    private void Add()
    {
        EditProfileName = EditProfileName.Trim();
        if (string.IsNullOrWhiteSpace(EditProfileName) || ProfileNames.Contains(EditProfileName.Trim()))
        {
            return;
        }

        ProfileNames.Add(EditProfileName);
        SelectedProfileName = EditProfileName;
        EditProfileName = string.Empty;
        UpdateStatusText();
    }

    [RelayCommand]
    private void Remove()
    {
        if (SelectedProfileName == null)
        {
            return;
        }

        ProfileNames.Remove(SelectedProfileName);
        if (ProfileNames.Count == 0 ) 
        {
            ProfileNames.Add("Default");
        }

        SelectedProfileName = ProfileNames[0];
        UpdateStatusText();
    }

    [RelayCommand]
    private void Ok()
    {
        Popup?.Close(ProfileNames.ToList());
    }

    [RelayCommand]
    private void Cancel()
    {
        Popup?.Close();
    }

    public void SetValues(List<string> profileNames)
    {
        Popup?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ProfileNames = new ObservableCollection<string>(profileNames);

                if (ProfileNames.Count == 0)
                {
                    ProfileNames.Add("Default");
                }

                SelectedProfileName = ProfileNames[0];
                UpdateStatusText();
            });
            return false;
        });
    }

    private void UpdateStatusText()
    {
        Status = $"Number of profiles: {ProfileNames.Count}";
    }
}
