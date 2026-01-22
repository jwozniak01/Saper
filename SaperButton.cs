using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Saper
{
    public class SaperButton : Button
    {
        public int Id { get; set; }
        public int Value { get; set; }

        public int Row { get; set; }
        public int Col { get; set; }

        public Image Image { get; set; }

        public State State { get; set; } = State.normal;


        public bool isVerified = false;

        public event EventHandler LeftClick;
        public event EventHandler RightClick;

        public SaperButton()
        {
            Click += SaperButton_Click;
            MouseRightButtonDown += SaperButton_RightClick;
        }

        private void SaperButton_Click(object sender, RoutedEventArgs e)
        {
            LeftClick?.Invoke(this, EventArgs.Empty);
        }

        private void SaperButton_RightClick(object sender, MouseButtonEventArgs e)
        {
            RightClick?.Invoke(this, EventArgs.Empty);
        }
    }
}
