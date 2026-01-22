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
    public partial class MainWindow : Window
    {
        private static int Rows { get; set; } = 8;
        private static int Columns { get; set; } = 8;

        public int Score { get; set; }

        private ImageManager imageManager;
        private int Mines { get; set; } = 10;
        private int ActualMines { get; set; } = 10;

        private List<int> listTilesWithBomb = new List<int>();
        private SaperButton[,] buttons = new SaperButton[Rows,Columns];

        private DispatcherTimer timer;
        private TimeSpan elapsedTime;
        public bool isFirstClick = true;

        public MainWindow()
        {
            imageManager = new ImageManager();
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
                    b.Image = imageManager.GetImage(ImageType.RedBomb);
                }
                if (b.State != State.normal)
                {
                    if(b.Value == -1) b.Image = imageManager.GetImage(ImageType.BlueBomb);
                    else b.Image = imageManager.GetImage(ImageType.RedBomb);
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

        private IEnumerable<SaperButton> GetNeighbors(int r, int c)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;
                    int nr = r + i;
                    int nc = c + j;
                    if (nr >= 0 && nr < Rows && nc >= 0 && nc < Columns)
                    {
                        yield return buttons[nr, nc];
                    }
                }
            }
        }

        private int flagTest(SaperButton b, out int badflags)
        {
            int goodflags = 0;
            badflags = 0;

            foreach (var neighbor in GetNeighbors(b.Row, b.Col))
            {
                if (neighbor.State == State.flag)
                {
                    if (neighbor.Value == -1) goodflags++;
                    else badflags++;
                }
            }
            return goodflags;
        }

        private void VerifyMine(SaperButton b, bool clickOnVisible)
        {
            if(!clickOnVisible)
            {
                foreach (var neighbor in GetNeighbors(b.Row, b.Col))
                {
                    VerifyMine(neighbor);
                }
                Score--;
            }
            else
            {
                int badflags;
                int goodflags = flagTest(b,out badflags);
                if (goodflags == b.Value && !b.isVerified && badflags==0)
                {

                    b.isVerified = true;
                    foreach (var neighbor in GetNeighbors(b.Row, b.Col))
                    {
                        if (neighbor.Value != -1)
                        {
                            if (neighbor.Value == 0) VerifyMine(neighbor, true);
                            if (neighbor.Content != neighbor.Image)
                            {
                                neighbor.Content = neighbor.Image;
                                Score--;
                            }
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
                if (AreImagesEqual(img.Source, imageManager.GetImageSource(ImageType.BlankGreen)))
                {
                    newImage.Source = imageManager.GetImageSource(ImageType.Flag);
                    ActualMines--;
                    b.Content = newImage;
                    b.State= State.flag;
                }
                else if (AreImagesEqual(img.Source, imageManager.GetImageSource(ImageType.Flag)))
                {
                    newImage.Source = imageManager.GetImageSource(ImageType.Mark);
                    ActualMines++;
                    b.Content = newImage;
                    b.State= State.mark;
                }
                else if (AreImagesEqual(img.Source, imageManager.GetImageSource(ImageType.Mark)))
                {
                    newImage.Source = imageManager.GetImageSource(ImageType.BlankGreen);
                    b.Content = newImage;
                    b.State = State.normal;
                }
            }
            lblMines.Content = "Mines: " + ActualMines;
        }

        private bool AreImagesEqual(ImageSource img1, ImageSource img2)
        {
            return img1 == img2;
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
                    b.Content = imageManager.GetImage(ImageType.BlankGreen);
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
                        b.Image = imageManager.GetImage(ImageType.Bomb);
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
                        foreach (var neighbor in GetNeighbors(i, j))
                        {
                             if (neighbor.Value == -1) temp++;
                        }

                        buttons[i, j].Value = temp;
                        switch (temp)
                        {
                            case 0:
                                buttons[i, j].Image = imageManager.GetImage(ImageType.Blank);
                                break;
                            case 1:
                                buttons[i, j].Image = imageManager.GetImage(ImageType.One);
                                break;
                            case 2:
                                buttons[i, j].Image = imageManager.GetImage(ImageType.Two);
                                break;
                            case 3:
                                buttons[i, j].Image = imageManager.GetImage(ImageType.Three);
                                break;
                            case 4:
                                buttons[i, j].Image = imageManager.GetImage(ImageType.Four);
                                break;
                            case 5:
                                buttons[i, j].Image = imageManager.GetImage(ImageType.Five);
                                break;
                            case 6:
                                buttons[i, j].Image = imageManager.GetImage(ImageType.Six);
                                break;
                            case 7:
                                buttons[i, j].Image = imageManager.GetImage(ImageType.Seven);
                                break;
                            case 8:
                                buttons[i, j].Image = imageManager.GetImage(ImageType.Eight);
                                break;
                        }
                    }
                }
            }
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
