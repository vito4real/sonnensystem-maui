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

        private bool _ufoAnimationRunning = false;
        private double _ufoProgress = 0;

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

        private void Speed500Kx(object sender, EventArgs e)
        {
            _timeSpeed = 500_000;
        }

        private void Speed1Мx(object sender, EventArgs e)
        {
            _timeSpeed = 1_000_000;
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
        private void OnAlienQuestionClicked(object sender, EventArgs e)
        {
            if (_ufoAnimationRunning)
                return;

            StartUfoFlyBy();
        }

        private void StartUfoFlyBy()
        {
            _ufoAnimationRunning = true;
            _ufoProgress = 0;

            var width = _drawable.LastDirtyRect.Width;
            var height = _drawable.LastDirtyRect.Height;

            Dispatcher.StartTimer(TimeSpan.FromMilliseconds(16), () =>
            {
                if (!_ufoAnimationRunning)
                    return false;

                _ufoProgress += 0.015;

                if (_ufoProgress >= 1.0)
                {
                    _drawable.IsUfoVisible = false;
                    _ufoAnimationRunning = false;
                    SolarSystemCanvas.Invalidate();
                    return false;
                }

                // ✨ ПАРАМЕТРИЧЕСКАЯ ТРАЕКТОРИЯ (дуга)

                double t = _ufoProgress;

                // X: слева → вправо
                float x = (float)(-100 + t * (width + 200));

                // Y: красивая дуга (парабола)
                float y = (float)(
                    height * 0.6
                    - 150 * Math.Sin(t * Math.PI)
                );

                _drawable.UfoX = x;
                _drawable.UfoY = y;
                _drawable.IsUfoVisible = true;

                SolarSystemCanvas.Invalidate();
                return true;
            });
        }
    }
}