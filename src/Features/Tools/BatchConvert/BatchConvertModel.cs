using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.SubtitleFormats;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Logic.Media;

namespace SubtitleAlchemist.Features.Tools.BatchConvert;

public partial class BatchConvertModel : ObservableObject, IQueryAttributable
{
    public BatchConvertPage? Page { get; set; }

    [ObservableProperty] private string _outputFolder;
    [ObservableProperty] private bool _saveInSourceFolder;
    [ObservableProperty] private bool _useSourceFolderVisible;
    [ObservableProperty] private bool _useOutputFolderVisible;
    [ObservableProperty] private bool _overwrite;
    [ObservableProperty] private string _targetFormatName;
    [ObservableProperty] private string _targetEncoding;
    [ObservableProperty] private ObservableCollection<BatchConvertItem> _batchItems;
    [ObservableProperty] private BatchConvertItem? _selectedBatchItem;

    [ObservableProperty] private ObservableCollection<string> _targetFormats;
    [ObservableProperty] private string? _selectedTargetFormat;
    [ObservableProperty] private ObservableCollection<string> _targetEncodings;
    [ObservableProperty] private string? _selectedTargetEncoding;

    [ObservableProperty] private ObservableCollection<BatchConvertFunction> _batchFunctions;
    [ObservableProperty] private BatchConvertFunction? _selectedBatchFunction;

    [ObservableProperty] private bool _isProgressVisible;
    [ObservableProperty] private string _progressText;
    [ObservableProperty] private double _progress;

    // Remove formatting
    [ObservableProperty] private bool _formattingRemoveAll;
    [ObservableProperty] private bool _formattingRemoveItalic;
    [ObservableProperty] private bool _formattingRemoveBold;
    [ObservableProperty] private bool _formattingRemoveUnderline;
    [ObservableProperty] private bool _formattingRemoveFontTags;
    [ObservableProperty] private bool _formattingRemoveAlignmentTags;
    [ObservableProperty] private bool _formattingRemoveColors;

    //Offset time codes
    [ObservableProperty] private bool _offsetTimeCodesForward;
    [ObservableProperty] private bool _offsetTimeCodesBack;
    [ObservableProperty] private TimeSpan _offsetTimeCodesTime;


    public View ViewRemoveFormatting { get; set; }
    public View ViewOffsetTimeCodes { get; set; }

    public Label LabelStatusText { get; set; }
    [ObservableProperty] private string _statusText;

    private readonly IFileHelper _fileHelper;
    private readonly IPopupService _popupService;
    private readonly IBatchConverter _batchConverter;

    private bool _stopping;

