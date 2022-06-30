using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MenuItem = RotaryMenuTutorial.Models.MenuItem;


namespace RotaryMenuTutorial.ViewModel
{
    public class EditMenuViewModel : INotifyPropertyChanged
    {

        private ObservableCollection<MenuItem> _itemSourceOption;
        private List<MenuItem> _itemSource;

        public ObservableCollection<MenuItem> ItemSourceOption
        {
            get
            {
                return _itemSourceOption;
            }

            set
            {
                _itemSourceOption = value;
                OnPropertyChanged();
            }
        }
        public List<MenuItem> ItemSource
        {
            get
            {
                return _itemSource;
            }

            set
            {
                _itemSource = value;
                OnPropertyChanged();
            }
        }

        public MenuItem OldMenuItem { get; set; }
        public MenuItem NewMenuItem { get; set; }

        private static EditMenuViewModel instance;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static EditMenuViewModel GetInstance()
        {
            if (instance == null)
                return new EditMenuViewModel();

            return instance;
        }
      

        public Command GetItemSourceOptionCommand { get; }
        public Command GetItemSourceCommand { get; }

        public EditMenuViewModel()
        {
            instance = this;

            OldMenuItem = new MenuItem(string.Empty, String.Empty);
            NewMenuItem = new MenuItem(string.Empty, String.Empty);

            this.ItemSource = MainViewModel.GetInstance().ItemSource;

            if(ItemSourceOption == null)

            ItemSourceOption = new ObservableCollection<MenuItem>
            {
                new MenuItem("Option 1", "RotaryMenuTutorial.Resources.Images.dotnet_bot2.png"),
                new MenuItem("Option 2", "RotaryMenuTutorial.Resources.Images.dotnet_bot2.png"),
                new MenuItem("Option 3", "RotaryMenuTutorial.Resources.Images.dotnet_bot2.png"),
                new MenuItem("Option 4", "RotaryMenuTutorial.Resources.Images.dotnet_bot2.png"),
                new MenuItem("Option 5", "RotaryMenuTutorial.Resources.Images.dotnet_bot2.png"),
            };

        }
    }


}
