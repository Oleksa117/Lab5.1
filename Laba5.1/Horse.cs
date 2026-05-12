using System;
using System.Windows.Media;
using System.Threading.Tasks;

namespace Laba5._1
{
    public class Horse
    {
        private static readonly Random random = new Random();

        public string Name { get; private set; }

        public Brush Color { get; private set; }

        public double Speed { get; private set; }

        // позиція на трасі
        public double X { get; set; }

        public TimeSpan FinishTime { get; set; }

        public Horse(string name, Brush color)
        {
            Name = name;
            Color = color;

            Speed = random.Next(5, 11);
        }

        // поточне прискорення
        public double Acceleration { get; set; }

        // асинхронна зміна прискорення
        public async Task ChangeAcceleration()
        {
            await Task.Run(() =>
            {
                // випадкове число від 0.7 до 1
                double value = 0.7 + random.NextDouble() * 0.3;

                // розрахунок прискорення
                Acceleration = Speed * value;
            });
        }
    }
}