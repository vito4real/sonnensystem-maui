using CosineKitty;
using Microsoft.Maui.Graphics;
using SonnensystemApp.Models;

namespace SonnensystemApp.Graphics
{
    public class SolarSystemDrawable : IDrawable
    {
        private readonly List<(float X, float Y, float Size)> _stars;

        private const float SunRadius = 14f;
        private const float StarParallaxFactor = 0.2f;

        public List<PlanetPosition> Planets { get; set; } = new();

        public DateTime CurrentTimeUtc { get; set; } = DateTime.UtcNow;

        public float Zoom { get; set; } = 1.0f;

        public float OffsetX { get; set; } = 0f;
        public float OffsetY { get; set; } = 0f;

        public bool IsUfoVisible { get; set; }
        public float UfoX { get; set; }
        public float UfoY { get; set; }

        public RectF LastDirtyRect { get; private set; }

        public SolarSystemDrawable()
        {
            var random = new Random(42);
            _stars = new List<(float X, float Y, float Size)>();

            for (int i = 0; i < 140; i++)
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
            LastDirtyRect = dirtyRect;

            DrawBackground(canvas, dirtyRect);
            DrawStars(canvas, dirtyRect);
            DrawTitle(canvas, dirtyRect);
            DrawSolarSystem(canvas, dirtyRect);
            DrawUfo(canvas);
        }

        private void DrawBackground(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Color.FromArgb("#050816");
            canvas.FillRectangle(dirtyRect);
        }

        private void DrawStars(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Colors.White;

            float parallaxX = OffsetX * StarParallaxFactor;
            float parallaxY = OffsetY * StarParallaxFactor;

            foreach (var star in _stars)
            {
                float baseX = star.X / 1000f * dirtyRect.Width;
                float baseY = star.Y / 1000f * dirtyRect.Height;

                float x = baseX + parallaxX;
                float y = baseY + parallaxY;

                if (x < -5) x += dirtyRect.Width + 10;
                if (x > dirtyRect.Width + 5) x -= dirtyRect.Width + 10;
                if (y < -5) y += dirtyRect.Height + 10;
                if (y > dirtyRect.Height + 5) y -= dirtyRect.Height + 10;

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
            float centerX = dirtyRect.Center.X + OffsetX;
            float centerY = dirtyRect.Center.Y + 30 + OffsetY;

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
                var orbitPoints = BuildOrbitPoints(planet, CurrentTimeUtc);

                for (int i = 1; i < orbitPoints.Count; i++)
                {
                    var p1 = orbitPoints[i - 1];
                    var p2 = orbitPoints[i];

                    canvas.DrawLine(
                        centerX + p1.X,
                        centerY + p1.Y,
                        centerX + p2.X,
                        centerY + p2.Y
                    );
                }
            }
        }

        private void DrawSun(ICanvas canvas, float centerX, float centerY)
        {
            canvas.FillColor = Colors.Gold;
            canvas.FillCircle(centerX, centerY, SunRadius);

            canvas.StrokeColor = Colors.Orange;
            canvas.StrokeSize = 2;
            canvas.DrawCircle(centerX, centerY, SunRadius + 4);
        }

        private void DrawPlanets(ICanvas canvas, float centerX, float centerY)
        {
            foreach (var planet in Planets)
            {
                float x = centerX + (float)(planet.X * Zoom);
                float y = centerY + (float)(planet.Y * Zoom);

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

        private List<PointF> BuildOrbitPoints(PlanetPosition planet, DateTime centerTime)
        {
            var points = new List<PointF>();

            int steps = 200;

            // Umlaufzeit in Tagen (Jahresdauer)
            double periodDays = planet.Name switch
            {
                "Merkur" => 88,
                "Venus" => 225,
                "Erde" => 365,
                "Mars" => 687,
                "Jupiter" => 4333,
                "Saturn" => 10759,
                "Uranus" => 30687,
                "Neptun" => 60190,
                _ => 365
            };

            var halfPeriod = TimeSpan.FromDays(periodDays / 2);

            for (int i = 0; i < steps; i++)
            {
                double t = (double)i / (steps - 1);
                var time = centerTime - halfPeriod + TimeSpan.FromDays(periodDays * t);

                // Mapping deutscher Name -> Astronomy Body
                var body = planet.Name switch
                {
                    "Merkur" => Body.Mercury,
                    "Venus" => Body.Venus,
                    "Erde" => Body.Earth,
                    "Mars" => Body.Mars,
                    "Jupiter" => Body.Jupiter,
                    "Saturn" => Body.Saturn,
                    "Uranus" => Body.Uranus,
                    "Neptun" => Body.Neptune,
                    _ => Body.Earth
                };

                var vec = Astronomy.HelioVector(body, new AstroTime(time));

                float x = (float)(vec.x * Zoom * 170);
                float y = (float)(vec.y * Zoom * 170);

                points.Add(new PointF(x, y));
            }

            return points;
        }

        private void DrawUfo(ICanvas canvas)
        {
            if (!IsUfoVisible)
                return;

            canvas.FillColor = Colors.Silver;
            canvas.FillEllipse(UfoX - 18, UfoY - 6, 36, 12);

            canvas.FillColor = Colors.LightBlue;
            canvas.FillEllipse(UfoX - 8, UfoY - 14, 16, 10);

            canvas.StrokeColor = Colors.White;
            canvas.StrokeSize = 1;
            canvas.DrawEllipse(UfoX - 18, UfoY - 6, 36, 12);
        }
    }
}