using RotaryMenuTutorial.ViewModel;
using MenuItem = RotaryMenuTutorial.Models.MenuItem;


namespace RotaryMenuTutorial.Pages;

public partial class EditMenuPage : ContentPage
{
    private static EditMenuPage instance;

    public static EditMenuPage GetInstance()
    {
        if (instance == null)
            return new EditMenuPage();

        return instance;
    }

    public EditMenuPage()
    {
        InitializeComponent();

        instance = this;

        BindingContext = new EditMenuViewModel();

    }

    public void UpdateList(MenuItem menuItem)
    {

        canvasView.ItemSource.Add(menuItem);
        canvasView.InvalidateSurface();

    }

    public MenuItem CheckSegment(MenuItem menuItem)
    {
        for (var i = 0; i < canvasView.ItemsSegment.Count; i++)
        {
            if (canvasView.ItemsSegment[i].Contains((float)menuItem.Location.X, (float)menuItem.Location.Y))
            {

                for (int j = 0; j < EditMenuViewModel.GetInstance().ItemSourceOption.Count; j++)
                {
                    if (EditMenuViewModel.GetInstance().ItemSourceOption[j].Name == menuItem.Name)
                    {
                        MenuItem newNenuItem = new MenuItem(canvasView.ItemSource[i].Name, canvasView.ItemSource[i].Icon);

                        return newNenuItem;
                    }
                }

            }
        }
        return null;
    }

}