///█ ■
////https://www.youtube.com/watch?v=SGZgvMwjq2U

namespace Snake
{
    public class GameSettings
    {
        public int ScreenWidth { get; set; } = 32;
        public int ScreenHeight { get; set; } = 16;
        public int GameSpeed { get; set; } = 100;
        public int Score { get; set; } = 5;

        public ConsoleColor CookieColor { get; set; } = ConsoleColor.Red;
        public ConsoleColor BorderColor { get; set; } = ConsoleColor.Yellow;
        public static Random RandomNumber { get; } = new Random();
    }

    public class Snake
    {
        public int HeadXPos { get; set; }
        public int HeadYPos { get; set; }
        public List<int> BodyXPos { get; }
        public List<int> BodyYPos { get; }
        public ConsoleColor SnakeHeadColor { get; set; }
        public ConsoleColor SnakeBodyColor { get; set; }

        public Snake()
        {
            BodyXPos = new List<int>();
            BodyYPos = new List<int>();
        }
    }

    public enum MovementDirection
    {
        Up,
        Down,
        Left,
        Right
    }


    class Program
    {
        private static int lastTailX;
        private static int lastTailY;

        private static void Main(string[] args)
        {
            var gameSettings = new GameSettings();
            var movement = MovementDirection.Right;
            var cookiePositionX = GameSettings.RandomNumber.Next(0, gameSettings.ScreenWidth);
            var cookiePositionY = GameSettings.RandomNumber.Next(0, gameSettings.ScreenHeight);
            var gameOver = false;
            var lastUpdateTime = DateTime.Now;

            var snake = new Snake
            {
                HeadXPos = gameSettings.ScreenWidth / 2,
                HeadYPos = gameSettings.ScreenHeight / 2,
                SnakeHeadColor = ConsoleColor.Cyan,
                SnakeBodyColor = ConsoleColor.Green,
            };

            DrawBorders(gameSettings);

            while (!gameOver)
            {
                var currentTime = DateTime.Now;
                var elapsedTime = (currentTime - lastUpdateTime).TotalMilliseconds;
                if (elapsedTime < gameSettings.GameSpeed) continue;

                ProcessInput(ref movement);
                UpdateSnakePosition(snake, movement, gameSettings);
                gameOver = CheckGameOver(snake, gameSettings);
                HandleCookieConsumption(ref cookiePositionX, ref cookiePositionY, snake, gameSettings);

                if (!gameOver)
                {
                    RedrawGameWindow(snake, cookiePositionX, cookiePositionY, gameSettings);
                }

                lastUpdateTime = currentTime;
            }

            DisplayGameOver(gameSettings);
        }

        private static void DrawBorders(GameSettings gameSettings)
        {
            Console.ForegroundColor = gameSettings.BorderColor;
            DrawBorderLine(0, gameSettings.ScreenWidth, (i) => (i, 0));
            DrawBorderLine(0, gameSettings.ScreenWidth, (i) => (i, gameSettings.ScreenHeight - 1));
            DrawBorderLine(0, gameSettings.ScreenHeight, (i) => (0, i));
            DrawBorderLine(0, gameSettings.ScreenHeight, (i) => (gameSettings.ScreenWidth - 1, i));
        }

        private static void DrawBorderLine(int start, int end, Func<int, (int, int)> getCoordinates)
        {
            for (var i = start; i < end; i++)
            {
                var (x, y) = getCoordinates(i);
                Console.SetCursorPosition(x, y);
                Console.Write("■");
            }
        }

        private static void ProcessInput(ref MovementDirection movement)
        {
            if (!Console.KeyAvailable) return;

            var key = Console.ReadKey(true).Key;
            movement = key switch
            {
                ConsoleKey.UpArrow when movement != MovementDirection.Down => MovementDirection.Up,
                ConsoleKey.DownArrow when movement != MovementDirection.Up => MovementDirection.Down,
                ConsoleKey.LeftArrow when movement != MovementDirection.Right => MovementDirection.Left,
                ConsoleKey.RightArrow when movement != MovementDirection.Left => MovementDirection.Right,
                _ => movement
            };
        }

        private static void UpdateSnakePosition(Snake snake, MovementDirection movement, GameSettings gameSettings)
        {
            // Move the snake's body by adding a new head position
            snake.BodyXPos.Add(snake.HeadXPos);
            snake.BodyYPos.Add(snake.HeadYPos);

            // Update the snake's head position
            switch (movement)
            {
                case MovementDirection.Up:
                    snake.HeadYPos--;
                    break;
                case MovementDirection.Down:
                    snake.HeadYPos++;
                    break;
                case MovementDirection.Left:
                    snake.HeadXPos--;
                    break;
                case MovementDirection.Right:
                    snake.HeadXPos++;
                    break;
            }

            if (snake.BodyXPos.Count > gameSettings.Score)
            {
                lastTailX = snake.BodyXPos[0];
                lastTailY = snake.BodyYPos[0];
                snake.BodyXPos.RemoveAt(0);
                snake.BodyYPos.RemoveAt(0);
            }
        }

        private static bool CheckGameOver(Snake snake, GameSettings gameSettings)
        {
            if (snake.HeadXPos < 1 || snake.HeadXPos >= gameSettings.ScreenWidth - 1 ||
                snake.HeadYPos < 1 || snake.HeadYPos >= gameSettings.ScreenHeight - 1)
            {
                return true;
            }

            for (var i = 0; i < snake.BodyXPos.Count; i++)
            {
                if (snake.BodyXPos[i] == snake.HeadXPos && snake.BodyYPos[i] == snake.HeadYPos)
                {
                    return true;
                }
            }

            return false;
        }

        private static void HandleCookieConsumption(ref int cookiePosX, ref int cookiePosY, Snake snake,
            GameSettings settings)
        {
            if (cookiePosX != snake.HeadXPos || cookiePosY != snake.HeadYPos) return;

            settings.Score++;
            cookiePosX = GameSettings.RandomNumber.Next(1, settings.ScreenWidth - 2);
            cookiePosY = GameSettings.RandomNumber.Next(1, settings.ScreenHeight - 2);
        }

        private static void RedrawGameWindow(Snake snake, int cookiePosX, int cookiePosY, GameSettings gameSettings)
        {
            if (snake.BodyXPos.Count >= gameSettings.Score)
            {
                Console.SetCursorPosition(lastTailX, lastTailY);
                Console.Write(" ");
            }

            DrawSnake(snake);
            DrawGameObject(cookiePosX, cookiePosY, gameSettings.CookieColor, '■');
        }

        private static void DrawSnake(Snake snake)
        {
            for (var i = 0; i < snake.BodyXPos.Count; i++)
            {
                Console.SetCursorPosition(snake.BodyXPos[i], snake.BodyYPos[i]);
                Console.ForegroundColor = snake.SnakeBodyColor;
                Console.Write("■");
            }

            Console.SetCursorPosition(snake.HeadXPos, snake.HeadYPos);
            Console.ForegroundColor = snake.SnakeHeadColor;
            Console.Write("■");
        }

        private static void DrawGameObject(int x, int y, ConsoleColor color, char symbol)
        {
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = color;
            Console.Write(symbol);
        }

        private static void DisplayGameOver(GameSettings gameSettings)
        {
            Console.SetCursorPosition(gameSettings.ScreenWidth / 5, gameSettings.ScreenHeight / 2);
            Console.WriteLine("Game over, Score: " + gameSettings.Score);
            Console.SetCursorPosition(gameSettings.ScreenWidth / 5, gameSettings.ScreenHeight / 2 + 1);
            Console.ReadKey();
        }
    }
}