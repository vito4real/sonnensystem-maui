using Microsoft.Maui.Graphics;

namespace SonnensystemApp.Models
{
    public class PlanetPosition
    {
        public string Name { get; set; } = string.Empty;

        public double OrbitRadius { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public double Radius { get; set; }

        public Color Color { get; set; } = Colors.White;
    }
}