    public BatchConvertModel(IFileHelper fileHelper, IPopupService popupService, IBatchConverter batchConverter)
    {
        _fileHelper = fileHelper;
        _popupService = popupService;
        _batchConverter = batchConverter;
        _outputFolder = string.Empty;
        _saveInSourceFolder = true;
        _targetFormatName = SubRip.NameOfFormat;
        _targetEncoding = TextEncoding.Utf8WithBom;

        _batchItems = new ObservableCollection<BatchConvertItem>();
        _targetFormats = new ObservableCollection<string>(SubtitleFormat.AllSubtitleFormats.Select(p => p.Name));
        _targetEncodings = new ObservableCollection<string>(EncodingHelper.GetEncodings().Select(p => p.DisplayName).ToList());

        _batchFunctions = new ObservableCollection<BatchConvertFunction>();
        ViewRemoveFormatting = new BoxView();
        ViewOffsetTimeCodes = new BoxView();

        LabelStatusText = new Label();
        _statusText = string.Empty;
        _progressText = string.Empty;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        Page?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var activeFunctions = Se.Settings.Tools.BatchConvert.ActiveFunctions;
                BatchFunctions = new ObservableCollection<BatchConvertFunction>
                {
                    MakeFunction(BatchConvertFunctionType.RemoveFormatting, "Remove formatting", ViewRemoveFormatting, activeFunctions),
                    MakeFunction(BatchConvertFunctionType.OffsetTimeCodes, "Offset time codes", ViewOffsetTimeCodes, activeFunctions),
                };

                LoadSettings();
            });
            return false;
        });
    }

    private static BatchConvertFunction MakeFunction(BatchConvertFunctionType functionType, string name, View view, string[] activeFunctions)
    {
        var isActive = activeFunctions.Contains(functionType.ToString());
        return new BatchConvertFunction(functionType, name, isActive, view);
    }

    [RelayCommand]
    private async Task FileAdd()
    {
        var fileNames = await _fileHelper.PickAndShowSubtitleFiles("Pick subtitle files");
        if (fileNames.Length == 0)
        {
            return;
        }

        foreach (var fileName in fileNames)
        {
            var fileInfo = new FileInfo(fileName);
            var subtitle = Subtitle.Parse(fileName);
            var batchItem = new BatchConvertItem(fileName, fileInfo.Length, subtitle != null ? subtitle.OriginalFormat.Name : "Unknown", subtitle);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                BatchItems.Add(batchItem);
            });
        }
    }

    [RelayCommand]
    private void FileRemove()
    {
        if (SelectedBatchItem != null)
        {
            BatchItems.Remove(SelectedBatchItem);
        }
    }

    [RelayCommand]
    private void FileClear()
    {
        BatchItems.Clear();
    }

    [RelayCommand]
    private async Task BatchOutputProperties()
    {
        var input = new BatchConvertOutputProperties
        {
            UseOutputFolder = UseOutputFolderVisible,
            OutputFolder = OutputFolder,
            Overwrite = Overwrite,
        };

        var result = await _popupService.ShowPopupAsync<BatchConvertOutputPropertiesPopupModel>(onPresenting: viewModel => viewModel.Initialize(input), CancellationToken.None);

        if (result is BatchConvertOutputProperties outputResult)
        {
            SaveInSourceFolder = !outputResult.UseOutputFolder;
            OutputFolder = outputResult.OutputFolder;
            Overwrite = outputResult.Overwrite;

            UpdateOutputFolder();
            SaveSettings();
        }
    }

    private void UpdateOutputFolder()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            UseSourceFolderVisible = SaveInSourceFolder;
            UseOutputFolderVisible = !SaveInSourceFolder;
        });
    }

    [RelayCommand]
    private void OpenOutputFolder()
    {
        UiUtil.OpenFolder(OutputFolder);
    }

    [RelayCommand]
    private void Convert()
    {
        foreach (var batchItem in BatchItems)
        {
            batchItem.Status = "-";
        }

        SaveSettings();

        var config = MakeBatchConvertConfig();
        _batchConverter.Initialize(config);
        var start = System.Diagnostics.Stopwatch.GetTimestamp();

        IsProgressVisible = true;
        var unused = Task.Run(async () =>
        {
            var count = 1;
            foreach (var batchItem in BatchItems)
            {
                var countDisplay = count;
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ProgressText = $"Converting {countDisplay}/{BatchItems.Count}";
                    Progress = countDisplay / (double)BatchItems.Count;
                });
                await _batchConverter.Convert(batchItem);
                count++;
            }
            IsProgressVisible = false;

            var end = System.Diagnostics.Stopwatch.GetTimestamp();
            ShowStatus($"{BatchItems.Count} files converted in {ProgressHelper.ToTimeResult(new TimeSpan(end - start).TotalMilliseconds)}");
        });
    }

    private BatchConvertConfig MakeBatchConvertConfig()
    {
        return new BatchConvertConfig
        {
            SaveInSourceFolder = SaveInSourceFolder,
            OutputFolder = OutputFolder,
            Overwrite = Overwrite,
            TargetFormatName = SelectedTargetFormat ?? string.Empty,
            TargetEncoding = SelectedTargetEncoding ?? string.Empty,

            RemoveFormatting = new BatchConvertConfig.RemoveFormattingSettings
            {
                IsActive = SelectedBatchFunction?.Type == BatchConvertFunctionType.RemoveFormatting,
                RemoveAll = FormattingRemoveAll,
                RemoveItalic = FormattingRemoveItalic,
                RemoveBold = FormattingRemoveBold,
                RemoveUnderline = FormattingRemoveUnderline,
                RemoveColor = FormattingRemoveColors,
                RemoveFontName = FormattingRemoveFontTags,
                RemoveAlignment = FormattingRemoveAlignmentTags,
            },

            OffsetTimeCodes = new BatchConvertConfig.OffsetTimeCodesSettings
            {
                IsActive = SelectedBatchFunction?.Type == BatchConvertFunctionType.OffsetTimeCodes,
                Forward = OffsetTimeCodesForward,
                Milliseconds = (long)OffsetTimeCodesTime.TotalMilliseconds,
            },
        };
    }

    [RelayCommand]
    private async Task Ok()
    {
        await Shell.Current.GoToAsync("..");
    }

    public void FunctionSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        foreach (var batchFunction in BatchFunctions)
        {
            batchFunction.View.IsVisible = batchFunction == SelectedBatchFunction;
        }
    }

    public void OnDisappearing()
    {
        _stopping = true;
        SaveSettings();
    }

    private void LoadSettings()
    {
        SelectedTargetFormat = Se.Settings.Tools.BatchConvert.TargetFormat;
        if (string.IsNullOrEmpty(SelectedTargetFormat))
        {
            SelectedTargetFormat = TargetFormats.First();
        }

        SelectedTargetEncoding = Se.Settings.Tools.BatchConvert.TargetEncoding;
        if (string.IsNullOrEmpty(SelectedTargetEncoding))
        {
            SelectedTargetEncoding = TargetEncodings.First();
        }

        SelectedBatchFunction = BatchFunctions.First();
        foreach (var batchFunction in BatchFunctions)
        {
            batchFunction.View.IsVisible = batchFunction == SelectedBatchFunction;
        }

        SaveInSourceFolder = !Se.Settings.Tools.BatchConvert.UseOutputFolder;
        OutputFolder = Se.Settings.Tools.BatchConvert.OutputFolder;
        Overwrite = Se.Settings.Tools.BatchConvert.Overwrite;
        UpdateOutputFolder();

        FormattingRemoveAll = Se.Settings.Tools.BatchConvert.FormattingRemoveAll;
        FormattingRemoveItalic = Se.Settings.Tools.BatchConvert.FormattingRemoveItalic;
        FormattingRemoveBold = Se.Settings.Tools.BatchConvert.FormattingRemoveBold;
        FormattingRemoveUnderline = Se.Settings.Tools.BatchConvert.FormattingRemoveUnderline;
        FormattingRemoveFontTags = Se.Settings.Tools.BatchConvert.FormattingRemoveFontTags;
        FormattingRemoveAlignmentTags = Se.Settings.Tools.BatchConvert.FormattingRemoveAlignmentTags;
        FormattingRemoveColors = Se.Settings.Tools.BatchConvert.FormattingRemoveColorTags;

        OffsetTimeCodesTime = TimeSpan.FromMilliseconds(Se.Settings.Tools.BatchConvert.OffsetTimeCodesMilliseconds);
        OffsetTimeCodesForward = Se.Settings.Tools.BatchConvert.OffsetTimeCodesForward;
    }

    private void SaveSettings()
    {
        Se.Settings.Tools.BatchConvert.TargetFormat = SelectedTargetFormat ?? string.Empty;
        Se.Settings.Tools.BatchConvert.TargetEncoding = SelectedTargetEncoding ?? string.Empty;
        Se.Settings.Tools.BatchConvert.ActiveFunctions = BatchFunctions.Where(p => p.IsSelected).Select(p => p.Type.ToString()).ToArray();
        Se.Settings.Tools.BatchConvert.UseOutputFolder = !SaveInSourceFolder;
        Se.Settings.Tools.BatchConvert.OutputFolder = OutputFolder;
        Se.Settings.Tools.BatchConvert.Overwrite = Overwrite;

        Se.Settings.Tools.BatchConvert.FormattingRemoveAll = FormattingRemoveAll;
        Se.Settings.Tools.BatchConvert.FormattingRemoveItalic = FormattingRemoveItalic;
        Se.Settings.Tools.BatchConvert.FormattingRemoveBold = FormattingRemoveBold;
        Se.Settings.Tools.BatchConvert.FormattingRemoveUnderline = FormattingRemoveUnderline;
        Se.Settings.Tools.BatchConvert.FormattingRemoveFontTags = FormattingRemoveFontTags;
        Se.Settings.Tools.BatchConvert.FormattingRemoveAlignmentTags = FormattingRemoveAlignmentTags;
        Se.Settings.Tools.BatchConvert.FormattingRemoveColorTags = FormattingRemoveColors;

        Se.Settings.Tools.BatchConvert.OffsetTimeCodesMilliseconds = (long)OffsetTimeCodesTime.TotalMilliseconds;
        Se.Settings.Tools.BatchConvert.OffsetTimeCodesForward = OffsetTimeCodesForward;

        Se.SaveSettings();
    }

    private void ShowStatus(string statusText)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            LabelStatusText.Opacity = 0;
            StatusText = statusText;
            LabelStatusText.FadeTo(1, 200);
        });

        Page?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(6_000), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (_stopping)
                {
                    return;
                }

                if (StatusText == statusText)
                {
                    LabelStatusText.FadeTo(0, 200);
                }
            });
            return false;
        });
    }
}
