using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Video.TextToSpeech;

public sealed class ElevenLabSettingsPopup : Popup
{
    public ElevenLabSettingsPopup(ElevenLabSettingsPopupModel vm)
    {
        BindingContext = vm;

        this.BindDynamicTheme();
        CanBeDismissedByTappingOutsideOfPopup = false;

        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
            },
            Margin = new Thickness(2),
            Padding = new Thickness(30, 20, 30, 10),
            RowSpacing = 20,
            ColumnSpacing = 10,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            WidthRequest = 350,
            HeightRequest = 300,
        }.BindDynamicTheme();

        var titleLabel = new Label
        {
            Text = "ElevenLabs settings",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
            Margin = new Thickness(0, 0, 0, 35),
        }.BindDynamicTheme();
        grid.Add(titleLabel, 0);
        grid.SetColumnSpan(titleLabel, 3);


        var labelStability = new Label
        {
            Text = "Stability",
            FontAttributes = FontAttributes.Bold,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelStability, 0, 1);

        var sliderStability = new Slider
        {
            Minimum = 0.0,
            Maximum = 1.0,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            WidthRequest = 150,
        };
        sliderStability.SetBinding(Slider.ValueProperty, nameof(vm.Stability));
        grid.Add(sliderStability, 1, 1);

        var imageStability = new Image
        {
            Source = "theme_dark_question.png",
            WidthRequest = 25,
            HeightRequest = 25,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
        };
        ToolTipProperties.SetText(imageStability, "The stability slider determines how stable the voice is and the randomness between each generation. Lowering this slider introduces a broader emotional range for the voice. As mentioned before, this is also influenced heavily by the original voice. Setting the slider too low may result in odd performances that are overly random and cause the character to speak too quickly. On the other hand, setting it too high can lead to a monotonous voice with limited emotion.");
        grid.Add(imageStability, 2, 1);

        var labelSimilarity = new Label
        {
            Text = "Similarity",
            FontAttributes = FontAttributes.Bold,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        grid.Add(labelSimilarity, 0, 2);

        var sliderSimilarity = new Slider
        {
            Minimum = 0.0,
            Maximum = 1.0,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            WidthRequest = 150,
        };
        sliderSimilarity.SetBinding(Slider.ValueProperty, nameof(vm.Similarity));
        grid.Add(sliderSimilarity, 1, 2);

        var imageSimilarity = new Image
        {
            Source = "theme_dark_question.png",
            WidthRequest = 25,
            HeightRequest = 25,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
        };
        ToolTipProperties.SetText(imageSimilarity, "The similarity slider dictates how closely the AI should adhere to the original voice when attempting to replicate it. If the original audio is of poor quality and the similarity slider is set too high, the AI may reproduce artifacts or background noise when trying to mimic the voice if those were present in the original recording.");
        grid.Add(imageSimilarity, 2, 2);


        var labelSpeakerBoost = new Label
        {
            Text = "Speaker Boost",
            FontAttributes = FontAttributes.Bold,
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();

        grid.Add(labelSpeakerBoost, 0, 3);

        var sliderSpeakerBoost = new Slider
        {
            Minimum = 0.0,
            Maximum = 1.0,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            WidthRequest = 150,
        };
        sliderSpeakerBoost.SetBinding(Slider.ValueProperty, nameof(vm.SpeakerBoost));
        grid.Add(sliderSpeakerBoost, 1, 3);

        var imageSpeakerBoost = new Image
        {
            Source = "theme_dark_question.png",
            WidthRequest = 25,
            HeightRequest = 25,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
        };
        ToolTipProperties.SetText(imageSpeakerBoost, "The speaker boost slider determines how much the AI should try to boost the voice of the speaker. This can be useful if the original audio is of poor quality or if the speaker's voice is too quiet. However, setting this slider too high can lead to the AI boosting background noise or artifacts present in the original recording.");
        grid.Add(imageSpeakerBoost, 2, 3);



        var buttonOk = new Button
        {
            Text = "OK",
            HorizontalOptions = LayoutOptions.Center,
            Command = vm.OkCommand,
            Margin = new Thickness(0, 0, 10, 0),
        }.BindDynamicTheme();

        var cancelButton = new Button
        {
            Text = "Cancel",
            HorizontalOptions = LayoutOptions.Center,
            Command = vm.CancelCommand,
        }.BindDynamicTheme();

        var buttonBar = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Start,
            Margin = new Thickness(0,25,0,0),
            Children =
            {
                buttonOk,
                cancelButton,
            },
        }.BindDynamicTheme();
        grid.Add(buttonBar, 0, 3);
        grid.SetColumnSpan(buttonBar, 3);


        var border = new Border
        {
            StrokeThickness = 1,
            Padding = new Thickness(4, 1, 1, 0),
            Margin = new Thickness(2),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(5)
            },
            Content = grid,
        }.BindDynamicTheme();

        Content = border;

        vm.Popup = this;
    }
}