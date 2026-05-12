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
using System.Diagnostics;

namespace Laba5._1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // список усіх коней
        public ObservableCollection<Horse> Horses { get; set; } = new ObservableCollection<Horse>();
        private double balance = 1000;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }


        private void CreateHorses(int count)
        {
            Horses.Clear();
            RaceCanvas.Children.Clear();

            Brush[] colors = { Brushes.Red, Brushes.Blue, Brushes.Yellow, Brushes.Black, Brushes.Purple, Brushes.Orange, Brushes.Pink, Brushes.Brown };

            // Отримуємо доступну висоту
            double availableHeight = RaceCanvas.ActualHeight;
            if (availableHeight <= 0)
            {
                availableHeight = 400;
            }

            // Відступи зверху та знизу
            double topMargin = 30;
            double bottomMargin = 30;
            double usableHeight = availableHeight - topMargin - bottomMargin;

            // Розраховуємо крок між кіньми
            double step = usableHeight / count;

            // Обмежуємо мінімальний крок, щоб коні не накладалися
            if (step < 45) step = 45;

            // Якщо коней багато і вони не вміщаються, встановлюємо мінімальний крок
            double totalNeededHeight = count * 45;
            if (totalNeededHeight > usableHeight)
            {
                step = 45;
                RaceCanvas.Height = topMargin + bottomMargin + totalNeededHeight;
            }
            else
            {
                RaceCanvas.Height = availableHeight;
            }

            for (int i = 0; i < count; i++)
            {
                Horse horse = new Horse($"Horse {i + 1}", colors[i]);

                Horses.Add(horse);

                // прямокутник який представляє коня
                horse.Shape = new Rectangle
                {
                    Width = 40,
                    Height = 40,
                    Fill = horse.Color,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    Tag = horse // Зберігаємо посилання на коня
                };
                // позиція коня на трасі
                double yPosition = topMargin + i * step;

                Canvas.SetLeft(horse.Shape, 10);
                Canvas.SetTop(horse.Shape, yPosition);

                RaceCanvas.Children.Add(horse.Shape);
            }

            // Встановлюємо лінію фінішу
            Canvas.SetLeft(FinishLine, 950);
            Canvas.SetTop(FinishLine, 0);
            FinishLine.Height = RaceCanvas.ActualHeight;

            // Додаємо лінію фінішу, якщо її ще немає
            if (!RaceCanvas.Children.Contains(FinishLine))
            {
                RaceCanvas.Children.Add(FinishLine);
            }
        }

        private void HorseCountListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;

            int count = int.Parse(((ListBoxItem)HorseCountListBox.SelectedItem).Content.ToString());

            double neededHeight = 30 + 30 + (count * 45);

            double minHeight = Math.Max(400, neededHeight);
            RaceCanvas.MinHeight = minHeight;

            // Оновлюємо висоту лінії фінішу
            if (FinishLine != null)
            {
                FinishLine.Height = RaceCanvas.Height;
            }
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            int count = int.Parse(((ListBoxItem)HorseCountListBox.SelectedItem).Content.ToString());

            CreateHorses(count);
            FinishLine.Height = RaceCanvas.ActualHeight;
            Stopwatch stopwatch = Stopwatch.StartNew();

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
                    Canvas.SetLeft(horse.Shape, horse.X);

                    if (horse.X >= 950 && horse.FinishTime == TimeSpan.Zero)
                    {
                        horse.FinishTime = stopwatch.Elapsed;
                    }
                }
                var ordered = Horses
                            .OrderBy(h => h.FinishTime)
                            .ToList();

                for (int i = 0; i < ordered.Count; i++)
                {
                    ordered[i].Position = i + 1;
                }

                var winner = Horses
                            .OrderBy(h => h.FinishTime)
                            .FirstOrDefault();

                if (winner != null && winner.FinishTime != TimeSpan.Zero)
                {
                    if (winner.Bet > 0)
                    {
                        double winMoney = winner.Bet * winner.Coefficient;

                        balance += winMoney;

                        BalanceText.Text = balance.ToString("0.00");

                        MessageBox.Show($"{winner.Name} переміг!\nВиграш: {winMoney:0.00}");
                        UpdateCoefficients();
                    }

                    break;
                }

                await Task.Delay(50);
            }
        }

        private void PlaceBet_Click(object sender, RoutedEventArgs e)
        {
            if (Horses.Count == 0)
            {
                MessageBox.Show("Спочатку створіть коней");
                return;
            }

            if (!double.TryParse(BetTextBox.Text, out double bet))
            {
                MessageBox.Show("Невірна ставка");
                return;
            }

            if (!int.TryParse(HorseNumberTextBox.Text, out int horseIndex))
            {
                MessageBox.Show("Невірний номер коня");
                return;
            }

            horseIndex--;

            if (horseIndex < 0 || horseIndex >= Horses.Count)
            {
                MessageBox.Show("Такого коня немає");
                return;
            }

            if (bet > balance)
            {
                MessageBox.Show("Недостатньо грошей");
                return;
            }

            foreach (var horse in Horses)
            {
                horse.Bet = 0;
            }

            Horses[horseIndex].Bet = bet;

            balance -= bet;

            BalanceText.Text = balance.ToString();
        }

        private void UpdateCoefficients()
        {
            foreach (var horse in Horses)
            {
                switch (horse.Position)
                {
                    case 1:
                        horse.Coefficient = Math.Max(1.2, horse.Coefficient - 0.3);
                        break;

                    case 2:
                        horse.Coefficient += 0.1;
                        break;

                    default:
                        horse.Coefficient += 0.3;
                        break;
                }

                horse.Coefficient =Math.Round(horse.Coefficient, 2);
            }
        }
    }
}

