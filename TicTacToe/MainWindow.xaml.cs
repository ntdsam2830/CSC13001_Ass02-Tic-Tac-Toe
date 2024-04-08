using Microsoft.Win32;
using System.IO;
using System.Media;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TicTacToe;

namespace TicTacToe
{
    public class DrawingVisualHost : FrameworkElement
    {
        private DrawingVisual _drawingVisual;

        public DrawingVisualHost()
        {
            _drawingVisual = new DrawingVisual();
        }

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index)
        {
            return _drawingVisual;
        }

        public DrawingContext RenderOpen()
        {
            return _drawingVisual.RenderOpen();
        }
    }

    public partial class MainWindow : Window
    {
        private int N = 12;
        private const int WINNING_COUNT = 5;
        private double _cellW;
        private double _cellH;
        private bool _isPlayerX = true;
        private readonly Brush _xBrush = Brushes.Red;
        private readonly Pen _oBrush = new Pen(Brushes.Blue, 2);
        private readonly Pen _gridPen = new Pen(Brushes.Black, 2);
        private List<List<char>> _board = new List<List<char>>();
        private int cursorX = 0;
        private int cursorY = 0;
        private MediaPlayer backgroundMusic;
        private MediaPlayer playSound = new MediaPlayer();
        private MediaPlayer winSound = new MediaPlayer();
        private MediaPlayer gameOverSound = new MediaPlayer();
        private bool isSoundOn = true;
        private bool flag = false; //check if the keyboard mode is activated 

        public MainWindow()
        {
            InitializeComponent();
            backgroundMusic = new MediaPlayer();
            backgroundMusic.MediaEnded += BackgroundMusic_MediaEnded;
            backgroundMusic.Open(new Uri("Audio/background.mp3", UriKind.Relative));
            backgroundMusic.Play();
            GameCanvas.SizeChanged += GameCanvas_SizeChanged;
            initializeBoard();
            DrawGrid();
        }
        private void BackgroundMusic_MediaEnded(object? sender, EventArgs e)
        {
            backgroundMusic.Position = TimeSpan.Zero;
            backgroundMusic.Play();
        }

        private void initializeBoard(int changedValue = 12) //create a char-0 board 
        {
            N = changedValue;
            _board = new List<List<char>>();

            for (int i = 0; i < N; i++)
            {
                List<char> temp = new List<char>();
                for (int j = 0; j < N; j++)
                {
                    temp.Add('\0');
                }
                _board.Add(temp);
            }
        }

        private void GameCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GameCanvas.Children.Clear();
            DrawGrid();

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    DrawSymbol(i, j, _board[i][j]);
                }
            }
        }

        private void DrawGrid()
        {
            GameCanvas.Children.Clear();
            _cellW = GameCanvas.ActualWidth / N;
            _cellH = GameCanvas.ActualHeight / N;
            DrawingVisualHost gridHost = new DrawingVisualHost();

            using (DrawingContext dc = gridHost.RenderOpen())
            {
                for (int i = 1; i < N; i++)
                {
                    dc.DrawLine(_gridPen, new Point(i * _cellW, 0), new Point(i * _cellW, N * _cellH));
                    dc.DrawLine(_gridPen, new Point(0, i * _cellH), new Point(N * _cellW, i * _cellH));
                }
            }

            GameCanvas.Children.Add(gridHost);
        }

        private void DrawCursor(int x, int y, bool reset = false)
        {
            DrawingVisualHost symbolHost = new DrawingVisualHost();

            using (DrawingContext dc = symbolHost.RenderOpen())
            {
                Pen color1 = new Pen(Brushes.Black, 2);
                Pen color2 = new Pen(Brushes.Yellow, 2);

                if (!reset) dc.DrawRectangle(null, color2, new Rect(x * _cellW, y * _cellH, _cellW, _cellH));
                else dc.DrawRectangle(null, color1, new Rect(x * _cellW, y * _cellH, _cellW, _cellH));
            }

            GameCanvas.Children.Add(symbolHost);
        }

        private bool IsGameOver()
        {
            // Check rows
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N - WINNING_COUNT + 1; j++) // Optimized check to avoid unnecessary iterations
                {
                    char symbol = _board[i][j];
                    if (symbol != '\0' && symbol == _board[i][j + 1] && symbol == _board[i][j + 2] && symbol == _board[i][j + 3] && symbol == _board[i][j + 4])
                    {
                        winSound.Open(new Uri("Audio/win.wav", UriKind.Relative));
                        winSound.Play();
                        if (_isPlayerX) MessageBox.Show("Player X wins.");
                        else MessageBox.Show("Player O wins.");
                        return true;
                    }
                }
            }
            // Check columns (similar logic as rows)
            for (int j = 0; j < N; j++)
            {
                for (int i = 0; i < N - WINNING_COUNT + 1; i++)
                {
                    char symbol = _board[i][j];
                    if (symbol != '\0' && symbol == _board[i + 1][j] && symbol == _board[i + 2][j] && symbol == _board[i + 3][j] && symbol == _board[i + 4][j])
                    {
                        winSound.Open(new Uri("Audio/win.wav", UriKind.Relative));
                        winSound.Play();
                        if (_isPlayerX) MessageBox.Show("Player X wins.");
                        else MessageBox.Show("Player O wins.");
                        return true;
                    }
                }
            }
            // Check diagonals (considering both directions)
            for (int i = 0; i < N - WINNING_COUNT + 1; i++)
            {
                for (int j = 0; j < N - WINNING_COUNT + 1; j++)
                {
                    // Top-left to bottom-right diagonal
                    char symbol1 = _board[i][j];
                    if (symbol1 != '\0' && symbol1 == _board[i + 1][j + 1] && symbol1 == _board[i + 2][j + 2] && symbol1 == _board[i + 3][j + 3] && symbol1 == _board[i + 4][j + 4])
                    {
                        winSound.Open(new Uri("Audio/win.wav", UriKind.Relative));
                        winSound.Play();
                        if (_isPlayerX) MessageBox.Show("Player X wins.");
                        else MessageBox.Show("Player O wins.");
                        return true;
                    }

                    // Bottom-left to top-right diagonal
                    if (symbol1 != '\0' && symbol1 == _board[i][N - j - 1] && symbol1 == _board[i + 1][N - j - 2] && symbol1 == _board[i + 2][N - j - 3] && symbol1 == _board[i + 3][N - j - 4] && symbol1 == _board[i + 4][N - j - 5])
                    {
                        winSound.Open(new Uri("Audio/win.wav", UriKind.Relative));
                        winSound.Play();
                        if (_isPlayerX) MessageBox.Show("Player X wins.");
                        else MessageBox.Show("Player O wins.");
                        return true;
                    }

                    // Bottom-left to top-right diagonal
                    char symbol2 = _board[i][N - j - 1];
                    if (symbol2 != '\0' && symbol2 == _board[i + 1][N - j - 2] && symbol2 == _board[i + 2][N - j - 3] && symbol2 == _board[i + 3][N - j - 4] && symbol2 == _board[i + 4][N - j - 5])
                    {
                        winSound.Open(new Uri("Audio/win.wav", UriKind.Relative));
                        winSound.Play();
                        if (_isPlayerX) MessageBox.Show("Player X wins.");
                        else MessageBox.Show("Player O wins.");
                        return true;
                    }

                    // Top-right to bottom-left diagonal
                    symbol2 = _board[N - i - 1][j];
                    if (symbol2 != '\0' && symbol2 == _board[N - i - 2][j + 1] && symbol2 == _board[N - i - 3][j + 2] && symbol2 == _board[N - i - 4][j + 3] && symbol2 == _board[N - i - 5][j + 4])
                    {
                        winSound.Open(new Uri("Audio/win.wav", UriKind.Relative));
                        winSound.Play();
                        if (!_isPlayerX) MessageBox.Show("Player X wins.");
                        else MessageBox.Show("Player O wins.");
                        return true;
                    }
                }
            }
            return false;
        }

        private void Canvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            int x = (int)(e.GetPosition(GameCanvas).X / _cellW);
            int y = (int)(e.GetPosition(GameCanvas).Y / _cellH);

            if (_board[x][y] == '\0')
            {
                if (_isPlayerX)
                {
                    DrawSymbol(x, y, 'X');
                }
                else
                {
                    DrawSymbol(x, y, 'O');
                }

                _board[x][y] = _isPlayerX ? 'X' : 'O';
                //CheckGameOver();
                if (IsGameOver())
                {
                    ClearTheBoard();
                }
                // Check if the board is full (no empty spaces)
                else if (isBoardFull())
                {
                    gameOverSound.Open(new Uri("Audio/lose.wav", UriKind.Relative));
                    gameOverSound.Play();
                    MessageBox.Show("Game Over.");
                    ClearTheBoard();
                }
                _isPlayerX = !_isPlayerX;
            }
        }

        private bool isBoardFull()
        {
            foreach (var board in _board)
            {
                var oke = board.Any(symbol => symbol == '\0');
                if (oke)
                {
                    return false;
                }
            }

            return true;
        }

        private void DrawSymbol(int x, int y, char player)
        {
            DrawingVisualHost symbolHost = new DrawingVisualHost();

            using (DrawingContext dc = symbolHost.RenderOpen())
            {
                playSound.Open(new Uri("Audio/play.wav", UriKind.Relative));
                if (player == 'X')
                {
                    playSound.Play();
                    dc.DrawLine(new Pen(_xBrush, 2), new Point(x * _cellW + 5, y * _cellH + 5), new Point((x + 1) * _cellW - 5, (y + 1) * _cellH - 5));
                    dc.DrawLine(new Pen(_xBrush, 2), new Point(x * _cellW + 5, (y + 1) * _cellH - 5), new Point((x + 1) * _cellW - 5, y * _cellH + 5));
                }
                else if (player == 'O')
                {
                    playSound.Play();
                    dc.DrawEllipse(null, _oBrush, new Point((x + 0.5) * _cellW, (y + 0.5) * _cellH), _cellW / N +15, _cellH / N + 15);
                }
            }

            GameCanvas.Children.Add(symbolHost);
        }

        private void ClearTheBoard()
        {
            GameCanvas.Children.Clear();
            initializeBoard();
            DrawGrid();
        }

        private void NewGame()
        {
            GameCanvas.MouseLeftButtonDown -= Canvas_MouseLeftButtonDown;

            _isPlayerX = true;

            cursorX = 0;

            cursorY = 0;

            flag = false;

            ClearTheBoard();

            GameCanvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
        }
        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            NewGame();
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Change()
        {
            var dialog = new ChangeSize();
            dialog.ShowDialog();

            if (dialog.DialogResult == true)
            {
                //MessageBox.Show(dialog.changedValue.ToString());
                initializeBoard(dialog.changedValue);
                DrawGrid();
                flag = false;
            }
        }
        private void Change_Click(object sender, RoutedEventArgs e)
        {
            Change();
        }

        private void Save()
        {
            // Open a SaveFileDialog to choose a file path
            var saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Tic Tac Toe Save File (*.txt)|*.txt";
            saveDialog.DefaultExt = ".txt";
            var saveResult = saveDialog.ShowDialog();

            if (saveResult == true)
            {
                string filePath = saveDialog.FileName;

                try
                {
                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        // Write the size of the board
                        writer.WriteLine(N);

                        // Write the game board
                        foreach (var row in _board)
                        {
                            writer.WriteLine(new string(row.ToArray()));
                        }

                        // Write the current player
                        writer.WriteLine(_isPlayerX ? "X" : "O");
                    }

                    MessageBox.Show("Game saved successfully.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving game: {ex.Message}");
                }
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private bool LoadSaveData(string filePath)
        {
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    if (reader != null)
                    {
                        // Read the size of the board
                        int size = int.Parse(reader.ReadLine());

                        // Read the game board
                        initializeBoard(size);
                        _board.Clear();
                        for (int i = 0; i < size; i++)
                        {
                            string line = reader.ReadLine();
                            _board.Add(line.ToCharArray().ToList());
                        }

                        // Read the current player
                        string currentPlayer = reader.ReadLine();
                        if (currentPlayer != null)
                        {
                            char curPlayer = char.Parse(currentPlayer);
                            if (curPlayer == 'O') _isPlayerX = false;
                        }
                        else return false;

                        // Redraw the grid and symbols
                        DrawGrid();
                        for (int i = 0; i < size; i++)
                        {
                            for (int j = 0; j < size; j++)
                            {
                                DrawSymbol(i, j, _board[i][j]);
                            }
                        }
                        flag= false;
                        return true;
                    }
                    else return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading game: {ex.Message}");
                return false;
            }
        }
        private void Load()
        {
            // Open an OpenFileDialog to choose a save file
            var openDialog = new OpenFileDialog();
            openDialog.Filter = "Tic Tac Toe Save File (*.txt)|*.txt";
            openDialog.DefaultExt = ".txt";
            var openResult = openDialog.ShowDialog();

            if (openResult == true)
            {
                string filePath = openDialog.FileName;

                if (LoadSaveData(filePath))
                {
                    MessageBox.Show("Game loaded successfully!");
                }
            }
        }
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            Load();
        }

        private void MoveCursor(int dx, int dy, bool keyUsing = false)
        {
            if (keyUsing)
            {
                int newX = cursorX + dx;
                int newY = cursorY + dy;

                // Check if the new position is within bounds
                if (newX >= 0 && newX < N && newY >= 0 && newY < N)
                {
                    // Clear the previous cursor position
                    DrawCursor(cursorX, cursorY, true);

                    // Update cursor position
                    cursorX = newX;
                    cursorY = newY;

                    // Draw the cursor at the new position
                    DrawCursor(cursorX, cursorY);
                }
                else if (newX >= N)
                {
                    DrawCursor(cursorX, cursorY, true);

                    cursorX = 0;
                    cursorY += 1;

                    DrawCursor(cursorX, cursorY);
                }
                else if (newX < 0)
                {
                    DrawCursor(cursorX, cursorY, true);

                    cursorX = N - 1;
                    cursorY -= 1;

                    DrawCursor(cursorX, cursorY);
                }
                else if (newY >= N)
                {
                    DrawCursor(cursorX, cursorY, true);

                    cursorX += 1;
                    cursorY = 0;

                    DrawCursor(cursorX, cursorY);
                }
                else if (newY < 0)
                {
                    DrawCursor(cursorX, cursorY, true);

                    cursorX -= 1;
                    cursorY = N - 1;

                    DrawCursor(cursorX, cursorY);
                }
            }
        }

        private bool ResetCursor()
        {
            // Clear the previous cursor position
            DrawCursor(cursorX, cursorY, true);

            cursorX = 0;
            cursorY = 0;

            // Draw the cursor at the first position
            DrawCursor(cursorX, cursorY);
            return true;
        }

        private void PlaceSymbol(int x, int y, bool keyUsing = false)
        {
            if (keyUsing)
            {
                if (_board[x][y] == '\0')
                {
                    char symbol = _isPlayerX ? 'X' : 'O';
                    DrawSymbol(x, y, symbol);
                    _board[x][y] = symbol;

                    // Check for game over
                    if (IsGameOver())
                    {
                        ClearTheBoard();

                    }
                    // Check if the board is full (no empty spaces)
                    else if (isBoardFull())
                    {
                        MessageBox.Show("Game Over.");
                        ClearTheBoard();
                    }

                    _isPlayerX = !_isPlayerX;
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.LeftShift:
                    flag = ResetCursor();
                    break;
                case Key.RightShift:
                    flag = ResetCursor();
                    break;
                case Key.Up:
                    // Move cursor up
                    if (flag) MoveCursor(0, -1, flag);
                    break;
                case Key.Down:
                    // Move cursor down
                    if (flag) MoveCursor(0, 1, flag);
                    break;
                case Key.Left:
                    // Move cursor left
                    if (flag) MoveCursor(-1, 0, flag);
                    break;
                case Key.Right:
                    // Move cursor right
                    if (flag) MoveCursor(1, 0, flag);
                    break;
                case Key.Enter:
                    // Place X or O on the board
                    if (flag) PlaceSymbol(cursorX, cursorY, flag);
                    break;
                case Key.N:
                    NewGame();
                    break;
                case Key.R:
                    NewGame();
                    break;
                case Key.Q:
                    Close();
                    break;
                case Key.S:
                    Save();
                    break;
                case Key.L:
                    Load();
                    break;
                case Key.C:
                    Change();
                    break;
                default:
                    // Ignore other keys
                    break;
            }
        }

        private void ToggleSoundButton_Click(object sender, RoutedEventArgs e)
        {
            isSoundOn = !isSoundOn;
            if (isSoundOn)
            {
                SoundIcon.Source = new BitmapImage(new Uri("Icons/volume.png", UriKind.Relative));
                backgroundMusic.Play();

            }
            else
            {
                SoundIcon.Source = new BitmapImage(new Uri("Icons/mute.png", UriKind.Relative));
                backgroundMusic.Pause();
            }
        }

        private void FAQ_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FAQ();
            dialog.ShowDialog();
        }
    }
}
