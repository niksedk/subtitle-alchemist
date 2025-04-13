using CommunityToolkit.Mvvm.ComponentModel;
using SharpHook;
using SharpHook.Native;
using SubtitleAlchemist.Logic;
using SubtitleAlchemist.Logic.Config;

namespace SubtitleAlchemist.Features.Main.LayoutPicker
{
    public partial class LayoutPickerPopupModel : ObservableObject
    {
        public int SelectedLayout
        {
            get => _selectedLayout;
            set
            {
                if (value < 0)
                {
                    return;
                }

                var old = _selectedLayout;
                _selectedLayout = value;
                MouseOutLayout(old);
                HighlightLayout(value);
            }
        }

        public const int Max  = 11;

        private readonly List<byte[]> _images;
        private readonly List<byte[]> _grayscaleImages;

        [ObservableProperty]
        public partial ImageSource? Layout1ImageSource { get; set; }

        [ObservableProperty]
        public partial ImageSource? Layout2ImageSource { get; set; }

        [ObservableProperty]
        public partial ImageSource? Layout3ImageSource { get; set; }

        [ObservableProperty]
        public partial ImageSource? Layout4ImageSource { get; set; }

        [ObservableProperty]
        public partial ImageSource? Layout5ImageSource { get; set; }

        [ObservableProperty]
        public partial ImageSource? Layout6ImageSource { get; set; }

        [ObservableProperty]
        public partial ImageSource? Layout7ImageSource { get; set; }

        [ObservableProperty]
        public partial ImageSource? Layout8ImageSource { get; set; }

        [ObservableProperty]
        public partial ImageSource? Layout9ImageSource { get; set; }

        [ObservableProperty]
        public partial ImageSource? Layout10ImageSource { get; set; }

        [ObservableProperty]
        public partial ImageSource? Layout11ImageSource { get; set; }

        [ObservableProperty]
        public partial ImageSource? Layout12ImageSource { get; set; }

        private int _selectedLayout = -1;

        public LayoutPickerPopup? Popup { get; set; }

        public LayoutPickerPopupModel()
        {
            var selectedLayout = 0;
            var imagePath = Se.BaseFolder;

            _images = new List<byte[]>();
            _grayscaleImages = new List<byte[]>();
            for (var i = 0; i < 12; i++)
            {
                AssetHelper.CopyToAppData($"layout_{(i + 1):00}.png");

                var bytes = File.ReadAllBytes(Path.Combine(imagePath, $"layout_{(i + 1):00}.png"));
                _images.Add(bytes);

                var grayscaledBitmap = SkBitmapExtensions.ConvertToGrayscale(bytes);
                var greyMs = SkBitmapExtensions.BitmapToPngStream(grayscaledBitmap);
                _grayscaleImages.Add(greyMs.ToArray());

                MouseOutLayout(i);
            }

            SelectedLayout = selectedLayout;
        }

        private void SetImageSource(int layoutNumber, byte[] bytes)
        {
            switch (layoutNumber)
            {
                case 0:
                    Layout1ImageSource = ImageSource.FromStream(() => new MemoryStream(bytes));
                    break;
                case 1:
                    Layout2ImageSource = ImageSource.FromStream(() => new MemoryStream(bytes));
                    break;
                case 2:
                    Layout3ImageSource = ImageSource.FromStream(() => new MemoryStream(bytes));
                    break;
                case 3:
                    Layout4ImageSource = ImageSource.FromStream(() => new MemoryStream(bytes));
                    break;
                case 4:
                    Layout5ImageSource = ImageSource.FromStream(() => new MemoryStream(bytes));
                    break;
                case 5:
                    Layout6ImageSource = ImageSource.FromStream(() => new MemoryStream(bytes));
                    break;
                case 6:
                    Layout7ImageSource = ImageSource.FromStream(() => new MemoryStream(bytes));
                    break;
                case 7:
                    Layout8ImageSource = ImageSource.FromStream(() => new MemoryStream(bytes));
                    break;
                case 8:
                    Layout9ImageSource = ImageSource.FromStream(() => new MemoryStream(bytes));
                    break;
                case 9:
                    Layout10ImageSource = ImageSource.FromStream(() => new MemoryStream(bytes));
                    break;
                case 10:
                    Layout11ImageSource = ImageSource.FromStream(() => new MemoryStream(bytes));
                    break;
                case 11:
                    Layout12ImageSource = ImageSource.FromStream(() => new MemoryStream(bytes));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(layoutNumber));
            }
        }

        public void Close(int layoutNumber)
        {
            SelectedLayout = layoutNumber;
            Close();
        }

        private void Close()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Popup?.Close(new LayoutPickerPopupResult { SelectedLayout = SelectedLayout });
            });
        }

        public void MouseOverLayout(int i)
        {
            var bytes = _images[i];
            SetImageSource(i, bytes);
        }

        public void MouseOutLayout(int i)
        {
            if (i < 0)
            {
                return;
            }

            if (i == SelectedLayout)
            {
                HighlightLayout(i);
                return;
            }

            var bytes = _grayscaleImages[i];
            SetImageSource(i, bytes);
        }

        public void HighlightLayout(int i)
        {
            var bytes = _images[i];
            var bitmap = SkBitmapExtensions.MakeImageBrighter(bytes, 0.5f);
            bytes = SkBitmapExtensions.BitmapToPngStream(bitmap).ToArray();
            SetImageSource(i, bytes);
        }

        public void KeyPressed(object? sender, KeyboardHookEventArgs e)
        {
            // change SelectedLayout depending on the key pressed
            switch (e.Data.KeyCode)
            {
                case KeyCode.Vc1:
                    Close(0);
                    break;
                case KeyCode.Vc2:
                    Close(1);
                    break;
                case KeyCode.Vc3:
                    Close(2);
                    break;
                case KeyCode.Vc4:
                    Close(3);
                    break;
                case KeyCode.Vc5:
                    Close(4);
                    break;
                case KeyCode.Vc6:
                    Close(5);
                    break;
                case KeyCode.Vc7:
                    Close(6);
                    break;
                case KeyCode.Vc8:
                    Close(7);
                    break;
                case KeyCode.Vc9:
                    Close(8);
                    break;
                case KeyCode.Vc0:
                    Close(9);
                    break;
                case KeyCode.VcMinus:
                case KeyCode.VcLeft:
                case KeyCode.VcUp:
                    if (SelectedLayout > 0)
                    {
                        SelectedLayout--;
                        e.SuppressEvent = true;
                    }

                    break;
                case KeyCode.VcNumPadAdd:
                case KeyCode.VcRight:
                case KeyCode.VcDown:
                    if (SelectedLayout < Max)
                    {
                        SelectedLayout++;
                        e.SuppressEvent = true;
                    }

                    break;
                case KeyCode.VcEnter:
                case KeyCode.VcNumPadEnter:
                    e.SuppressEvent = true;
                    Close();

                    break;
            }
        }
    }
}
