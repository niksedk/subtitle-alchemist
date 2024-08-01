using CommunityToolkit.Maui.Markup;

namespace SubtitleAlchemist.Views.Main
{
    public static class InitSubtitleListView
    {
        public static CollectionView MakeSubtitleListView(MainViewModel vm)
        {
            var view = new CollectionView
            {
                SelectionMode = SelectionMode.Single,
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Colors.DarkGray,
                Header = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new(50),
                        new(95),
                        new(95),
                        new(95),
                        new(GridLength.Star)
                    },
                    Children =
                    {
                        new BoxView { BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"], Margin = 1, ZIndex = -1 }.Column(0),
                        new Label { Text = "#", TextColor = (Color)Application.Current.Resources["TextColor"], Margin = 5 }.Column(0),
                        new BoxView { BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"], Margin = 1, ZIndex = -1 }.Column(1),
                        new Label { Text = "Start", TextColor = (Color)Application.Current.Resources["TextColor"], Margin = 5 }.Column(1),
                        new BoxView { BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"], Margin = 1, ZIndex = -1 }.Column(2),
                        new Label { Text = "End", TextColor = (Color)Application.Current.Resources["TextColor"], Margin = 5 }.Column(2),
                        new BoxView { BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"], Margin = 1, ZIndex = -1 }.Column(3),
                        new Label { Text = "Duration", TextColor =(Color)Application.Current.Resources["TextColor"], Margin = 5 }.Column(3),
                        new BoxView { BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"], Margin = 1, ZIndex = -1 }.Column(4),
                        new Label { Text = "Text", TextColor = (Color)Application.Current.Resources["TextColor"], Margin = 5 }.Column(4)
                    }
                },
                ItemTemplate = new DataTemplate(() =>
                    new Grid
                    {
                        ColumnDefinitions = new ColumnDefinitionCollection
                        {
                            new(50),
                            new(95),
                            new(95),
                            new(95),
                            new(GridLength.Star)
                        },
                        Children =
                        {
                            new BoxView
                            {
                                BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"], Margin = 1, ZIndex = -1
                            }.Column(0).Bind(VisualElement.BackgroundColorProperty, nameof(DisplayParagraph.BackgroundColor)),
                            new Label { TextColor =(Color)Application.Current.Resources["TextColor"], Margin = 5 }.Column(0).Bind("Number"),

                            new BoxView
                            {
                                BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"], Margin = 1, ZIndex = -1
                            }.Column(1).Bind(VisualElement.BackgroundColorProperty, nameof(DisplayParagraph.BackgroundColor)),
                            new Label { TextColor =(Color)Application.Current.Resources["TextColor"], Margin = 5 }.Column(1).Bind("StartTime"),

                            new BoxView
                            {
                                BackgroundColor =(Color)Application.Current.Resources["BackgroundColor"], Margin = 1, ZIndex = -1
                            }.Column(2).Bind(VisualElement.BackgroundColorProperty, nameof(DisplayParagraph.BackgroundColor)),
                            new Label { TextColor =(Color)Application.Current.Resources["TextColor"], Margin = 5 }.Column(2).Bind("EndTime"),

                            new BoxView
                            {
                                BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"], Margin = 1, ZIndex = -1
                            }.Column(3).Bind(VisualElement.BackgroundColorProperty, nameof(DisplayParagraph.BackgroundColor)),
                            new Label { TextColor = (Color)Application.Current.Resources["TextColor"], Margin = 5 }.Column(3).Bind("Duration"),

                            new BoxView
                            {
                                BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"], Margin = 1, ZIndex = -1
                            }.Column(4).Bind(VisualElement.BackgroundColorProperty, nameof(DisplayParagraph.BackgroundColor)),
                            new Label { TextColor = (Color)Application.Current.Resources["TextColor"], Margin = 5 }.Column(4).Bind("Text")
                        }
                    })
            };

            view.SetBinding(ItemsView.ItemsSourceProperty, "Paragraphs");
            view.SelectionChanged += vm.OnCollectionViewSelectionChanged;

            vm.SubtitleListViewContextMenu = new MenuFlyout();
            vm.SubtitleListViewContextMenuItems = new List<MenuFlyoutItem>
            {
                new MenuFlyoutItem { Text = "Delete x lines?", Command = vm.DeleteSelectedLinesCommand },
                new MenuFlyoutItem { Text = "Insert line before", Command = vm.InsertBeforeCommand },
                new MenuFlyoutItem { Text = "Insert line after", Command = vm.InsertAfterCommand },
                new MenuFlyoutSeparator(),
                new MenuFlyoutItem { Text = "Italic", Command = vm.ItalicCommand, KeyboardAccelerators =
                {
                    new KeyboardAccelerator
                    {
                        Modifiers = KeyboardAcceleratorModifiers.Ctrl,
                        Key = "I",
                    }
                }},
            };
            foreach (var item in vm.SubtitleListViewContextMenuItems)
            {
                vm.SubtitleListViewContextMenu.Add(item);
            }
            FlyoutBase.SetContextFlyout(view, vm.SubtitleListViewContextMenu);

            return view;
        }
    }
}
