using System.Collections.ObjectModel;
using System.Drawing.Text;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nikse.SubtitleEdit.Core.Common;
using SubtitleAlchemist.Controls.ColorPickerControl;
using SubtitleAlchemist.Logic.Config;
using SubtitleAlchemist.Logic.Media;

namespace SubtitleAlchemist.Features.Files.ExportImage;

public partial class ExportImagePageModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty] public partial string Title { get; set; }

    [ObservableProperty] public partial bool IsExportButtonEnabled { get; set; }

    [ObservableProperty] public partial ObservableCollection<string> FontNames { get; set; }
    [ObservableProperty] public partial string SelectedFontName { get; set; }

    [ObservableProperty] public partial ObservableCollection<ResolutionItem> ResolutionItems { get; set; }
    [ObservableProperty] public partial ResolutionItem SelectedResolutionItem { get; set; }

    [ObservableProperty] public partial ObservableCollection<AlignmentItem> AlignmentItems { get; set; }
    [ObservableProperty] public partial AlignmentItem SelectedAlignmentItem { get; set; }

    [ObservableProperty] public partial ObservableCollection<int> FontSizes { get; set; }
    [ObservableProperty] public partial int SelectedFontSize { get; set; }

    [ObservableProperty] public partial ObservableCollection<BorderStyleItem> BorderStyleItems { get; set; }
    [ObservableProperty] public partial BorderStyleItem SelectedBorderStyleItem { get; set; }

    [ObservableProperty] public partial ObservableCollection<string> ProfileItems { get; set; }
    [ObservableProperty] public partial string SelectedProfileItem { get; set; }

    [ObservableProperty] public partial ObservableCollection<int> BottomMarginItems { get; set; }
    [ObservableProperty] public partial int SelectedBottomMarginItem { get; set; }

    [ObservableProperty] public partial ObservableCollection<int> BottomMarginUnitItems { get; set; }
    [ObservableProperty] public partial int SelectedBottomMarginUnitItem { get; set; }

    [ObservableProperty] public partial ObservableCollection<int> LeftRightMarginItems { get; set; }
    [ObservableProperty] public partial int SelectedLeftRightMarginItem { get; set; }

    [ObservableProperty] public partial ObservableCollection<int> LeftRightMarginUnitItems { get; set; }
    [ObservableProperty] public partial int SelectedLeftRightMarginUnitItem { get; set; }

    [ObservableProperty] public partial ObservableCollection<decimal> FrameRateItems { get; set; }
    [ObservableProperty] public partial decimal SelectedframeRateItem { get; set; }

    [ObservableProperty] public partial decimal FontKerningExtra { get; set; }

    [ObservableProperty] public partial bool IsBold { get; set; }
    [ObservableProperty] public partial Color FontColor { get; set; }
    [ObservableProperty] public partial Color BorderColor { get; set; }
    [ObservableProperty] public partial float BorderWidth { get; set; }
    [ObservableProperty] public partial Color ShadowColor { get; set; }
    [ObservableProperty] public partial float ShadowWidth{ get; set; }
    [ObservableProperty] public partial int ShadowAlpha { get; set; }

    public ExportImagePage? Page { get; set; }

    private readonly CancellationTokenSource _cancellationTokenSource;
    private Subtitle _subtitle;
    private IFileHelper _fileHelper;
    private readonly IPopupService _popupService;

    public ExportImagePageModel(IFileHelper fileHelper, IPopupService popupService)
    {
        Title = "Export Images";
        IsExportButtonEnabled = true;

        FontNames = new ObservableCollection<string>();
        SelectedFontName = "Arial";

        ResolutionItems = new ObservableCollection<ResolutionItem>(ResolutionItem.GetResolutions());
        SelectedResolutionItem = ResolutionItems[0];

        AlignmentItems = new ObservableCollection<AlignmentItem>(AlignmentItem.GetAlignments());
        SelectedAlignmentItem = AlignmentItems[0];

        BorderStyleItems = new ObservableCollection<BorderStyleItem>(BorderStyleItem.GetBorderStyles());
        SelectedBorderStyleItem = BorderStyleItems[0];

        FontSizes = new ObservableCollection<int>(Enumerable.Range(8, 40));
        SelectedFontSize = 12;

        ProfileItems = new ObservableCollection<string>(new[] { "Default" });
        SelectedProfileItem = ProfileItems[0];

        BottomMarginItems = new ObservableCollection<int>(Enumerable.Range(0, 100));
        SelectedBottomMarginItem = 5;

        BottomMarginUnitItems = new ObservableCollection<int>(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
        SelectedBottomMarginUnitItem = 0;

        LeftRightMarginItems = new ObservableCollection<int>(Enumerable.Range(0, 100));
        SelectedLeftRightMarginItem = 5;

        LeftRightMarginUnitItems = new ObservableCollection<int>(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
        SelectedLeftRightMarginUnitItem = 0;

        FrameRateItems = new ObservableCollection<decimal>(new[] { 23.976m, 24m, 25m, 29.97m, 30m, 50m, 59.94m, 60m });
        SelectedframeRateItem = 24;

        FontColor = Colors.White;
        BorderColor = Colors.Black;
        ShadowColor = Colors.Black;
        ShadowWidth = 2;
        BorderWidth = 2;        
        ShadowAlpha = 200;

        _subtitle = new Subtitle();
        _fileHelper = fileHelper;
        _cancellationTokenSource = new CancellationTokenSource();
        _popupService = popupService;
    }

    private void LoadFonts()
    {
        var fontFamilies = FontHelper.GetSystemFonts();
        FontNames = new ObservableCollection<string>(fontFamilies.OrderBy(f => f));
        SelectedFontName = FontNames.First();
    }

    [RelayCommand]
    public void PickColor()
    {
    }

    [RelayCommand]
    public async Task Cancel()
    {
        _cancellationTokenSource.Cancel();
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    public void Export()
    {
        IsExportButtonEnabled = false;
        SaveSettings();

        var fileHelper = new FileHelper();
        var task = Task.Run(() =>
        {
            var ms = new MemoryStream();
            var exporter = new BluRayExporter();
            var lines = new List<ImageExportLine>(_subtitle.Paragraphs.Select(p => new ImageExportLine() { Paragraph = p }));
            var settings = Se.Settings.File.ExportImages;
            var progress = new Progress<float>();
            var result = exporter.Export(ms, settings, lines, progress, _cancellationTokenSource.Token);
            if (result)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await _fileHelper.SaveStreamAs(ms, "Save Bluray sup file as...", Path.GetFileNameWithoutExtension(_subtitle.FileName), ".sup", _cancellationTokenSource.Token);

                    //TODO: show popup

                    await Shell.Current.GoToAsync("..", new Dictionary<string, object>
                    {
                        { "Page", nameof(ExportImagePage) },
                    });
                });
            }
            else
            {
                IsExportButtonEnabled = true;
            }
        });
    }

    private void SaveSettings()
    {
        if (SelectedFontName != null)
        {
            Se.Settings.File.ExportImages.FontName = SelectedFontName;
        }

        Se.Settings.File.ExportImages.FontSize = SelectedFontSize;
        Se.Settings.File.ExportImages.ResolutionWidth = SelectedResolutionItem?.Width ?? 1920;
        Se.Settings.File.ExportImages.ResolutionHeight = SelectedResolutionItem?.Height ?? 1080;
        Se.Settings.File.ExportImages.Alignment = SelectedAlignmentItem?.Alignment.ToString() ?? string.Empty;
        Se.Settings.File.ExportImages.BorderStyle = SelectedBorderStyleItem?.BorderStyle.ToString() ?? string.Empty;
        Se.Settings.File.ExportImages.Profile = SelectedProfileItem ?? "Default";
        Se.Settings.File.ExportImages.BottomMargin = SelectedBottomMarginItem;
        Se.Settings.File.ExportImages.BottomMarginUnit = SelectedBottomMarginUnitItem;
        Se.Settings.File.ExportImages.LeftRightMargin = SelectedLeftRightMarginItem;
        Se.Settings.File.ExportImages.LeftRightMarginUnit = SelectedLeftRightMarginUnitItem;
        Se.Settings.File.ExportImages.FontKerningExtra = FontKerningExtra;
        Se.Settings.File.ExportImages.FrameRate = SelectedframeRateItem;
        Se.Settings.File.ExportImages.FontColor = FontColor.ToHex();
        Se.Settings.File.ExportImages.BorderColor = BorderColor.ToHex();
        Se.Settings.File.ExportImages.BorderWidth = BorderWidth;
        Se.Settings.File.ExportImages.ShadowColor = ShadowColor.ToHex();
        Se.Settings.File.ExportImages.ShadowWidth = ShadowWidth;
        Se.Settings.File.ExportImages.ShadowAlpha = ShadowAlpha;

        Se.SaveSettings();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        var page = query["Page"].ToString();
        _subtitle = (Subtitle)query["Subtitle"];

        if (query.ContainsKey("Format") && query["Format"] is string format)
        {

        }

        Page?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LoadFonts();
                LoadSettings();
            });
            return false;
        });
    }

    private void LoadSettings()
    {
        var settings = Se.Settings.File.ExportImages;

        SelectedFontName = settings.FontName;
        SelectedFontSize = settings.FontSize;
        
        SelectedResolutionItem = 
            ResolutionItems.FirstOrDefault(r => r.Width == settings.ResolutionWidth && r.Height == settings.ResolutionHeight) 
            ?? ResolutionItems.First(p=>p.Width == 1920);
        
        SelectedAlignmentItem = 
            AlignmentItems.FirstOrDefault(a => a.Alignment.ToString() == settings.Alignment) 
            ?? AlignmentItems.First(p=>p.Alignment == AlignmentType.BottomCenter);
        
        SelectedBorderStyleItem = 
            BorderStyleItems.FirstOrDefault(b => b.BorderStyle.ToString() == settings.BorderStyle) 
            ?? BorderStyleItems.First(p=>p.BorderStyle == BorderStyleType.Numbered);

        SelectedProfileItem = settings.Profile;
        SelectedBottomMarginItem = settings.BottomMargin;
        SelectedBottomMarginUnitItem = settings.BottomMarginUnit;
        SelectedLeftRightMarginItem = settings.LeftRightMargin;
        SelectedLeftRightMarginUnitItem = settings.LeftRightMarginUnit;
        FontKerningExtra = settings.FontKerningExtra;
        SelectedframeRateItem = settings.FrameRate;
        FontColor = Color.FromArgb(settings.FontColor);
        BorderColor = Color.FromArgb(settings.BorderColor);
        ShadowColor = Color.FromArgb(settings.ShadowColor);
        ShadowWidth = settings.ShadowWidth;
        ShadowAlpha = settings.ShadowAlpha;
    }

    internal void FontColorTapped(object? sender, TappedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var result = await _popupService.ShowPopupAsync<ColorPickerPopupModel>(
                onPresenting: vm => vm.SetCurrentColor(FontColor),
                CancellationToken.None);

            if (result is Color color)
            {
                FontColor = color;
            }
        });
    }

    internal void BorderColorTapped(object? sender, TappedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var result = await _popupService.ShowPopupAsync<ColorPickerPopupModel>(
                onPresenting: vm => vm.SetCurrentColor(BorderColor),
                CancellationToken.None);

            if (result is Color color)
            {
                BorderColor = color;
            }
        });
    }

    internal void ShadowColorTapped(object? sender, TappedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var result = await _popupService.ShowPopupAsync<ColorPickerPopupModel>(
                onPresenting: vm => vm.SetCurrentColor(ShadowColor),
                CancellationToken.None);

            if (result is Color color)
            {
                ShadowColor = color;
            }
        });
    }
}
