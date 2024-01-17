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
using System.Windows.Shapes;

namespace Saper
{
    public partial class ChooseWindow : Window
    {
        public int Rows { get; set; } = 0;
        public int Cols { get; set; } = 0;
        public int Mines { get; set; } = 0;
        public bool HowClosed { get; set; } = false;
        public ChooseWindow()
        {
            InitializeComponent();
            
        }

        
        private void btnReady_Click(object sender, RoutedEventArgs e)
        {
            int temp=0;
            if(Int32.TryParse(txbColumns.Text, out temp))
                if (temp > 1 && temp <= 50)  Cols = temp;
            if (Int32.TryParse(txbRows.Text, out temp))
                if (temp > 1 && temp <= 50) Rows = temp;
            if (Int32.TryParse(txbMines.Text, out temp))
                if (temp > 0 && temp < Rows * Cols) Mines = temp;
           
            if (Rows != 0 && Cols != 0 && Mines != 0)
            {
                HowClosed= true;
                Close();
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumeric(e.Text);
        }

        private bool IsTextNumeric(string text)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(text, "^[0-9]+$");
        }

        private void btnEasy_Click(object sender, RoutedEventArgs e)
        {
            Cols = 8;
            Rows = 8;
            Mines = 10;
            HowClosed = true;
            Close();
        }

        private void btnInter_Click(object sender, RoutedEventArgs e)
        {
            Cols = 16;
            Rows = 16;
            Mines = 40;
            HowClosed = true;
            Close();
        }

        private void btnExpert_Click(object sender, RoutedEventArgs e)
        {
            Cols = 31;
            Rows = 16;
            Mines = 99;
            HowClosed = true;
            Close();
        }
    }
}
