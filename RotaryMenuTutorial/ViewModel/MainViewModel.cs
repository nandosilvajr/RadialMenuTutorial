using RotaryMenuTutorial.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MenuItem = RotaryMenuTutorial.Models.MenuItem;

namespace RotaryMenuTutorial.ViewModel
{
    public class MainViewModel
    {
        private static MainViewModel instance;

        public static MainViewModel GetInstance()
        {
            if(instance == null) return new MainViewModel();
            return instance;
        }

        public List<MenuItem> ItemSource { get; set; }

        public ICommand OpenEditMenuPageCommand
        {
            get
            {
                return new Command(OpenEditMenuPage);
            }
        }

        private void OpenEditMenuPage(object obj)
        {
            App.Current.MainPage.Navigation.PushAsync(new EditMenuPage());
        } 
        public MainViewModel()
        {
            instance = this; 
            ItemSource = new List<MenuItem>
            {
                new MenuItem("Item 1", "RotaryMenuTutorial.Resources.Images.dotnet_bot.png"),
                new MenuItem("Item 2", "RotaryMenuTutorial.Resources.Images.dotnet_bot.png"),
                new MenuItem("Item 3", "RotaryMenuTutorial.Resources.Images.dotnet_bot.png"),
                new MenuItem("Item 4", "RotaryMenuTutorial.Resources.Images.dotnet_bot.png"),
                new MenuItem("Item 5", "RotaryMenuTutorial.Resources.Images.dotnet_bot.png"),
            };
        }
    }
}
