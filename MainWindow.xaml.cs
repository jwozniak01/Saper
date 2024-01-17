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
using System.Windows.Threading;

namespace Saper
{
    public enum State
    {
        normal,
        flag,
        mark
    }
    
    

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
    public partial class MainWindow : Window
    {
        private static int Rows { get; set; } = 8;
        private static int Columns { get; set; } = 8;

        public int Score { get; set; }

        private SolidColorBrush[] colors = new SolidColorBrush[3];
        private Image[] images = new Image[15];
        private int Mines { get; set; } = 10;
        private int ActualMines { get; set; } = 10;

        private List<int> listTilesWithBomb = new List<int>();
        private SaperButton[,] buttons = new SaperButton[Rows,Columns];

        private DispatcherTimer timer;
        private TimeSpan elapsedTime;
        public bool isFirstClick = true;
        public MainWindow()
        {
            colors[0] = new SolidColorBrush(Colors.White);
            colors[1] = new SolidColorBrush(Colors.LightGray);
            colors[2] = new SolidColorBrush(Colors.Red);
            images[0] = new Image {Source = new BitmapImage(new Uri("Images/blank.png", UriKind.RelativeOrAbsolute))};
            images[1] = new Image { Source = new BitmapImage(new Uri("Images/1.png", UriKind.RelativeOrAbsolute)) };
            images[2] = new Image { Source = new BitmapImage(new Uri("Images/2.png", UriKind.RelativeOrAbsolute)) };
            images[3] = new Image { Source = new BitmapImage(new Uri("Images/3.png", UriKind.RelativeOrAbsolute)) };
            images[4] = new Image { Source = new BitmapImage(new Uri("Images/4.png", UriKind.RelativeOrAbsolute)) };
            images[5] = new Image { Source = new BitmapImage(new Uri("Images/5.png", UriKind.RelativeOrAbsolute)) };
            images[6] = new Image { Source = new BitmapImage(new Uri("Images/6.png", UriKind.RelativeOrAbsolute)) };
            images[7] = new Image { Source = new BitmapImage(new Uri("Images/7.png", UriKind.RelativeOrAbsolute)) };
            images[8] = new Image { Source = new BitmapImage(new Uri("Images/8.png", UriKind.RelativeOrAbsolute)) };
            images[9] = new Image { Source = new BitmapImage(new Uri("Images/bomb.png", UriKind.RelativeOrAbsolute)) };
            images[10] = new Image { Source = new BitmapImage(new Uri("Images/flag.png", UriKind.RelativeOrAbsolute)) };
            images[11] = new Image { Source = new BitmapImage(new Uri("Images/mark.png", UriKind.RelativeOrAbsolute)) };
            images[12] = new Image { Source = new BitmapImage(new Uri("Images/blankg.png", UriKind.RelativeOrAbsolute)) };
            images[13] = new Image { Source = new BitmapImage(new Uri("Images/redBomb.png", UriKind.RelativeOrAbsolute)) };
            images[14] = new Image { Source = new BitmapImage(new Uri("Images/blueBomb.png", UriKind.RelativeOrAbsolute)) };
            MaxWidth = SystemParameters.PrimaryScreenWidth;
            MaxHeight = SystemParameters.PrimaryScreenHeight;
            Left = 0;
            Top = 0;
            InitializeComponent();
            InitializeTimer();
            InitializeGame();
        }


        private void InitializeGame()
        {
            WindowDraw();
            PlaceMines();
            GridDraw();
            RestartTimer();
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Tick;
        }

