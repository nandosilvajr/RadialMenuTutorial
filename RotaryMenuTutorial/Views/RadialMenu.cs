using RotaryMenuTutorial.Pages;
using RotaryMenuTutorial.ViewModel;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using System.Diagnostics;
using System.Reflection;
using MenuItem = RotaryMenuTutorial.Models.MenuItem;


namespace RotaryMenuTutorial.Views
{

    public sealed class RadialMenu : SKCanvasView
    {

        public List<long> touchIds = new List<long>();
        private int _borderWidth = 6;
        private int _selectedArcSegmentIndex = -1;
        private SKPath _circlePath = new SKPath();
        private List<SKPath> _arcSegments = new List<SKPath>();
        private List<SKRect> _menuItems = new List<SKRect>();

        public static readonly BindableProperty ItemSourceProperty = BindableProperty.Create(nameof(ItemSource),
            typeof(IReadOnlyList<MenuItem>),
            typeof(RadialMenu),
            new List<MenuItem>());

        public List<MenuItem> ItemSource
        {
            get => (List<MenuItem>)GetValue(ItemSourceProperty);
            set
            {
                SetValue(ItemSourceProperty, value);
            }
        }

        public static readonly BindableProperty ItemsSegmentProperty = BindableProperty.Create(nameof(ItemsSegment),
            typeof(List<SKPath>),
            typeof(RadialMenu),
            new List<SKPath>());

        public List<SKPath> ItemsSegment
        {
            get => (List<SKPath>)GetValue(ItemsSegmentProperty);
            set
            {
                SetValue(ItemsSegmentProperty, value);
            }
        }

        public static readonly BindableProperty DiameterProperty = BindableProperty.Create(nameof(Diameter),
            typeof(int),
            typeof(RadialMenu),
            50);

        public int Diameter
        {
            get => (int)GetValue(DiameterProperty);
            set => SetValue(DiameterProperty, value);
        }

        public static readonly BindableProperty IsEditableProperty = BindableProperty.Create(nameof(IsEditable), typeof(bool), typeof(RadialMenu), false);

        public bool IsEditable
        {
            get => (bool)GetValue(IsEditableProperty);
            set => SetValue(IsEditableProperty, value);
        }

        public static readonly BindableProperty IsLinearProperty = BindableProperty.Create(nameof(IsLinear), typeof(bool), typeof(RadialMenu), false);

        public bool IsLinear
        {
            get => (bool)GetValue(IsLinearProperty);
            set => SetValue(IsLinearProperty, value);
        }

        public static readonly BindableProperty ArcLengthProperty = BindableProperty.Create(nameof(ArcLength),
    typeof(int),
    typeof(RadialMenu),
    400);

        public int ArcLength
        {
            get => (int)GetValue(ArcLengthProperty);
            set => SetValue(ArcLengthProperty, value);
        }

        public static readonly BindableProperty NewItemProperty = BindableProperty.Create(nameof(NewItem),
            typeof(MenuItem),
            typeof(RadialMenu),
            null, 
            propertyChanged: OnEventNameChanged);

        public MenuItem NewItem
        {
            get => (MenuItem)GetValue(NewItemProperty);
            set => SetValue(NewItemProperty, value);
        }

        static void OnEventNameChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (RadialMenu)bindable;
            control.NewItem = (MenuItem)newValue;
            
}


