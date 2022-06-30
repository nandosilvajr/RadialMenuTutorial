using RotaryMenuTutorial.Pages;
using RotaryMenuTutorial.ViewModel;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MenuItem = RotaryMenuTutorial.Models.MenuItem;


namespace RotaryMenuTutorial.Views
{
    public class PanContainer : ContentView
    {

        private double x, y;
        public PanContainer()
        {
            var panGesture = new PanGestureRecognizer();

            panGesture.PanUpdated += OnPanUpdated;
            this.GestureRecognizers.Add(panGesture);
        }

        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            float scale = (float)DeviceDisplay.Current.MainDisplayInfo.Density;

            var obj = sender as ContentView;
            var getObjectPositionX = obj.X;
            var getObjectPositionY = obj.Y;

            switch (e.StatusType)
            {
                case GestureStatus.Started:

                    break;
                case GestureStatus.Running:
                    Content.TranslationX = x + e.TotalX;
                    Content.TranslationY = y + e.TotalY;

                    break;
                case GestureStatus.Completed:
                    x = Content.TranslationX;
                    y = Content.TranslationY;

                    SKPoint sKPoint = new SKPoint
                    {
                        X = ((float)x + 50 + (float)getObjectPositionX) * scale,
                        Y = ((((((float)y - 50) 
                            - (float)Content.Height) * -scale) 
                            - (float)DeviceDisplay.Current.MainDisplayInfo.Height) 
                            + (float)getObjectPositionY) * -1
                    };

                    var getContainter = Content as Grid;

                    var getChild = getContainter.Children[0] as Grid;

                    var getImage = getChild.Children[0] as Image;

                    var getLabel = getChild.Children[1] as Label;

                    var getHideLabel = getChild.Children[2] as Label;

                    MenuItem menuItem = new MenuItem(getLabel.Text, getHideLabel.Text, new Point { X = sKPoint.X, Y = sKPoint.Y });

                    var getOldItem = EditMenuPage.GetInstance().CheckSegment(menuItem);

                    if (getOldItem != null)
                    {
                        EditMenuPage.GetInstance().UpdateList(menuItem);

                        getLabel.Text = getOldItem.Name;

                        getHideLabel.Text = getOldItem.Icon;

                        Assembly assembly = GetType().GetTypeInfo().Assembly;

                        Stream stream = assembly.GetManifestResourceStream($"{getOldItem.Icon}");

                        getImage.Source = ImageSource.FromStream(() => stream);
                    }

                    x = 0;

                    y = 0;

                    Content.TranslateTo(0, 0, 1000, Easing.BounceOut);

                    break;
            }

        }
    }
}
