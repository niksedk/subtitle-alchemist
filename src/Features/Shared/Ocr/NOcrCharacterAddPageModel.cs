using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.SubtitleFormats;
using SubtitleAlchemist.Features.Tools.AdjustDuration;
using SubtitleAlchemist.Features.Tools.BatchConvert;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Logic.Media;

namespace SubtitleAlchemist.Features.Shared.Ocr;

public partial class NOcrCharacterAddPageModel : ObservableObject, IQueryAttributable
{
    public NOcrCharacterAddPage? Page { get; set; }

    [ObservableProperty] private string _outputFolder;
    [ObservableProperty] private bool _saveInSourceFolder;
    [ObservableProperty] private bool _useSourceFolderVisible;
    [ObservableProperty] private bool _useOutputFolderVisible;
    [ObservableProperty] private bool _overwrite;
    [ObservableProperty] private string _targetFormatName;
    [ObservableProperty] private string _targetEncoding;
    [ObservableProperty] private ObservableCollection<BatchConvertItem> _batchItems;
    [ObservableProperty] private BatchConvertItem? _selectedBatchItem;
    [ObservableProperty] private string _batchItemsInfo;

    [ObservableProperty] private ObservableCollection<string> _targetFormats;
    [ObservableProperty] private string? _selectedTargetFormat;
    [ObservableProperty] private ObservableCollection<string> _targetEncodings;
    [ObservableProperty] private string? _selectedTargetEncoding;

    [ObservableProperty] private ObservableCollection<BatchConvertFunction> _batchFunctions;
    [ObservableProperty] private BatchConvertFunction? _selectedBatchFunction;

    [ObservableProperty] private bool _isProgressVisible;
    [ObservableProperty] private bool _isConverting;
    [ObservableProperty] private bool _areControlsEnabled;
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

    // Offset time codes
    [ObservableProperty] private bool _offsetTimeCodesForward;
    [ObservableProperty] private bool _offsetTimeCodesBack;
    [ObservableProperty] private TimeSpan _offsetTimeCodesTime;

    // Adjust display duration
    [ObservableProperty] private ObservableCollection<AdjustDurationItem> _adjustTypes;
    [ObservableProperty] private AdjustDurationItem _selectedAdjustType;
    [ObservableProperty] private TimeSpan _adjustSeconds;
    [ObservableProperty] private int _adjustPercentage;
    [ObservableProperty] private TimeSpan _adjustFixedValue;
    [ObservableProperty] private decimal _adjustRecalculateMaximumCharacters;
    [ObservableProperty] private decimal _adjustRecalculateOptimalCharacters;
    [ObservableProperty] private bool _adjustIsSecondsVisible;
    [ObservableProperty] private bool _adjustIsPercentVisible;
    [ObservableProperty] private bool _adjustIsFixedVisible;
    [ObservableProperty] private bool _adjustIsRecalculateVisible;

    // Delete lines
    [ObservableProperty] private ObservableCollection<int> _deleteLineNumbers;
    [ObservableProperty] private int _deleteXFirstLines;
    [ObservableProperty] private int _deleteXLastLines;
    [ObservableProperty] private string _deleteLinesContains;

    // Change frame rate
    [ObservableProperty] private ObservableCollection<double> _frameRates;
    [ObservableProperty] private double _selectedFromFrameRate;
    [ObservableProperty] private double _selectedToFrameRate;


    public View ViewRemoveFormatting { get; set; }
    public View ViewOffsetTimeCodes { get; set; }
    public View ViewAdjustDuration { get; set; }
    public View ViewDeleteLines { get; set; }
    public View ViewChangeFrameRate { get; set; }

    public Label LabelStatusText { get; set; }

    [ObservableProperty] private string _statusText;

    private readonly IFileHelper _fileHelper;
    private readonly IPopupService _popupService;
    private readonly IBatchConverter _batchConverter;
    private CancellationToken _cancellationToken;
    private CancellationTokenSource _cancellationTokenSource;

    private bool _stopping;

