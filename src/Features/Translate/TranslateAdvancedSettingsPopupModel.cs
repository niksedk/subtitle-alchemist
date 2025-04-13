using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Globalization;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.AutoTranslate;
using Nikse.SubtitleEdit.Core.Settings;

namespace SubtitleAlchemist.Features.Translate;

public partial class TranslateAdvancedSettingsPopupModel : ObservableObject
{
    public Picker PickerLineMerge { get; set; } = new();
    public TranslateAdvancedSettingsPopup? Popup { get; set; }
    public Entry DelayEntry { get; set; } = new();
    public Entry MaximumBytesEntry { get; set; } = new();
    public Label PromptLabel { get; set; } = new();
    public Entry PromptEntry { get; set; } = new();

    public IAutoTranslator? AutoTranslator;

    [ObservableProperty]
    public partial ObservableCollection<string> LineMergeItems { get; set; } = new();

    [ObservableProperty]
    public partial string? LineMergeSelectedItem { get; set; }

    [ObservableProperty]
    public partial string MaximumBytes { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string DelayInSeconds { get; set; } = string.Empty;

    public TranslateAdvancedSettingsPopupModel()
    {
        LineMergeItems.Clear();
        LineMergeItems.Add("Default");
        LineMergeItems.Add("Translate each line separately");
        LineMergeSelectedItem = LineMergeItems[0];

        MaximumBytes = Configuration.Settings.Tools.AutoTranslateMaxBytes.ToString(CultureInfo.InvariantCulture);
        DelayInSeconds = Configuration.Settings.Tools.AutoTranslateDelaySeconds.ToString(CultureInfo.InvariantCulture);
    }

    public void LineMergeSelectedIndexChanged(object? sender, EventArgs e)
    {
        if (PickerLineMerge.SelectedItem == null)
        {
            return;
        }

        LineMergeSelectedItem = PickerLineMerge.SelectedItem.ToString();
    }

    [RelayCommand]
    private void Ok()
    {
        if (AutoTranslator == null)
        {
            return;
        }

        var error = string.Empty;
        if (PromptEntry.IsVisible)
        {
            if (PromptEntry.Text.Replace("{0}", string.Empty).Replace("{1}", string.Empty).Contains('{'))
            {
                error = string.Format("{0} requires an API key.", AutoTranslator.Name);
                //TODO: show error
                return;
            }

            if (PromptEntry.Text.Replace("{0}", string.Empty).Replace("{1}", string.Empty).Contains('}'))
            {
                error = "Character not allowed in prompt: '}' (besides '{0}' and '{1}')";
                return;
            }

            if (PromptEntry.Text.Length > 1000)
            {
                error = "Too many characters in prompt";
                return;
            }
        }

        var engineType = AutoTranslator.GetType();
        if (engineType == typeof(ChatGptTranslate))
        {
            Configuration.Settings.Tools.ChatGptPrompt = PromptEntry.Text;
        }
        else if (engineType == typeof(OllamaTranslate))
        {
            Configuration.Settings.Tools.OllamaPrompt = PromptEntry.Text;
        }
        else if (engineType == typeof(LmStudioTranslate))
        {
            Configuration.Settings.Tools.LmStudioPrompt = PromptEntry.Text;
        }
        else if (engineType == typeof(AnthropicTranslate))
        {
            Configuration.Settings.Tools.AnthropicPrompt = PromptEntry.Text;
        }
        else if (engineType == typeof(GroqTranslate))
        {
            Configuration.Settings.Tools.GroqPrompt = PromptEntry.Text;
        }
        else if (engineType == typeof(OpenRouterTranslate))
        {
            Configuration.Settings.Tools.OpenRouterPrompt = PromptEntry.Text;
        }

        if (LineMergeSelectedItem != null && LineMergeItems.IndexOf(LineMergeSelectedItem) == 1)
        {
            Configuration.Settings.Tools.AutoTranslateStrategy = TranslateStrategy.TranslateEachLineSeparately.ToString();
        }
        else
        {
            Configuration.Settings.Tools.AutoTranslateStrategy = TranslateStrategy.Default.ToString();
        }

        if (int.TryParse(MaximumBytesEntry.Text, out var maxBytesInt))
        {
            Configuration.Settings.Tools.AutoTranslateMaxBytes = maxBytesInt;
        }

        if (int.TryParse(DelayEntry.Text, out var delayInSecondsInt))
        {
            Configuration.Settings.Tools.AutoTranslateDelaySeconds = delayInSecondsInt;
        }

        Popup?.Close("OK");
    }

    [RelayCommand]
    private void Cancel()
    {
        Popup?.Close();
    }

    public void SetValues()
    {
        Popup?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LineMergeSelectedItem = LineMergeItems[0];
                PickerLineMerge.SelectedItem = LineMergeSelectedItem;

                MaximumBytes = Configuration.Settings.Tools.AutoTranslateMaxBytes.ToString(CultureInfo.InvariantCulture);
                MaximumBytesEntry.Text = MaximumBytes;

                DelayInSeconds = Configuration.Settings.Tools.AutoTranslateDelaySeconds.ToString(CultureInfo.InvariantCulture);
                DelayEntry.Text = DelayInSeconds;

                if (AutoTranslator == null)
                {
                    return;
                }


                var engineType = AutoTranslator.GetType();
                if (engineType == typeof(ChatGptTranslate))
                {
                    PromptEntry.Text = Configuration.Settings.Tools.ChatGptPrompt;
                    if (string.IsNullOrWhiteSpace(PromptEntry.Text))
                    {
                        PromptEntry.Text = new ToolsSettings().ChatGptPrompt;
                    }
                }
                else if (engineType == typeof(OllamaTranslate))
                {
                    PromptEntry.Text = Configuration.Settings.Tools.OllamaPrompt;
                    if (string.IsNullOrWhiteSpace(PromptEntry.Text))
                    {
                        PromptEntry.Text = new ToolsSettings().OllamaPrompt;
                    }
                }
                else if (engineType == typeof(LmStudioTranslate))
                {
                    PromptEntry.Text = Configuration.Settings.Tools.LmStudioPrompt;
                    if (string.IsNullOrWhiteSpace(PromptEntry.Text))
                    {
                        PromptEntry.Text = new ToolsSettings().LmStudioPrompt;
                    }
                }
                else if (engineType == typeof(AnthropicTranslate))
                {
                    PromptEntry.Text = Configuration.Settings.Tools.AnthropicPrompt;
                    if (string.IsNullOrWhiteSpace(PromptEntry.Text))
                    {
                        PromptEntry.Text = new ToolsSettings().AnthropicPrompt;
                    }
                }
                else if (engineType == typeof(GroqTranslate))
                {
                    PromptEntry.Text = Configuration.Settings.Tools.GroqPrompt;
                    if (string.IsNullOrWhiteSpace(PromptEntry.Text))
                    {
                        PromptEntry.Text = new ToolsSettings().GroqPrompt;
                    }
                }
                else if (engineType == typeof(OpenRouterTranslate))
                {
                    PromptEntry.Text = Configuration.Settings.Tools.OpenRouterPrompt;
                    if (string.IsNullOrWhiteSpace(PromptEntry.Text))
                    {
                        PromptEntry.Text = new ToolsSettings().OpenRouterPrompt;
                    }
                }
                else
                {
                    PromptLabel.IsVisible = false;
                    PromptEntry.IsVisible = false;
                }
            });
            return false;
        });
    }
}