        private void Tick(object sender, EventArgs e)
        {
            elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1));
            UpdateTimerLabel();
        }
        private void UpdateTimerLabel()
        {
            lblTimer.Content = "Timer: " + elapsedTime.ToString(@"mm\:ss");
        }
        private void StartTimer()
        {
            elapsedTime = TimeSpan.Zero;
            timer.Start();
        }
        private void StopTimer()
        {
            timer.Stop();
        }
        private void RestartTimer()
        {
            StopTimer();
            elapsedTime = TimeSpan.Zero;
        }

        private void WindowDraw()
        {
            int height = 50 * Rows + 140;
            int width = 50 * Columns + 135;
            if (height > MaxHeight && width > MaxWidth) { WindowState = WindowState.Maximized; return; }
            if (height <= MaxHeight) Height = height; else Height = MaxHeight; 
            if (width <= MaxWidth) Width = width; else Width = MaxWidth;
            
        }

        private void PlaceMines()
        {
            listTilesWithBomb.Clear();
            ActualMines = Mines;
            Random rnd = new Random();
            int number;
            for(int i = 0; i < Mines; i++)
            {
                do
                {
                    number = rnd.Next(0, Columns * Rows);
                } while (listTilesWithBomb.Contains(number));
                listTilesWithBomb.Add(number);
            }

            Score = Rows * Columns - Mines;
            lblMines.Content = "Mines: " + Mines;
            lblScore.Content = "Score: " + Score;
        }

        private void ShowAllBoard(SaperButton current, bool isBombClicked)
        {
            foreach(SaperButton b in buttons)
            {
                if (b == current && isBombClicked)
                {
                    b.Image = CreateImage(images[13].Source);
                }
                if (b.State != State.normal)
                { 
                    if(b.Value == -1) b.Image = CreateImage(images[14].Source);
                    else b.Image = CreateImage(images[13].Source);
                }
                b.Content = b.Image;
            }
        }

        private void EndGame(SaperButton b, bool isBombClicked)
        {
            StopTimer();
            isFirstClick = true;
            ShowAllBoard(b,isBombClicked);
            MessageBox.Show("BOMB!!!","OOOOOPS!!!",MessageBoxButton.OK,MessageBoxImage.Exclamation);
            Block();
        }

        private int flagTest(SaperButton b, out int badflags)
        {
            int i = b.Row;
            int j = b.Col;
            int goodflags = 0;
            badflags = 0;
            if (i > 0 && j > 0 && buttons[i - 1, j - 1].State==State.flag)
            {
                if (buttons[i - 1, j - 1].Value == -1) goodflags++;
                else badflags++;
            }
            if (j > 0 && buttons[i, j - 1].State == State.flag)
            {
                if(buttons[i, j - 1].Value == -1) goodflags++;
                else badflags++;
            }
            if (i < Rows - 1 && j > 0 && buttons[i + 1, j - 1].State == State.flag)
            {
                if (buttons[i + 1, j - 1].Value == -1) goodflags++;
                else badflags++;
            }
            if (i > 0 && buttons[i - 1, j].State == State.flag)
            {
                if (buttons[i - 1, j].Value == -1) goodflags++;
                else badflags++;
            }
            if (i < Rows - 1 && buttons[i + 1, j].State == State.flag)
            {
                if (buttons[i + 1, j].Value == -1) goodflags++;
                else badflags++;
            }
            if (i > 0 && j < Columns - 1 && buttons[i - 1, j + 1].State == State.flag)
            {
                if (buttons[i - 1, j + 1].Value == -1) goodflags++;
                else badflags++;
            }
            if (j < Columns - 1 && buttons[i, j + 1].State == State.flag)
            {
                if (buttons[i, j + 1].Value == -1) goodflags++;
                else badflags++;
            }
            if (i < Rows - 1 && j < Columns - 1 && buttons[i + 1, j + 1].State == State.flag)
            {
                if (buttons[i + 1, j + 1].Value == -1) goodflags++;
                else badflags++;
            }
            return goodflags;
        }

        private void VerifyMine(SaperButton b, bool clickOnVisible)
        {
            int i = b.Row;
            int j = b.Col;

            if(!clickOnVisible)
            {
                if (i > 0 && j > 0) VerifyMine(buttons[i - 1, j - 1]);
                if (j > 0) VerifyMine(buttons[i, j - 1]);
                if (i < Rows - 1 && j > 0) VerifyMine(buttons[i + 1, j - 1]);
                if (i > 0) VerifyMine(buttons[i - 1, j]);
                if (i < Rows - 1) VerifyMine(buttons[i + 1, j]);
                if (i > 0 && j < Columns - 1) VerifyMine(buttons[i - 1, j + 1]);
                if (j < Columns - 1) VerifyMine(buttons[i, j + 1]);
                if (i < Rows - 1 && j < Columns - 1) VerifyMine(buttons[i + 1, j + 1]);
                Score--;
            }
            else
            {
                int badflags;
                int goodflags = flagTest(b,out badflags);
                if (goodflags == b.Value && !b.isVerified && badflags==0)
                {

                    b.isVerified = true;
                    if (i > 0 && j > 0 && buttons[i - 1, j - 1].Value != -1)
                    {
                        if (buttons[i - 1, j - 1].Value == 0) VerifyMine(buttons[i - 1, j - 1],true);
                        if (buttons[i - 1, j - 1].Content != buttons[i - 1, j - 1].Image)
                        {
                            buttons[i - 1, j - 1].Content = buttons[i - 1, j - 1].Image;
                            Score--;
                        }
                    }
                    if (j > 0 && buttons[i, j - 1].Value != -1)
                    {
                        
                        if (buttons[i, j - 1].Value == 0) VerifyMine(buttons[i, j - 1], true);
                        if(buttons[i, j - 1].Content != buttons[i, j - 1].Image)
                        {
                            buttons[i, j - 1].Content = buttons[i, j - 1].Image;
                            Score--;
                        }
                    }
                    if (i < Rows - 1 && j > 0 && buttons[i + 1, j - 1].Value != -1)
                    {
                        
                        if (buttons[i + 1, j - 1].Value == 0) VerifyMine(buttons[i + 1, j - 1], true);
                        if(buttons[i + 1, j - 1].Content != buttons[i + 1, j - 1].Image)
                        {
                            buttons[i + 1, j - 1].Content = buttons[i + 1, j - 1].Image;
                            Score--;
                        }
                    }
                    if (i > 0 && buttons[i - 1, j].Value != -1)
                    {
                        
                        if (buttons[i - 1, j].Value == 0) VerifyMine(buttons[i - 1, j], true);
                        if(buttons[i - 1, j].Content != buttons[i - 1, j].Image)
                        {
                            buttons[i - 1, j].Content = buttons[i - 1, j].Image;
                            Score--;
                        }
                    }
                    if (i < Rows - 1 && buttons[i + 1, j].Value != -1)
                    {
                        
                        if (buttons[i + 1, j].Value == 0) VerifyMine(buttons[i + 1, j], true);
                        if(buttons[i + 1, j].Content != buttons[i + 1, j].Image)
                        {
                            buttons[i + 1, j].Content = buttons[i + 1, j].Image;
                            Score--;
                        }
                    }
                    if (i > 0 && j < Columns - 1 && buttons[i - 1, j + 1].Value != -1)
                    {
                        
                        if (buttons[i - 1, j + 1].Value == 0) VerifyMine(buttons[i - 1, j + 1], true);
                        if(buttons[i - 1, j + 1].Content != buttons[i - 1, j + 1].Image)
                        {
                            buttons[i - 1, j + 1].Content = buttons[i - 1, j + 1].Image;
                            Score--;
                        }
                    }
                    if (j < Columns - 1 && buttons[i, j + 1].Value != -1)
                    {
                        
                        if (buttons[i, j + 1].Value == 0) VerifyMine(buttons[i, j + 1], true);
                        if(buttons[i, j + 1].Content != buttons[i, j + 1].Image)
                        {
                            buttons[i, j + 1].Content = buttons[i, j + 1].Image;
                            Score--;
                        }
                    }
                    if (i < Rows - 1 && j < Columns - 1 && buttons[i + 1, j + 1].Value != -1)
                    {
                        
                        if (buttons[i + 1, j + 1].Value == 0) VerifyMine(buttons[i + 1, j + 1], true);
                        if(buttons[i + 1, j + 1].Content != buttons[i + 1, j + 1].Image)
                        {
                            buttons[i + 1, j + 1].Content = buttons[i + 1, j + 1].Image;
                            Score--;
                        }
                    }
                }
                else if (badflags != 0 && goodflags!=b.Value) EndGame(b,false);   
            }
        }

        private void VerifyMine(SaperButton b)
        {
            if (b.Value == -1)
            {
                EndGame(b,true);
                return;
            }

            if (b.Value == 0 && b.Content != b.Image) //zero i nieodkryte
            {
                b.Content = b.Image;
                VerifyMine(b, false);
            }
            else if (b.Value > 0 && b.Content != b.Image) //wieksze niz zero i nieodkryte
            {
                b.Content = b.Image;
                Score--;
            }
            else if (b.Value > 0 && b.Content == b.Image && !b.isVerified) //wieksze niz zero i odkryte
            {
                VerifyMine(b, true);
                
            }
            lblScore.Content = "Score: " + Score;
        }



        private void Button_LeftClick(object sender, EventArgs e)
        {
            if (isFirstClick)
            {
                isFirstClick = false;

                StartTimer();
            }
            SaperButton b = (SaperButton)sender;
            if(b.State == State.normal) VerifyMine(b);
            if (Score == 0)
            {
                StopTimer();
                isFirstClick = true;
                MessageBox.Show("CONGRATULATIONS!!!", "YOU WIN!!!", MessageBoxButton.OK, MessageBoxImage.Information);
                Block();
                ShowAllBoard(b,false);
            }
        }

        private void Block()
        {
            foreach(SaperButton b in buttons)
            {
                b.IsEnabled = false;
            }
        }
        private void Button_RightClick(object sender, EventArgs e)
        {
            if (isFirstClick)
            {
                isFirstClick = false;
                StartTimer();
            }
            SaperButton b = (SaperButton)sender;
            Image newImage = new Image();

            if (b.Content is Image img)
            {
                if (img.Source == images[12].Source)
                {
                    newImage.Source = images[10].Source;
                    ActualMines--;
                    b.Content = newImage;
                    b.State= State.flag;
                }
                else if (img.Source == images[10].Source)
                {
                    newImage.Source = images[11].Source;
                    ActualMines++;
                    b.Content = newImage;
                    b.State= State.mark;
                }
                else if (img.Source == images[11].Source)
                {
                    newImage.Source = images[12].Source;
                    b.Content = newImage;
                    b.State = State.normal;
                } 
            }
            lblMines.Content = "Mines: " + ActualMines;
        }

        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            RestartTimer();
            InitializeGame();         
        }

        private void GridDraw()
        {
            buttons = new SaperButton[Rows, Columns];
            ButtonGrid.RowDefinitions.Clear();
            ButtonGrid.ColumnDefinitions.Clear();
            ButtonGrid.Children.Clear();
            
            for (int i = 0; i < Rows; i++)
                ButtonGrid.RowDefinitions.Add(new RowDefinition());

            for (int i = 0; i < Columns; i++)
                ButtonGrid.ColumnDefinitions.Add(new ColumnDefinition());
            int id = 0;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    SaperButton b = new SaperButton();
                    b.Content = CreateImage(images[12].Source);
                    b.LeftClick += Button_LeftClick;
                    b.RightClick += Button_RightClick;
                    b.Width = 50;
                    b.Height = 50;
                    b.Margin = new Thickness(0);
                    b.Padding = new Thickness(0);
                    b.VerticalAlignment = VerticalAlignment.Top;
                    b.HorizontalAlignment = HorizontalAlignment.Left;
                    b.Id = id;
                    b.Col = j;
                    b.Row = i;
                    if (listTilesWithBomb.Contains(id))
                    {
                        b.Value = -1;
                        b.Image = CreateImage(images[9].Source);
                    }
                    Grid.SetRow(b, i);
                    Grid.SetColumn(b, j);
                    ButtonGrid.Children.Add(b);
                    buttons[i, j] = b;
                    id++;
                }
            }
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    int temp = 0;
                    if (buttons[i, j].Value != -1)
                    {
                        if (i > 0 && j > 0 && buttons[i - 1, j - 1].Value == -1) temp++;
                        if (j > 0 && buttons[i, j - 1].Value == -1) temp++;
                        if (i < Rows - 1 && j > 0 && buttons[i + 1, j - 1].Value == -1) temp++;
                        if (i > 0 && buttons[i - 1, j].Value == -1) temp++;
                        if (i < Rows - 1 && buttons[i + 1, j].Value == -1) temp++;
                        if (i > 0 && j < Columns - 1 && buttons[i - 1, j + 1].Value == -1) temp++;
                        if (j < Columns - 1 && buttons[i, j + 1].Value == -1) temp++;
                        if (i < Rows - 1 && j < Columns - 1 && buttons[i + 1, j + 1].Value == -1) temp++;
                        buttons[i, j].Value = temp;
                        switch (temp)
                        {
                            case 0:
                                buttons[i, j].Image = CreateImage(images[0].Source);
                                break;
                            case 1:
                                buttons[i, j].Image = CreateImage(images[1].Source);
                                break;
                            case 2:
                                buttons[i, j].Image = CreateImage(images[2].Source);
                                break;
                            case 3:
                                buttons[i, j].Image = CreateImage(images[3].Source);
                                break;
                            case 4:
                                buttons[i, j].Image = CreateImage(images[4].Source);
                                break;
                            case 5:
                                buttons[i, j].Image = CreateImage(images[5].Source);
                                break;
                            case 6:
                                buttons[i, j].Image = CreateImage(images[6].Source);
                                break;
                            case 7:
                                buttons[i, j].Image = CreateImage(images[7].Source);
                                break;
                            case 8:
                                buttons[i, j].Image = CreateImage(images[8].Source);
                                break;
                        }
                    }
                }
            }
        }

        private Image CreateImage(ImageSource source)
        {
            Image image = new Image();
            image.Source = source;
            image.Stretch = Stretch.UniformToFill;
            return image;
        }

        private void btnChoose_Click(object sender, RoutedEventArgs e)
        {
            ChooseClick();
        }

        private void ChooseClick()
        {
            ChooseWindow window = new ChooseWindow();
            window.ShowDialog();
            if (window.HowClosed)
            {
                Columns = window.Cols;
                Rows = window.Rows;
                Mines = window.Mines;
                ActualMines = window.Mines;
                InitializeGame();
            }
        }
    }
}
