using RotaryMenuTutorial.ViewModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using MenuItem = RotaryMenuTutorial.Models.MenuItem;

namespace RotaryMenuTutorial.Views;

public partial class MenuOptionsCustom : ContentView
{

    public static readonly BindableProperty ItemSourceProperty = BindableProperty.Create(nameof(ItemSource),
         typeof(ObservableCollection<MenuItem>),
         typeof(MenuOptionsCustom),
         new ObservableCollection<MenuItem>(),
         propertyChanged: OnEventNameChanged);

    public ObservableCollection<MenuItem> ItemSource
    {
        get => (ObservableCollection<MenuItem>)GetValue(ItemSourceProperty);
        set => SetValue(ItemSourceProperty, value);
    }

    static void OnEventNameChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (MenuOptionsCustom)bindable;
        control.ItemSource = (ObservableCollection<MenuItem>)newValue;
    }


    public MenuOptionsCustom()
	{
		InitializeComponent();

        ItemSource = EditMenuViewModel.GetInstance().ItemSourceOption;

        List<PanContainer> panContainers = new List<PanContainer>();

        AbsoluteLayout views = new AbsoluteLayout();

        Assembly assembly = GetType().GetTypeInfo().Assembly;

        for (int i = 0; i < ItemSource.Count; i++)
        {

            Grid grid = new Grid();
            var gridSize = 100;
            grid.HorizontalOptions = LayoutOptions.Center;

            Stream stream = assembly.GetManifestResourceStream($"{ItemSource[i].Icon}");

            Image image = new Image();
            image.Source = ImageSource.FromStream(() => stream);
            image.HeightRequest = 60;
            image.WidthRequest = 60;

            Label label = new Label();

            label.VerticalOptions = LayoutOptions.End;
            label.WidthRequest = 100;
            label.LineBreakMode = LineBreakMode.WordWrap;
            label.HorizontalTextAlignment = TextAlignment.Center;
            label.Text = ItemSource[i].Name;
            label.TextColor = Colors.Black;

            Label labelHidden = new Label();

            labelHidden.VerticalOptions = LayoutOptions.End;
            labelHidden.WidthRequest = 100;
            labelHidden.LineBreakMode = LineBreakMode.WordWrap;
            labelHidden.HorizontalTextAlignment = TextAlignment.Center;
            labelHidden.Text = ItemSource[i].Icon;
            labelHidden.TextColor = Colors.Black;
            labelHidden.IsVisible = false;

            grid.Add(image);
            grid.Add(label);
            grid.Add(labelHidden);
            grid.HeightRequest = 100;
            grid.WidthRequest = 100;

            PanContainer panContainer = new PanContainer();

            panContainer.Content = new Grid
            {
                Children = { grid }
            };

            panContainer.HeightRequest = 100;

            var rect = new Rect(((double)i / ItemSource.Count) + 0.1, 1, gridSize - 50, gridSize);

            AbsoluteLayout.SetLayoutBounds(panContainer, rect);
            AbsoluteLayout.SetLayoutFlags(panContainer, Microsoft.Maui.Layouts.AbsoluteLayoutFlags.PositionProportional);

            menuOptionsLayout.Add(panContainer);
        };

    }
}