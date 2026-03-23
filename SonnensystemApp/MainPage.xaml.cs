using SonnensystemApp.Graphics;
using SonnensystemApp.Services;

#if WINDOWS
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Windows.Foundation;
#endif

namespace SonnensystemApp
{
    public partial class MainPage : ContentPage
    {
        private readonly SolarSystemDrawable _drawable;
        private readonly HeliocentricEphemerisService _astronomyService;

        private DateTime _simulatedTime = DateTime.UtcNow;

        private double _timeSpeed = 1.0; // 1x, 10x, 100x...

#if WINDOWS
        private bool _isDragging = false;
        private Windows.Foundation.Point _lastPointerPosition;
#endif

        public MainPage()
        {
            InitializeComponent();

            _drawable = new SolarSystemDrawable();
            _astronomyService = new HeliocentricEphemerisService();

            SolarSystemCanvas.Drawable = _drawable;

            SolarSystemCanvas.HandlerChanged += OnHandlerChanged;

            StartSimulation();
        }

        private void Speed1x(object sender, EventArgs e)
        {
            _timeSpeed = 1;
        }

        private void Speed10x(object sender, EventArgs e)
        {
            _timeSpeed = 10;
        }

        private void Speed100x(object sender, EventArgs e)
        {
            _timeSpeed = 100;
        }

        private void Speed1000x(object sender, EventArgs e)
        {
            _timeSpeed = 1000;
        }

        private void SpeedMega(object sender, EventArgs e)
        {
            _timeSpeed = 10_000_000;
        }

        private void StartSimulation()
        {
            UpdateScene();

            Dispatcher.StartTimer(TimeSpan.FromMilliseconds(50), () =>
            {
                UpdateScene();
                return true;
            });
        }

        private void UpdateScene()
        {
            var delta = TimeSpan.FromMilliseconds(50);

            _simulatedTime = _simulatedTime.AddSeconds(delta.TotalSeconds * _timeSpeed);

            _drawable.CurrentTimeUtc = _simulatedTime;
            _drawable.Planets = _astronomyService.GetPlanetPositions(_simulatedTime);

            SolarSystemCanvas.Invalidate();
        }

        private void OnHandlerChanged(object? sender, EventArgs e)
        {
#if WINDOWS
            var platformView = SolarSystemCanvas.Handler?.PlatformView as FrameworkElement;

            if (platformView != null)
            {
                platformView.PointerWheelChanged += OnPointerWheelChanged;
                platformView.PointerPressed += OnPointerPressed;
                platformView.PointerMoved += OnPointerMoved;
                platformView.PointerReleased += OnPointerReleased;
            }
#endif
        }

#if WINDOWS
        private void OnPointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            var delta = e.GetCurrentPoint((UIElement)sender).Properties.MouseWheelDelta;

            if (delta > 0)
                _drawable.Zoom *= 1.1f;
            else
                _drawable.Zoom /= 1.1f;

            _drawable.Zoom = Math.Clamp(_drawable.Zoom, 0.1f, 50f);

            SolarSystemCanvas.Invalidate();
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _isDragging = true;
            _lastPointerPosition = e.GetCurrentPoint((UIElement)sender).Position;

            ((UIElement)sender).CapturePointer(e.Pointer);
        }

        private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!_isDragging)
                return;

            var currentPosition = e.GetCurrentPoint((UIElement)sender).Position;

            float dx = (float)(currentPosition.X - _lastPointerPosition.X);
            float dy = (float)(currentPosition.Y - _lastPointerPosition.Y);

            _drawable.OffsetX += dx;
            _drawable.OffsetY += dy;

            _lastPointerPosition = currentPosition;

            SolarSystemCanvas.Invalidate();
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _isDragging = false;

            ((UIElement)sender).ReleasePointerCapture(e.Pointer);
        }
#endif
    }
}