        public RadialMenu()
        {
            EnableTouchEvents = true;

        }
        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);

            if (IsEditable)
                CreateEditableMenu(e);
            else
                CreateMenu(e);

        }

        public void CreateEditableMenu(SKPaintSurfaceEventArgs e)
        {

            Assembly assembly = GetType().GetTypeInfo().Assembly;
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            // check if item has a correspondent on list

            _selectedArcSegmentIndex = -1;

            var lastItem = ItemSource.Last();

            for (var i = 0; i < _arcSegments.Count; i++)
            {
                if (_arcSegments[i].Contains((float)lastItem.Location.X, (float)lastItem.Location.Y)
                    && !_circlePath.Contains((float)lastItem.Location.X, (float)lastItem.Location.Y))
                {

                    _selectedArcSegmentIndex = i;

                    _selectedArcSegmentIndex = i;

                    for (int j = 0; j < EditMenuViewModel.GetInstance().ItemSourceOption.Count; j++)
                    {
                        if (EditMenuViewModel.GetInstance().ItemSourceOption[j].Name == lastItem.Name)
                        {
                            MenuItem newNenuItem = new MenuItem(ItemSource[i].Name, ItemSource[i].Icon);

                            EditMenuViewModel.GetInstance().ItemSourceOption.RemoveAt(j);
                            EditMenuViewModel.GetInstance().ItemSourceOption.Insert(j, newNenuItem);

                            NewItem = newNenuItem;

                            // EditMenuViewModel.GetInstance().UpdateListOptions(newNenuItem);
                        }
                    }
                    ItemSource[i] = lastItem;

                    ItemSource.RemoveAt(ItemSource.Count - 1);
                }
            }




            #region Rotary Menu

            if (ItemSource.Count == 0)
            {
                throw new Exception("Missing menu items for radial wheel");
            }

            const int offset = 120;

            const int selectedArcPadding = 20;

            var sweepingAngle = 360 / ItemSource.Count;

            var borderPaint = new SKPaint
            {
                StrokeWidth = _borderWidth,
                Style = SKPaintStyle.Stroke,
                StrokeCap = SKStrokeCap.Round,
                Color = Colors.Transparent.ToSKColor()
            };

            _arcSegments.Clear();

            for (var i = 0; i < ItemSource.Count; i++)
            {
                var path = new SKPath();

                var degrees = i * sweepingAngle;

                var length = _selectedArcSegmentIndex == i
                    ? ArcLength + selectedArcPadding
                    : ArcLength;

                var startX = info.Rect.MidX + length * Math.Cos((degrees - offset) * (Math.PI / 180));
                var startY = info.Rect.MidY + length * Math.Sin((degrees - offset) * (Math.PI / 180));

                path.MoveTo(new SKPoint((float)startX, (float)startY));
                path.ArcTo(new SKRect(
                    info.Rect.MidX - length,
                    info.Rect.MidY - length,
                    info.Rect.MidX + length,
                    info.Rect.MidY + length
                ), degrees - offset, sweepingAngle, true);
                path.LineTo(new SKPoint(info.Rect.MidX, info.Rect.MidY));
                path.Close();

                _arcSegments.Add(path);
                ItemsSegment.Add(path);

                using (var textPaint = new SKPaint
                {
                    IsAntialias = true,
                    TextSize = 30,
                    TextAlign = SKTextAlign.Center
                })
                {
                    const float lengthRatio = 0.75f;

                    var iconX = (float)(info.Rect.MidX
                        + (ArcLength
                        * lengthRatio)
                        * Math.Cos((degrees + (sweepingAngle / 2) - offset)
                        * (Math.PI / 180)));

                    var iconY = (float)(info.Rect.MidY +
                        (ArcLength * lengthRatio)
                        * Math.Sin((degrees + (sweepingAngle / 2) - offset)
                        * (Math.PI / 180)))
                        //vertical text alignment offset
                        + (textPaint.TextSize / 2);
                    //canvas.DrawCircle(new SKPoint(iconX, iconY), 10, borderPaint);
                    using (Stream stream = assembly.GetManifestResourceStream($"{ItemSource[i].Icon}"))
                    {
                        SKBitmap bitmap = SKBitmap.Decode(stream);

                        var resizedMenu = bitmap.Resize(new SKImageInfo(200, 200), SKBitmapResizeMethod.Lanczos3);

                        var image = SKImage.FromBitmap(resizedMenu);

                        canvas.DrawImage(image, (float)iconX - 100, (float)iconY - 120, textPaint);
                    }

                    SKPaint circlePaint = new SKPaint
                    {
                        Color = SKColors.Tomato
                    };



                    // create the paint for the path
                    var pathStroke = new SKPaint
                    {
                        IsAntialias = true,
                        Style = SKPaintStyle.Stroke,
                        Color = SKColors.Black,
                        StrokeWidth = 5
                    };
                    if (!String.IsNullOrEmpty(ItemSource[i].Name))
                    {
                        canvas.DrawCircle((float)iconX + 50, (float)iconY - 100, 20, circlePaint);

                        // create a path to x badge
                        var pathX = new SKPath();
                        pathX.MoveTo((float)iconX + (float)41, (float)iconY - 90);
                        pathX.LineTo((float)iconX + (float)61, (float)iconY - 110);
                        pathX.MoveTo((float)iconX + (float)61, (float)iconY - 90);
                        pathX.LineTo((float)iconX + (float)41, (float)iconY - 110);

                        // draw the path
                        canvas.DrawPath(pathX, pathStroke);
                    }

                    canvas.DrawText($"{ItemSource[i].Name}", (float)iconX, (float)iconY + 100, textPaint);
                }
            }


            // Center Circle
            _circlePath.Reset();
            _circlePath.AddCircle(100, 100, Diameter);
            _circlePath.Close();

            canvas.DrawPath(_circlePath, borderPaint);
            var innerPaints = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = SKColor.Parse("#dedede"),
            };

            canvas.DrawCircle(
                new SKPoint(info.Rect.MidX, info.Rect.MidY),
                Diameter - (_borderWidth / 2), innerPaints);

            using (Stream stream = assembly.GetManifestResourceStream($"RotaryMenuTutorial.Resources.Images.dotnet_bot.png"))
            {
                SKBitmap bitmap = SKBitmap.Decode(stream);
                var resizedMenu = bitmap.Resize(new SKImageInfo(200, 200), SKBitmapResizeMethod.Lanczos3);
                var image = SKImage.FromBitmap(resizedMenu);
                // Define icons position on circle
                canvas.DrawImage(image, info.Rect.MidX - 100, info.Rect.MidY - 120);
            }

            using (var textPaint = new SKPaint
            {
                IsAntialias = true,
                TextSize = 30,
                TextAlign = SKTextAlign.Center
            })

                canvas.DrawText($"{ItemSource[0].Name}", info.Rect.MidX, info.Rect.MidY + 100, textPaint);


            #endregion
            canvas.Restore();
            // EditMenuPage.GetInstance().CreateOptionsMenu();
        }

        public void CreateMenu(SKPaintSurfaceEventArgs e)
        {
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            #region Rotary Menu

            // Get info assembly

            if (ItemSource.Count == 0)
            {
                throw new Exception("Missing menu items for radial wheel");
            }

            const int offset = 120;
            const int selectedArcPadding = 20;

            var sweepingAngle = 360 / ItemSource.Count;

            var borderPaint = new SKPaint
            {
                StrokeWidth = _borderWidth,
                Style = SKPaintStyle.Stroke,
                StrokeCap = SKStrokeCap.Round,
                Color = Colors.Transparent.ToSKColor()
            };

            _arcSegments.Clear();

            for (var i = 0; i < ItemSource.Count; i++)
            {
                if (i == 0)
                {
                    using (Stream stream = assembly.GetManifestResourceStream($"RotaryMenuTutorial.Resources.Images.dotnet_bot.png"))
                    {
                        SKBitmap bitmap = SKBitmap.Decode(stream);
                        var resizedMenu = bitmap.Resize(new SKImageInfo(200, 200), SKBitmapResizeMethod.Lanczos3);
                        var image = SKImage.FromBitmap(resizedMenu);

                        // Define icons position on circle
                        canvas.DrawImage(image, info.Rect.MidX - 100, info.Rect.MidY - 120);
                    }

                    using (var textPaint = new SKPaint
                    {
                        IsAntialias = true,
                        TextSize = 30,
                        TextAlign = SKTextAlign.Center
                    })
                        canvas.DrawText($"{ItemSource[i].Name}", info.Rect.MidX, info.Rect.MidY + 100, textPaint);

                }

                var path = new SKPath();

                var degrees = i * sweepingAngle;

                var length = _selectedArcSegmentIndex == i
                    ? ArcLength + selectedArcPadding
                    : ArcLength;

                var startX = info.Rect.MidX + length * Math.Cos((degrees - offset) * (Math.PI / 180));
                var startY = info.Rect.MidY + length * Math.Sin((degrees - offset) * (Math.PI / 180));

                path.MoveTo(new SKPoint((float)startX, (float)startY));
                path.ArcTo(new SKRect(
                    info.Rect.MidX - length,
                    info.Rect.MidY - length,
                    info.Rect.MidX + length,
                    info.Rect.MidY + length
                ), degrees - offset, sweepingAngle, true);
                path.LineTo(new SKPoint(info.Rect.MidX, info.Rect.MidY));
                path.Close();

                _arcSegments.Add(path);

                using (var textPaint = new SKPaint
                {
                    IsAntialias = true,
                    TextSize = 30,
                    TextAlign = SKTextAlign.Center
                })
                {
                    const float lengthRatio = 0.75f;

                    var iconX = (float)(info.Rect.MidX
                        + (ArcLength
                        * lengthRatio)
                        * Math.Cos((degrees + (sweepingAngle / 2) - offset)
                        * (Math.PI / 180)));

                    var iconY = (float)(info.Rect.MidY +
                        (ArcLength * lengthRatio)
                        * Math.Sin((degrees + (sweepingAngle / 2) - offset)
                        * (Math.PI / 180)))
                        //vertical text alignment offset
                        + (textPaint.TextSize / 2);
                    
                    using (Stream stream = assembly.GetManifestResourceStream($"{ItemSource[i].Icon}"))
                    {
                        SKBitmap bitmap = SKBitmap.Decode(stream);

                        var resizedMenu = bitmap.Resize(new SKImageInfo(200, 200), SKBitmapResizeMethod.Lanczos3);

                        var image = SKImage.FromBitmap(resizedMenu);

                        canvas.DrawImage(image, (float)iconX - 100, (float)iconY - 120, textPaint);
                    }

                    canvas.DrawText($"{ItemSource[i].Name}", (float)iconX, (float)iconY + 100, textPaint);
                }
            }


            // Center Circle
            _circlePath.Reset();
            _circlePath.AddCircle(100, 100, Diameter);
            _circlePath.Close();


            #endregion
            canvas.Restore();
        }
        protected async override void OnTouch(SKTouchEventArgs e)
        {
            base.OnTouch(e);

        }

        private async void TouchMenu(SKTouchEventArgs e)
        {
            Point pt = new Point
            {
                X = e.Location.X,
                Y = e.Location.Y
            };

            SKPoint point =
                new SKPoint((float)(this.CanvasSize.Width * pt.X / this.Width),
                            (float)(this.CanvasSize.Height * pt.Y / this.Height));

            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:

                    _selectedArcSegmentIndex = -1;

                    for (var i = 0; i < _arcSegments.Count; i++)
                    {
                        if (_arcSegments[i].Contains(e.Location.X, e.Location.Y)
                            && !_circlePath.Contains(e.Location.X, e.Location.Y))
                        {
                            _selectedArcSegmentIndex = i;

                            //await Navigation.PushAsync(new BuyRentPage());

                        }
                    }
                    InvalidateSurface();

                    break;
                case SKTouchAction.Moved:
                    break;
            }

            e.Handled = true;
        }

    }
}

