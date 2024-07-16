using CommunityToolkit.Maui.Markup;

namespace SubtitleAlchemist.Views.Main
{
    public static class InitSubtitleListView
    {
        public static CollectionView MakeSubtitleListView(MainViewModel vm)
        {
            var view = new CollectionView
            {
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
                            }.Column(0),
                            new Label { TextColor =(Color)Application.Current.Resources["TextColor"], Margin = 5 }.Column(0).Bind("Number"),

                            new BoxView
                            {
                                BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"], Margin = 1, ZIndex = -1
                            }.Column(1),
                            new Label { TextColor =(Color)Application.Current.Resources["TextColor"], Margin = 5 }.Column(1).Bind("StartTime"),

                            new BoxView
                            {
                                BackgroundColor =(Color)Application.Current.Resources["BackgroundColor"], Margin = 1, ZIndex = -1
                            }.Column(2),
                            new Label { TextColor =(Color)Application.Current.Resources["TextColor"], Margin = 5 }.Column(2).Bind("EndTime"),

                            new BoxView
                            {
                                BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"], Margin = 1, ZIndex = -1
                            }.Column(3),
                            new Label { TextColor = (Color)Application.Current.Resources["TextColor"], Margin = 5 }.Column(3).Bind("Duration"),

                            new BoxView
                            {
                                BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"], Margin = 1, ZIndex = -1
                            }.Column(4),
                            new Label { TextColor = (Color)Application.Current.Resources["TextColor"], Margin = 5 }.Column(4).Bind("Text")
                        }
                    })
            };

            view.SetBinding(ItemsView.ItemsSourceProperty, "Paragraphs");
            return view;
        }
    }
}
