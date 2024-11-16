using SubtitleAlchemist.Logic;

namespace SubtitleAlchemist.Features.Tools.ChangeCasing;

public class ChangeCasingPage : ContentPage
{
    public ChangeCasingPage(ChangeCasingPageModel vm)
    {
        BindingContext = vm;

        var stack = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            Padding = new Thickness(40),
        };

        var labelTitle = new Label
        {
            Text = "Change Casing to",
            FontAttributes = FontAttributes.Bold,
            Margin = new Thickness(0, 0, 0, 25),
        }.AsTitle();
        stack.Children.Add(labelTitle);

        var radioNormalCasing = new RadioButton
        {
            Content = "Normal Casing",
            GroupName = "Casing",
            IsChecked = true,
            Margin = new Thickness(0, 0, 0, 0),
            Padding = new Thickness(0, 0, 0, 0),
            FontAttributes = FontAttributes.Bold,
        }.BindIsChecked(nameof(vm.ToNormalCasing));
        stack.Children.Add(radioNormalCasing);

        var labelFixNames = new Label
        {
            Text = "Fix names.",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        var switchFixNames = new Switch
        {
            IsToggled = true,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(5,0,0,0),
        }.BindDynamicTheme().BindToggledProperty(nameof(vm.FixNames));
        var stackFixNames = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Children =
            {
                labelFixNames,
                switchFixNames,
            },
            Margin = new Thickness(20,0,0,0),
        }.BindDynamicTheme();
        stack.Children.Add(stackFixNames);

        var labelOnlyChangeAllUppercaseLines = new Label
        {
            Text = "Only change all uppercase lines.",
            VerticalOptions = LayoutOptions.Center,
        }.BindDynamicTheme();
        var switchOnlyChangeAllUppercaseLines = new Switch
        {
            IsToggled = true,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(5, 0, 0, 0),
        }.BindDynamicTheme().BindToggledProperty(nameof(vm.OnlyChangeAllUppercaseLines));
        var stackOnlyChangeAllUppercaseLines = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Children =
            {
                labelOnlyChangeAllUppercaseLines,
                switchOnlyChangeAllUppercaseLines,
            },
            Margin = new Thickness(20, 0, 0, 20),
        }.BindDynamicTheme();
        stack.Children.Add(stackOnlyChangeAllUppercaseLines);

        var radioFixOnlyNames = new RadioButton
        {
            Content = "Same casing, but fix names",
            GroupName = "Casing",
            Margin = new Thickness(0, 0, 0, 20),
            FontAttributes = FontAttributes.Bold,
        }.BindIsChecked(nameof(vm.FixOnlyNames));
        stack.Children.Add(radioFixOnlyNames);
        
        var radioFixUppercaseLines = new RadioButton
        {
            Content = "ALL UPPERCASE",
            GroupName = "Casing",
            Margin = new Thickness(0, 0, 0, 20),
            FontAttributes = FontAttributes.Bold,
        }.BindIsChecked(nameof(vm.ToUppercase));
        stack.Children.Add(radioFixUppercaseLines);

        var radioFixLowercaseLines = new RadioButton
        {
            Content = "all lowercase",
            GroupName = "Casing",
            Margin = new Thickness(0, 0, 0, 20),
            FontAttributes = FontAttributes.Bold,
        }.BindIsChecked(nameof(vm.ToLowercase));
        stack.Children.Add(radioFixLowercaseLines);

        var buttonOk = new Button
        {
            Text = "OK",
            Margin = new Thickness(0, 30, 0, 0),
            Command = vm.OkCommand,
        }.BindDynamicTheme();
        var buttonCancel = new Button
        {
            Text = "Cancel",
            Margin = new Thickness(10, 30, 0, 0),
            Command = vm.CancelCommand,
        }.BindDynamicTheme();
        var stackButtons = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Children =
            {
                buttonOk,
                buttonCancel,
            },
        };
        stack.Children.Add(stackButtons);

        Content = stack;

        this.BindDynamicTheme();

        vm.Page = this;
    }
    
}
