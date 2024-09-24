using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace SubtitleAlchemist.Features.LayoutPicker;

public class LayoutPickerPopup : Popup
{
    private enum Row { Row0, Row1, Row2 }
    private enum Column { Column0, Column1, Column2, Column3 }

    private readonly LayoutPickerModel _model;

    protected override Task OnClosed(
        object? result, 
        bool wasDismissedByTappingOutsideOfPopup,
        CancellationToken token = new CancellationToken())
    {
        SharpHookHandler.AddKeyPressed(_model.KeyPressed);
        return base.OnClosed(result, wasDismissedByTappingOutsideOfPopup, token);
    }

    public LayoutPickerPopup(LayoutPickerModel model)
    {
        const int columnWidth = 220;
        const int columnHeight = 160;

        BindingContext = model;
        _model = model;

        CanBeDismissedByTappingOutsideOfPopup = true;

        var grid = new Grid
        {
            RowDefinitions = Rows.Define(
                (Row.Row0, columnHeight),
                (Row.Row1, columnHeight),
                (Row.Row2, columnHeight)
            ),

            ColumnDefinitions = Columns.Define(
                (Column.Column0, columnWidth),
                (Column.Column1, columnWidth),
                (Column.Column2, columnWidth),
                (Column.Column3, columnWidth)
            ),

            Children =
            {
                new Image()
                    .Margin(10)
                    .Bind(ImageButton.SourceProperty, static vm => vm.Layout1ImageSource,
                        static (LayoutPickerModel vm, ImageSource? imageSource) => vm.Layout1ImageSource = imageSource)
                    .Row(0).Column(0),
                MakeLabel(0, model, 0, 0),

                new Image()
                    .Margin(10)
                    .Bind(ImageButton.SourceProperty, static vm => vm.Layout2ImageSource,
                        static (LayoutPickerModel vm, ImageSource? imageSource) => vm.Layout2ImageSource = imageSource)
                    .Row(0).Column(1),
                MakeLabel(1, model, 0, 1),

                new Image()
                    .Margin(10)
                    .Bind(ImageButton.SourceProperty, static vm => vm.Layout3ImageSource,
                        static (LayoutPickerModel vm, ImageSource? imageSource) => vm.Layout3ImageSource = imageSource)
                    .Row(0).Column(2),
                MakeLabel(2, model, 0, 2),

                new Image()
                    .Margin(10)
                    .Bind(ImageButton.SourceProperty, static vm => vm.Layout4ImageSource,
                        static (LayoutPickerModel vm, ImageSource? imageSource) => vm.Layout4ImageSource = imageSource)
                    .Row(0).Column(3),
                    MakeLabel(3, model, 0, 3),

                new Image()
                    .Margin(10)
                    .Bind(ImageButton.SourceProperty, static vm => vm.Layout5ImageSource,
                        static (LayoutPickerModel vm, ImageSource? imageSource) => vm.Layout5ImageSource = imageSource)
                    .Row(1).Column(0),
                MakeLabel(4, model, 1, 0),

                new Image()
                    .Margin(10)
                    .Bind(ImageButton.SourceProperty, static vm => vm.Layout6ImageSource,
                        static (LayoutPickerModel vm, ImageSource? imageSource) => vm.Layout6ImageSource = imageSource)
                    .Row(1).Column(1),
                MakeLabel(5, model, 1, 1),

                new Image()
                    .Margin(10)
                    .Bind(ImageButton.SourceProperty, static vm => vm.Layout7ImageSource,
                        static (LayoutPickerModel vm, ImageSource? imageSource) => vm.Layout7ImageSource = imageSource)
                    .Row(1).Column(2),
                MakeLabel(6, model, 1, 2),

                new Image()
                    .Margin(10)
                    .Bind(ImageButton.SourceProperty, static vm => vm.Layout8ImageSource,
                        static (LayoutPickerModel vm, ImageSource? imageSource) => vm.Layout8ImageSource = imageSource)
                    .Row(1).Column(3),
                MakeLabel(7, model, 1, 3),

                new Image()
                    .Margin(10)
                    .Bind(ImageButton.SourceProperty, static vm => vm.Layout9ImageSource,
                        static (LayoutPickerModel vm, ImageSource? imageSource) => vm.Layout9ImageSource = imageSource)
                    .Row(2).Column(0),
                MakeLabel(8, model, 2, 0),

                new Image()
                    .Margin(10)
                    .Bind(ImageButton.SourceProperty, static vm => vm.Layout10ImageSource,
                        static (LayoutPickerModel vm, ImageSource? imageSource) => vm.Layout10ImageSource = imageSource)
                    .Row(2).Column(1),
                MakeLabel(9, model, 2, 1),

                new Image()
                    .Margin(10)
                    .Bind(ImageButton.SourceProperty, static vm => vm.Layout11ImageSource,
                        static (LayoutPickerModel vm, ImageSource? imageSource) => vm.Layout11ImageSource = imageSource)
                    .Row(2).Column(2),
                MakeLabel(10, model, 2, 2),

                new Image()
                    .Margin(10)
                    .Bind(ImageButton.SourceProperty, static vm => vm.Layout12ImageSource,
                        static (LayoutPickerModel vm, ImageSource? imageSource) => vm.Layout12ImageSource = imageSource)
                    .Row(2).Column(3),
                MakeLabel(11, model, 2, 3),
            }
        }.BindDynamicTheme();

        this.BindDynamicTheme();

        var windowBorder = new Border
        {
            StrokeThickness = 1,
            Padding = new Thickness(1),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5),
            },
            BackgroundColor = Colors.Transparent,
            Content = grid,
        }.BindDynamicTheme();

        Content = windowBorder;

        SharpHookHandler.AddKeyPressed(model.KeyPressed);
        model.Popup = this;
    }

    private static Label MakeLabel(int layoutNumber, LayoutPickerModel model, int row, int column)
    {
        return new Label
        {
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            GestureRecognizers =
            {
                new TapGestureRecognizer { Command = new Command(() => model.Close(layoutNumber)) },
                new PointerGestureRecognizer
                {
                    PointerEnteredCommand = new Command(() => model.MouseOverLayout(layoutNumber)),
                    PointerExitedCommand = new Command(() => model.MouseOutLayout(layoutNumber)),
                }
            },
            Shadow = new Shadow
            {
                Offset = new Point(2, 2),
                Opacity = 0.5f,
                Radius = 5
            }
        }
        .Text($"{layoutNumber + 1}")
        .FontSize(50)
        .TextColor(Colors.White)
        .Row(row).Column(column);
    }
}