using System;
using System.Windows.Media;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Shapes;

namespace Laba5._1
{
    public class Horse : INotifyPropertyChanged
    {
        private static readonly Random random = new Random();

        public string Name { get; private set; }

        public Brush Color { get; private set; }
        public Rectangle Shape { get; set; }

        public double Speed { get; private set; }

        // позиція на трасі
        private double x;
        public double X
        {
            get => x;
            set
            {
                x = value;
                OnPropertyChanged(nameof(X));
            }
        }

        private TimeSpan finishTime;

        public TimeSpan FinishTime
        {
            get => finishTime;
            set
            {
                finishTime = value;
                OnPropertyChanged(nameof(FinishTime));
            }
        }

        public Horse(string name, Brush color)
        {
            Name = name;
            Color = color;

            Speed = random.Next(5, 11);
        }

        // поточне прискорення
        private double acceleration;

        public double Acceleration
        {
            get => acceleration;
            set
            {
                acceleration = value;
                OnPropertyChanged(nameof(Acceleration));
            }
        }

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

        private int position;

        public int Position
        {
            get => position;
            set
            {
                position = value;
                OnPropertyChanged(nameof(Position));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}
   