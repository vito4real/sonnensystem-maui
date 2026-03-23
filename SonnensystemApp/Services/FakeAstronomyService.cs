using Microsoft.Maui.Graphics;
using SonnensystemApp.Models;

namespace SonnensystemApp.Services
{
    public class FakeAstronomyService
    {
        public List<PlanetPosition> GetPlanetPositions(DateTime utcNow)
        {
            double t = (utcNow - new DateTime(2000, 1, 1)).TotalDays;

            return new List<PlanetPosition>
            {
                CreatePlanet("Mercury",  45, 88,    t, 4, Colors.LightGray),
                CreatePlanet("Venus",    75, 225,   t, 5, Colors.Orange),
                CreatePlanet("Earth",   105, 365,   t, 6, Colors.DeepSkyBlue),
                CreatePlanet("Mars",    140, 687,   t, 5, Colors.IndianRed),
                CreatePlanet("Jupiter", 190, 4333,  t, 9, Colors.BurlyWood),
                CreatePlanet("Saturn",  240, 10759, t, 8, Colors.Khaki),
                CreatePlanet("Uranus",  285, 30687, t, 7, Colors.LightBlue),
                CreatePlanet("Neptune", 330, 60190, t, 7, Colors.CornflowerBlue)
            };
        }

        private PlanetPosition CreatePlanet(
            string name,
            double orbitRadius,
            double periodDays,
            double totalDays,
            double radius,
            Color color)
        {
            double angle = 2 * Math.PI * (totalDays % periodDays) / periodDays;

            return new PlanetPosition
            {
                Name = name,
                OrbitRadius = orbitRadius,
                X = orbitRadius * Math.Cos(angle),
                Y = orbitRadius * Math.Sin(angle),
                Radius = radius,
                Color = color
            };
        }
    }
}