using SonnensystemApp.Graphics;
using SonnensystemApp.Services;

namespace SonnensystemApp
{
    public partial class MainPage : ContentPage
    {
        private readonly SolarSystemDrawable _drawable;
        private readonly FakeAstronomyService _astronomyService;

        public MainPage()
        {
            InitializeComponent();

            _drawable = new SolarSystemDrawable();
            _astronomyService = new FakeAstronomyService();

            SolarSystemCanvas.Drawable = _drawable;

            StartSimulation();
        }

        private void StartSimulation()
        {
            UpdateScene();

            Dispatcher.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                UpdateScene();
                return true;
            });
        }

        private void UpdateScene()
        {
            var nowUtc = DateTime.UtcNow;

            _drawable.CurrentTimeUtc = nowUtc;
            _drawable.Planets = _astronomyService.GetPlanetPositions(nowUtc);

            SolarSystemCanvas.Invalidate();
        }
    }
}