    public NOcrCharacterAddPageModel(IFileHelper fileHelper, IPopupService popupService, IBatchConverter batchConverter)
    {
        _fileHelper = fileHelper;
        _popupService = popupService;
        _batchConverter = batchConverter;
        _outputFolder = string.Empty;
        _saveInSourceFolder = true;
        _targetFormatName = SubRip.NameOfFormat;
        _targetEncoding = TextEncoding.Utf8WithBom;

        _batchItems = new ObservableCollection<BatchConvertItem>();
        _batchItemsInfo = string.Empty;
        _targetFormats = new ObservableCollection<string>(SubtitleFormat.AllSubtitleFormats.Select(p => p.Name));
        _targetEncodings = new ObservableCollection<string>(EncodingHelper.GetEncodings().Select(p => p.DisplayName).ToList());

        _batchFunctions = new ObservableCollection<BatchConvertFunction>();
        ViewRemoveFormatting = new BoxView();
        ViewOffsetTimeCodes = new BoxView();
        ViewAdjustDuration = new BoxView();
        ViewDeleteLines = new BoxView();
        ViewChangeFrameRate = new BoxView();

        LabelStatusText = new Label();
        _statusText = string.Empty;
        _progressText = string.Empty;
        _areControlsEnabled = true;

        _adjustTypes = new ObservableCollection<AdjustDurationItem>
        {
            new(AdjustDurationType.Seconds, "Seconds"),
            new(AdjustDurationType.Percent, "Percent"),
            new(AdjustDurationType.Fixed, "Fixed"),
            new(AdjustDurationType.Recalculate, "Recalculate"),
        };

        _selectedAdjustType = _adjustTypes.First();
        _deleteLinesContains = string.Empty;
        _deleteLineNumbers = new ObservableCollection<int>(Enumerable.Range(0, 100));
        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;

        _frameRates = new ObservableCollection<double>
        {
            23.976,
            24,
            25,
            29.97,
            30,
            48,
            59.94,
            60,
            120,
        };
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
                    MakeFunction(BatchConvertFunctionType.AdjustDisplayDuration, "Adjust display duration", ViewAdjustDuration, activeFunctions),
                    MakeFunction(BatchConvertFunctionType.DeleteLines, "Delete lines", ViewDeleteLines, activeFunctions),
                    MakeFunction(BatchConvertFunctionType.ChangeFrameRate, "Change frame rate", ViewChangeFrameRate, activeFunctions),
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

        MakeBatchItemsInfo();
    }

    private void MakeBatchItemsInfo()
    {
        BatchItemsInfo = $"{BatchItems.Count:#,###,##0} items";
    }

    [RelayCommand]
    private void FileRemove()
    {
        if (SelectedBatchItem != null)
        {
            BatchItems.Remove(SelectedBatchItem);
        }

        MakeBatchItemsInfo();
    }

    [RelayCommand]
    private void FileClear()
    {
        BatchItems.Clear();
        MakeBatchItemsInfo();
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
    private void CancelConvert()
    {
        _cancellationTokenSource.Cancel();
    }

    [RelayCommand]
    private void Convert()
    {
        if (BatchItems.Count == 0)
        {
            ShowStatus("No files to convert");
            return;
        }

        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;

        foreach (var batchItem in BatchItems)
        {
            batchItem.Status = "-";
        }

        SaveSettings();

        var config = MakeBatchConvertConfig();
        _batchConverter.Initialize(config);
        var start = System.Diagnostics.Stopwatch.GetTimestamp();

        IsProgressVisible = true;
        IsConverting = true;
        AreControlsEnabled = false;
        var unused = Task.Run(async () =>
        {
            var count = 1;
            foreach (var batchItem in BatchItems)
            {
                var countDisplay = count;
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ProgressText = $"Converting {countDisplay:#,###,##0}/{BatchItems.Count:#,###,##0}";
                    Progress = countDisplay / (double)BatchItems.Count;
                });
                await _batchConverter.Convert(batchItem);
                count++;

                if (_cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }
            IsProgressVisible = false;
            IsConverting = false;
            AreControlsEnabled = true;

            var end = System.Diagnostics.Stopwatch.GetTimestamp();
            var message = $"{BatchItems.Count:#,###,##0} files converted in {ProgressHelper.ToTimeResult(new TimeSpan(end - start).TotalMilliseconds)}";
            if (_cancellationToken.IsCancellationRequested)
            {
                message += " - conversion cancelled by user";
            }
            ShowStatus(message);
        });
    }

