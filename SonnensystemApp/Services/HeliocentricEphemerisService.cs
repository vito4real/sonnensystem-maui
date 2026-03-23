using static CosineKitty.Astronomy;
using CosineKitty;
using Microsoft.Maui.Graphics;
using SonnensystemApp.Models;

namespace SonnensystemApp.Services
{
    public class HeliocentricEphemerisService
    {
        public List<PlanetPosition> GetPlanetPositions(DateTime utcNow)
        {
            var time = new AstroTime(utcNow);

            return new List<PlanetPosition>
            {
                CreatePlanet(Body.Mercury, "Mercury", 4, Colors.LightGray, time),
                CreatePlanet(Body.Venus,   "Venus",   5, Colors.Orange, time),
                CreatePlanet(Body.Earth,   "Earth",   6, Colors.DeepSkyBlue, time),
                CreatePlanet(Body.Mars,    "Mars",    5, Colors.IndianRed, time),
                CreatePlanet(Body.Jupiter, "Jupiter", 9, Colors.BurlyWood, time),
                CreatePlanet(Body.Saturn,  "Saturn",  8, Colors.Khaki, time),
                CreatePlanet(Body.Uranus,  "Uranus",  7, Colors.LightBlue, time),
                CreatePlanet(Body.Neptune, "Neptune", 7, Colors.CornflowerBlue, time)
            };
        }

        private PlanetPosition CreatePlanet(
            Body body,
            string name,
            double radius,
            Color color,
            AstroTime time)
        {
            // Гелиоцентрический вектор планеты относительно Солнца
            var vec = Astronomy.HelioVector(body, time);

            // vec.x / vec.y измеряются в астрономических единицах (AU)
            // Для UI нам нужен визуальный scale
            const double scale = 170.0;

            double x = vec.x * scale;
            double y = vec.y * scale;

            double orbitRadius = Math.Sqrt(x * x + y * y);

            return new PlanetPosition
            {
                Name = name,
                OrbitRadius = orbitRadius,
                X = x,
                Y = y,
                Radius = radius,
                Color = color
            };
        }
    }
}