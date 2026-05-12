using System;
using System.Windows.Media;

namespace Laba5._1
{
    public class Horse
    {
        // ім'я коня
        public string Name { get; private set; }

        // колір коня
        public Brush Color { get; private set; }

        // базова швидкість
        public double Speed { get; private set; }

        // позиція на трасі
        public double X { get; set; }

        // час фінішу
        public TimeSpan FinishTime { get; set; }

        public Horse(string name, Brush color)
        {
            Name = name;
            Color = color;
        }
    }
}