    private BatchConvertConfig MakeBatchConvertConfig()
    {
        var activeFunctions = BatchFunctions.Where(p => p.IsSelected).Select(p => p.Type).ToList();

        return new BatchConvertConfig
        {
            SaveInSourceFolder = SaveInSourceFolder,
            OutputFolder = OutputFolder,
            Overwrite = Overwrite,
            TargetFormatName = SelectedTargetFormat ?? string.Empty,
            TargetEncoding = SelectedTargetEncoding ?? string.Empty,

            RemoveFormatting = new BatchConvertConfig.RemoveFormattingSettings
            {
                IsActive = activeFunctions.Contains(BatchConvertFunctionType.RemoveFormatting),
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
                IsActive = activeFunctions.Contains(BatchConvertFunctionType.OffsetTimeCodes),
                Forward = OffsetTimeCodesForward,
                Milliseconds = (long)OffsetTimeCodesTime.TotalMilliseconds,
            },

            AdjustDuration = new BatchConvertConfig.AdjustDurationSettings
            {
                IsActive = activeFunctions.Contains(BatchConvertFunctionType.AdjustDisplayDuration),
                AdjustmentType = SelectedAdjustType.Type,
                Percentage = AdjustPercentage,
                FixedMilliseconds = (int) AdjustFixedValue.TotalMilliseconds,
                MaxCharsPerSecond = (double)AdjustRecalculateMaximumCharacters,
                OptimalCharsPerSecond = (double)AdjustRecalculateOptimalCharacters,
            },

            DeleteLines = new BatchConvertConfig.DeleteLinesSettings
            {
                IsActive = activeFunctions.Contains(BatchConvertFunctionType.DeleteLines),
                DeleteXFirst = DeleteXFirstLines,
                DeleteXLast = DeleteXLastLines,
                DeleteContains = DeleteLinesContains,
            },

            ChangeFrameRate = new BatchConvertConfig.ChangeFrameRateSettings
            {
                IsActive = activeFunctions.Contains(BatchConvertFunctionType.ChangeFrameRate),
                FromFrameRate = SelectedFromFrameRate,
                ToFrameRate = SelectedToFrameRate,
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
        _cancellationTokenSource.Cancel();
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

        var adjustType = AdjustTypes.FirstOrDefault(p => p.Type.ToString() == Se.Settings.Tools.BatchConvert.AdjustVia);
        SelectedAdjustType = adjustType ?? AdjustTypes.First();
        AdjustSeconds = TimeSpan.FromSeconds(Se.Settings.Tools.BatchConvert.AdjustDurationSeconds);
        AdjustFixedValue = TimeSpan.FromMilliseconds(Se.Settings.Tools.BatchConvert.AdjustDurationFixedMilliseconds);
        AdjustPercentage = Se.Settings.Tools.BatchConvert.AdjustDurationPercentage;
        AdjustRecalculateOptimalCharacters = (decimal)Se.Settings.Tools.BatchConvert.AdjustOptimalCps;
        AdjustRecalculateMaximumCharacters = (decimal)Se.Settings.Tools.BatchConvert.AdjustMaxCps;

        SelectedFromFrameRate = Se.Settings.Tools.BatchConvert.ChangeFrameRateFrom;
        SelectedToFrameRate = Se.Settings.Tools.BatchConvert.ChangeFrameRateTo;
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

        Se.Settings.Tools.BatchConvert.AdjustVia = SelectedAdjustType.Type.ToString();
        Se.Settings.Tools.BatchConvert.AdjustDurationSeconds = AdjustSeconds.TotalSeconds;
        Se.Settings.Tools.BatchConvert.AdjustDurationPercentage = AdjustPercentage;
        Se.Settings.Tools.BatchConvert.AdjustDurationFixedMilliseconds = (int)AdjustFixedValue.TotalMilliseconds;
        Se.Settings.Tools.BatchConvert.AdjustOptimalCps = (double)AdjustRecalculateOptimalCharacters;
        Se.Settings.Tools.BatchConvert.AdjustMaxCps = (double)AdjustRecalculateMaximumCharacters;

        Se.Settings.Tools.BatchConvert.ChangeFrameRateFrom = SelectedFromFrameRate;
        Se.Settings.Tools.BatchConvert.ChangeFrameRateTo = SelectedToFrameRate;

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

    public void PickerAdjustVia_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (sender is Picker { SelectedItem: AdjustDurationItem selectedItem })
        {
            AdjustIsSecondsVisible = false;
            AdjustIsFixedVisible = false;
            AdjustIsPercentVisible = false;
            AdjustIsRecalculateVisible = false;

            if (selectedItem.Type == AdjustDurationType.Seconds)
            {
                AdjustIsSecondsVisible = true;
            }
            else if (selectedItem.Type == AdjustDurationType.Percent)
            {
                AdjustIsPercentVisible = true;
            }
            else if (selectedItem.Type == AdjustDurationType.Fixed)
            {
                AdjustIsFixedVisible = true;
            }
            else if (selectedItem.Type == AdjustDurationType.Recalculate)
            {
                AdjustIsRecalculateVisible = true;
            }
        }
    }
}
