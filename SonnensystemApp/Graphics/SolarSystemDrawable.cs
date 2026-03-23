using Microsoft.Maui.Graphics;
using SonnensystemApp.Models;

namespace SonnensystemApp.Graphics
{
    public class SolarSystemDrawable : IDrawable
    {
        private readonly List<(float X, float Y, float Size)> _stars;

        public List<PlanetPosition> Planets { get; set; } = new();

        public DateTime CurrentTimeUtc { get; set; } = DateTime.UtcNow;

        public SolarSystemDrawable()
        {
            var random = new Random(42);
            _stars = new List<(float X, float Y, float Size)>();

            for (int i = 0; i < 120; i++)
            {
                _stars.Add((
                    X: random.Next(0, 1000),
                    Y: random.Next(0, 1000),
                    Size: random.Next(1, 3)
                ));
            }
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            DrawBackground(canvas, dirtyRect);
            DrawStars(canvas, dirtyRect);
            DrawTitle(canvas, dirtyRect);
            DrawSolarSystem(canvas, dirtyRect);
        }

        private void DrawBackground(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Color.FromArgb("#050816");
            canvas.FillRectangle(dirtyRect);
        }

        private void DrawStars(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Colors.White;

            foreach (var star in _stars)
            {
                float x = star.X / 1000f * dirtyRect.Width;
                float y = star.Y / 1000f * dirtyRect.Height;
                canvas.FillCircle(x, y, star.Size);
            }
        }

        private void DrawTitle(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FontColor = Colors.White;
            canvas.FontSize = 22;
            canvas.DrawString(
                "Sonnensystem",
                20,
                20,
                dirtyRect.Width - 40,
                30,
                HorizontalAlignment.Left,
                VerticalAlignment.Center);

            canvas.FontColor = Colors.LightGray;
            canvas.FontSize = 14;
            canvas.DrawString(
                $"UTC: {CurrentTimeUtc:yyyy-MM-dd HH:mm:ss}",
                20,
                50,
                dirtyRect.Width - 40,
                22,
                HorizontalAlignment.Left,
                VerticalAlignment.Center);
        }

        private void DrawSolarSystem(ICanvas canvas, RectF dirtyRect)
        {
            float centerX = dirtyRect.Center.X;
            float centerY = dirtyRect.Center.Y + 30;

            DrawOrbits(canvas, centerX, centerY);
            DrawSun(canvas, centerX, centerY);
            DrawPlanets(canvas, centerX, centerY);
        }

        private void DrawOrbits(ICanvas canvas, float centerX, float centerY)
        {
            canvas.StrokeColor = Color.FromArgb("#2B3553");
            canvas.StrokeSize = 1;

            foreach (var planet in Planets)
            {
                canvas.DrawCircle(centerX, centerY, (float)planet.OrbitRadius);
            }
        }

        private void DrawSun(ICanvas canvas, float centerX, float centerY)
        {
            canvas.FillColor = Colors.Gold;
            canvas.FillCircle(centerX, centerY, 16);

            canvas.StrokeColor = Colors.Orange;
            canvas.StrokeSize = 2;
            canvas.DrawCircle(centerX, centerY, 20);
        }

        private void DrawPlanets(ICanvas canvas, float centerX, float centerY)
        {
            foreach (var planet in Planets)
            {
                float x = centerX + (float)planet.X;
                float y = centerY + (float)planet.Y;

                canvas.FillColor = planet.Color;
                canvas.FillCircle(x, y, (float)planet.Radius);

                canvas.FontColor = Colors.White;
                canvas.FontSize = 12;
                canvas.DrawString(
                    planet.Name,
                    x + 8,
                    y - 8,
                    90,
                    20,
                    HorizontalAlignment.Left,
                    VerticalAlignment.Center);
            }
        }
    }
}