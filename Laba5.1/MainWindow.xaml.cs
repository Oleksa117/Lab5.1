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
            }
        }
    }
   
}


