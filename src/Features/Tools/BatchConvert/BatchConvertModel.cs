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

    [ObservableProperty] private bool _formattingRemoveAll;
    [ObservableProperty] private bool _formattingRemoveItalic;
    [ObservableProperty] private bool _formattingRemoveBold;
    [ObservableProperty] private bool _formattingRemoveUnderline;
    [ObservableProperty] private bool _formattingRemoveFontTags;
    [ObservableProperty] private bool _formattingRemoveAlignmentTags;
    [ObservableProperty] private bool _formattingRemoveColors;

    public View ViewRemoveFormatting { get; set; }
    public View ViewOffsetTimeCodes { get; set; }

    private readonly IFileHelper _fileHelper;
    private readonly IPopupService _popupService;

    public BatchConvertModel(IFileHelper fileHelper, IPopupService popupService)
    {
        _fileHelper = fileHelper;
        _popupService = popupService;
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

                LoadSettings();
            });
            return false;
        });
    }

    private BatchConvertFunction MakeFunction(BatchConvertFunctionType functionType, string name, View view, string[] activeFunctions)
    {
        var isActive = activeFunctions.Contains(functionType.ToString());
        return new BatchConvertFunction(functionType, name, isActive, view);
    }

    [RelayCommand]
    private async Task FileAdd()
    {
        var fileNames = await _fileHelper.PickAndShowVideoFiles("Pick video files");
        if (fileNames.Length == 0)
        {
            return;
        }

        foreach (var fileName in fileNames)
        {
            var fileInfo = new FileInfo(fileName);
            var batchItem = new BatchConvertItem(fileName, fileInfo.Length, "Unknown");

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
            UseOutputFolder = Se.Settings.Tools.BatchConvert.UseOutputFolder,
            OutputFolder = Se.Settings.Tools.BatchConvert.OutputFolder,
            Overwrite = Se.Settings.Tools.BatchConvert.Overwrite,
        };

        var result = await _popupService.ShowPopupAsync<BatchConvertOutputPropertiesPopupModel>(onPresenting: viewModel => viewModel.Initialize(input), CancellationToken.None);

        if (result is BatchConvertOutputProperties outputResult)
        {
            Se.Settings.Tools.BatchConvert.UseOutputFolder = outputResult.UseOutputFolder;
            Se.Settings.Tools.BatchConvert.OutputFolder = outputResult.OutputFolder;
            Se.Settings.Tools.BatchConvert.Overwrite = outputResult.Overwrite;
            Se.SaveSettings();
        }
    }

    [RelayCommand]
    private async Task Convert()
    {
        SaveSettings();
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
        SaveSettings();
    }

    private void LoadSettings()
    {
        FormattingRemoveAll = Se.Settings.Tools.BatchConvert.FormattingRemoveAll;
        FormattingRemoveItalic = Se.Settings.Tools.BatchConvert.FormattingRemoveItalic;
        FormattingRemoveBold = Se.Settings.Tools.BatchConvert.FormattingRemoveBold;
        FormattingRemoveUnderline = Se.Settings.Tools.BatchConvert.FormattingRemoveUnderline;
        FormattingRemoveFontTags = Se.Settings.Tools.BatchConvert.FormattingRemoveFontTags;
        FormattingRemoveAlignmentTags = Se.Settings.Tools.BatchConvert.FormattingRemoveAlignmentTags;
        FormattingRemoveColors = Se.Settings.Tools.BatchConvert.FormattingRemoveColorTags;
    }

    private void SaveSettings()
    {
        Se.Settings.Tools.BatchConvert.TargetFormat = SelectedTargetFormat ?? string.Empty;
        Se.Settings.Tools.BatchConvert.TargetEncoding = SelectedTargetEncoding ?? string.Empty;
        Se.Settings.Tools.BatchConvert.ActiveFunctions = BatchFunctions.Where(p => p.IsSelected).Select(p => p.Type.ToString()).ToArray();

        Se.Settings.Tools.BatchConvert.FormattingRemoveAll = FormattingRemoveAll;
        Se.Settings.Tools.BatchConvert.FormattingRemoveItalic = FormattingRemoveItalic;
        Se.Settings.Tools.BatchConvert.FormattingRemoveBold = FormattingRemoveBold;
        Se.Settings.Tools.BatchConvert.FormattingRemoveUnderline = FormattingRemoveUnderline;
        Se.Settings.Tools.BatchConvert.FormattingRemoveFontTags = FormattingRemoveFontTags;
        Se.Settings.Tools.BatchConvert.FormattingRemoveAlignmentTags = FormattingRemoveAlignmentTags;
        Se.Settings.Tools.BatchConvert.FormattingRemoveColorTags = FormattingRemoveColors;

        Se.SaveSettings();
    }
}
