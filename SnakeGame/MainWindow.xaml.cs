using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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

namespace SnakeGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string Konami = "";


        private int speed = 100;

        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
        {
            { GridValue.Empty, Images.Empty },
            { GridValue.Snake, Images.Body },
            { GridValue.Food, Images.Food }
        };

        private readonly Dictionary<Direction, int> dirToRotation = new()
        {
            { Direction.Up, 0 },
            { Direction.Right, 90 },
            { Direction.Down, 180 },
            { Direction.Left, 270 }
        };

        private readonly int rows = 28, cols = 28;


        private readonly Image[,] gridImages;
        private GameState gameState;
        private bool gameRunning;
        private int highScore = 0;
        private Random random = new Random();

        private DispatcherTimer timer;
        private DateTime startTime;
        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid();
            gameState = new GameState(rows, cols);
            string fileName = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "highScore.txt)");
            if (File.Exists(fileName))
            {
                StreamReader sr = new StreamReader(fileName);
                highScore = int.Parse(sr.ReadLine());
                sr.Close();
            }
            else
            {
                StreamWriter sw = new StreamWriter(fileName);
                sw.WriteLine(highScore);
                sw.Close();
            }
        }
        private async Task RunGame()
        {
            Draw();
            await ShowCountDown();

            Start_Timer();

            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();
            await ShowGameOver();
            gameState= new GameState(rows, cols);
        }

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }

            if (!gameRunning)
            {
                gameRunning = true;
                await RunGame();
                gameRunning = false;
            }
        }




        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Left:
                    gameState.ChangeDirection(Direction.Left);
                    Konami += "L";
                    break;
                case Key.Right:
                    gameState.ChangeDirection(Direction.Right);
                    Konami += "R";
                    break;
                case Key.Up:
                    gameState.ChangeDirection(Direction.Up);
                    Konami += "U";
                    break;
                case Key.Down:
                    gameState.ChangeDirection(Direction.Down);
                    Konami += "D";
                    break;
                    //add a string checker as well as a way to save strings entered.
            }
            if (Konami.Contains("UDLR"))
            {
                Konami = "";
                speed += 50;
            }
        }
        private async Task GameLoop()
        {
            while (!gameState.GameOver)
            {
                await Task.Delay(speed);
                gameState.Move();
                Draw();
            }
        }

        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Image image = new Image
                    {
                        Source = Images.Empty,
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    };

                    images[r, c] = image;
                    GameGrid.Children.Add(image);
                }
            }

            return images;
        }

        
        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();
            ScoreText.Text= $"SCORE {gameState.Score}";
        }

        

        private void DrawGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for ( int c = 0; c < cols; c++)
                {
                    GridValue gridValue = gameState.Grid[r, c];
                    gridImages[r, c].Source = gridValToImage[gridValue];
                    gridImages[r, c].RenderTransform = Transform.Identity;
                }
            }
        }

        private void DrawSnakeHead()
        {
            Position headPos = gameState.HeadPosition();
            Image image = gridImages[headPos.Row, headPos.Col];
            image.Source = Images.Head;

            int rotation = dirToRotation[gameState.Dir];
            image.RenderTransform = new RotateTransform(rotation);
        }

        private async Task DrawDeadSnake()
        {
            List<Position> positions = new List<Position>(gameState.SnakePositions());

            for (int i = 0; i < positions.Count; i++)
            {
                Position pos = positions[i];
                ImageSource source = (i == 0) ? Images.DeadHead : Images.DeadBody;
                gridImages[pos.Row, pos.Col].Source = source;

                Audio.DeathSound.Play();

                //Delay for each body death. for a quicker death, set number smaller
                await Task.Delay(75);
            }
        }

        private async Task ShowCountDown()
        {
            for (int i = 3; i >= 1; i--)
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(500);
            }
        }

        private async Task ShowGameOver()
        {
            ShakeWindow(2000);

            if (gameState.Score > highScore)
            {
                highScore = gameState.Score;
                StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + "\\highScore.txt");
                sw.WriteLine(highScore);
                sw.Close();
            }

            HighScoreText.Text = $"HIGH SCORE: {highScore}";

            timer.Stop();


            if (speed > 100)
            {
                speed = 100;
            }


            await DrawDeadSnake();

            Audio.Theme.Play();

            await Task.Delay(1000);


            
            Overlay.Visibility = Visibility.Visible;

            

            OverlayText.Text = "PRESS ANY KEY TO START";
        }

        private async Task ShakeWindow(int durationMs)
        {
            var oLeft = this.Left;
            var oTop = this.Top;

            var shakeTimer = new DispatcherTimer();
            shakeTimer.Tick += (sender, args) =>
            {
                this.Left = oLeft + random.Next(-10, 11);
                this.Top = oTop + random.Next(-10, 11);
            };

            shakeTimer.Interval = TimeSpan.FromMilliseconds(100);
            shakeTimer.Start();

            await Task.Delay(durationMs);
            shakeTimer.Stop();   

        }
        
        private void Start_Timer()
        {
          timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;

            startTime = DateTime.Now;

            timer.Start();
            
        }

        private void End_Timer()
        {

            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsedTime = DateTime.Now - startTime;
            timerTextBlock.Text = $"{elapsedTime.Hours:D2} : {elapsedTime.Minutes:D2} : {elapsedTime.Seconds:D2}";
        }
    }
}
