using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Shapes;

namespace Laba5._1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // список усіх коней
        public ObservableCollection<Horse> Horses { get; set; } = new ObservableCollection<Horse>();

        public MainWindow()
        {
            InitializeComponent();
        }


        private void CreateHorses(int count)
        {
            Horses.Clear();

            Brush[] colors = { Brushes.Red, Brushes.Blue, Brushes.Yellow, Brushes.Black };

            for (int i = 0; i < count; i++)
            {
                Horse horse = new Horse($"Horse {i + 1}", colors[i]);

                Horses.Add(horse);

                // прямокутник який представляє коня
                Rectangle rect = new Rectangle
                {
                    Width = 40,
                    Height = 40,
                    Fill = horse.Color
                };

                // позиція коня на трасі
                Canvas.SetLeft(rect, 0);
                Canvas.SetTop(rect, 50 + i * 60);

                RaceCanvas.Children.Add(rect);
            }
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            int count = int.Parse(((ListBoxItem)HorseCountListBox.SelectedItem).Content.ToString());

            CreateHorses(count);

            // гонка триває поки всі не дійдуть до фінішу
            while (Horses.Any(h => h.X < 950))
            {
                List<Task> tasks = new List<Task>();

                foreach (var horse in Horses)
                {
                    // запуск асинхронного прискорення
                    tasks.Add(horse.ChangeAcceleration());
                }

                // очікування завершення всіх потоків
                await Task.WhenAll(tasks);

                foreach (var horse in Horses)
                {
                    horse.X += horse.Acceleration;
                }

                await Task.Delay(50);
            }
        }
    }

}


