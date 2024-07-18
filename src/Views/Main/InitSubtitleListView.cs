using CommunityToolkit.Maui.Markup;
using static Microsoft.Maui.Controls.VisualStateManager;

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
         //   view.SetBinding(SelectableItemsView.SelectedItemsProperty, "SelectedParagraphs");
            view.SelectionChanged += vm.OnCollectionViewSelectionChanged;

            //// Make a Setter with property background color and value light sky blue
            //Setter backgroundColorSetter = new() { Property = VisualElement.BackgroundColorProperty, Value = Colors.LightSkyBlue };
            //VisualState stateSelected = new() { Name = CommonStates.Selected, Setters = { backgroundColorSetter } };
            //VisualState stateNormal = new() { Name = CommonStates.Normal };
            //VisualStateGroup visualStateGroup = new() { Name = nameof(CommonStates), States = { stateSelected, stateNormal } };
            //VisualStateGroupList visualStateGroupList = new() { visualStateGroup };
            //Setter vsgSetter = new() { Property = VisualStateGroupsProperty, Value = visualStateGroupList };
            //Style style = new(typeof(Grid)) { Setters = { vsgSetter } };

            //// Add the style to the resource dictionary
            //vm.MainPage.Resources.Add(style);


            return view;
        }
    }
}
