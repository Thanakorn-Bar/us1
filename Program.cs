using System;
using System.Collections.Generic;
using System.Threading;

namespace TsunamiEscape
{
    struct Coordinate
    {
        public int X;
        public int Y;
        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    enum ObstacleType
    {
        Normal,       // อุปสรรคทั่วไป
        Deadly,       // อุปสรรคที่ลดพลังชีวิต
        ScorePenalty  // อุปสรรคที่ลดคะแนน
    }

    enum ItemType
    {
        HealthPowerUp, // ไอเทมเพิ่มพลังชีวิต
        ExtraLife      // ไอเทมเพิ่มชีวิต
    }

    class GameObject
    {
        public Coordinate Position;
        public char Symbol;
        public ConsoleColor Color;
        public bool Enabled;
        public int Speed;
        public ObstacleType ObstacleType; // เพิ่มประเภทอุปสรรค
        public ItemType ItemType; // เพิ่มประเภทไอเทม
    }

    class Program
    {
        static int W = 40;  // Width of the screen
        static int H = 20;  // Height of the screen
        static int Level = 1;
        static int Speed = 200;
        static int Score = 0;
        static int PlayerLives = 5;  // Start with 5 lives
        static int PlayerHealth = 10;  // Initial health 10 instead of 5
        static GameObject Player;
        static List<GameObject> Obstacles;
        static List<GameObject> Items;
        static GameObject TsunamiWave;
        static Random RG;

        static void Main(string[] args)
        {
            SetupScreen();
            InitializeGame();

            while (Player.Enabled && PlayerLives > 0)
            {
                GetInput();
                UpdateObstacles();
                UpdateItems();
                CollisionDetection();
                CheckLevelUp();
                DrawFrame();
                Thread.Sleep(Speed);
            }

            GameOver();
        }

        static void SetupScreen()
        {
            Console.Title = "Escape the Tsunami";
            Console.BufferHeight = Console.WindowHeight = H;
            Console.BufferWidth = Console.WindowWidth = W;
            Console.CursorVisible = false;
        }

        static void InitializeGame()
        {
            Player = new GameObject { Position = new Coordinate(2, H / 2), Symbol = 'P', Color = ConsoleColor.Green, Enabled = true };
            TsunamiWave = new GameObject { Position = new Coordinate(0, H / 2), Symbol = '~', Color = ConsoleColor.Blue, Enabled = true };
            Obstacles = new List<GameObject>();
            Items = new List<GameObject>();
            RG = new Random();
        }

        static void PlaySound(int frequency, int duration)
        {
            Console.Beep(frequency, duration);
        }

        static void GetInput()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                while (Console.KeyAvailable) Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (Player.Position.Y > 0) Player.Position.Y--;
                        break;
                    case ConsoleKey.DownArrow:
                        if (Player.Position.Y < H - 1) Player.Position.Y++;
                        break;
                }
            }
        }

        static void UpdateObstacles()
        {
            // Spawn new obstacles randomly from the right side
            if (RG.Next(0, 10) > 7)
            {
                ObstacleType type = (ObstacleType)RG.Next(0, 3);  // Randomly pick an obstacle type

                // Update obstacle symbols and behavior
                Obstacles.Add(new GameObject
                {
                    Position = new Coordinate(W - 1, RG.Next(0, H)),
                    Symbol = type == ObstacleType.Normal ? '#' : (type == ObstacleType.Deadly ? 'X' : '%'),
                    Color = type == ObstacleType.Normal ? ConsoleColor.Red : (type == ObstacleType.Deadly ? ConsoleColor.DarkRed : ConsoleColor.Yellow),
                    Enabled = true,
                    Speed = type == ObstacleType.Deadly ? RG.Next(3, 5) : RG.Next(1, 3), // Deadly obstacles move faster
                    ObstacleType = type  // Assign the random type
                });
            }

            // Move obstacles leftward
            List<GameObject> newObstacles = new List<GameObject>();
            foreach (var obstacle in Obstacles)
            {
                obstacle.Position.X--;
                if (obstacle.Position.X >= 0) newObstacles.Add(obstacle);
            }
            Obstacles = newObstacles;
        }

        static void UpdateItems()
        {
            // Reduce spawn chance for HealthPowerUp and ExtraLife to 1 in 20 (lower chance)
            if (RG.Next(0, 20) > 18)  // 1 in 20 chance to spawn HealthPowerUp item
            {
                // Create HealthPowerUp item
                Items.Add(new GameObject
                {
                    Position = new Coordinate(W - 1, RG.Next(0, H)),
                    Symbol = 'H',
                    Color = ConsoleColor.Cyan,
                    Enabled = true,
                    ItemType = ItemType.HealthPowerUp
                });
            }

            if (RG.Next(0, 20) > 18)  // 1 in 20 chance to spawn ExtraLife item
            {
                Items.Add(new GameObject
                {
                    Position = new Coordinate(W - 1, RG.Next(0, H)),
                    Symbol = 'S',  // Symbol for ExtraLife item
                    Color = ConsoleColor.Magenta,
                    Enabled = true,
                    ItemType = ItemType.ExtraLife  // Item type is ExtraLife
                });
            }

            // Move items leftward
            List<GameObject> newItems = new List<GameObject>();
            foreach (var item in Items)
            {
                item.Position.X--;
                if (item.Position.X >= 0) newItems.Add(item);
            }
            Items = newItems;
        }

        static void CollisionDetection()
        {
            // Check collisions with obstacles
            foreach (var obstacle in Obstacles)
            {
                if (obstacle.Position.X == Player.Position.X && obstacle.Position.Y == Player.Position.Y)
                {
                    // Change screen color to red for 0.2 second
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.Clear();
                    PlaySound(1000, 500);  // Play beep sound for collision
                    Thread.Sleep(200);  // Keep the red screen for 0.2 second
                    Console.BackgroundColor = ConsoleColor.Black; // Restore background color
                    Console.Clear();

                    switch (obstacle.ObstacleType)
                    {
                        case ObstacleType.Normal:
                            PlayerLives--;
                            Player.Color = ConsoleColor.Red; // Flash red when player hits a normal obstacle
                            DrawFrame();
                            Thread.Sleep(200);
                            Player.Color = ConsoleColor.Green; // Restore player's color
                            Player.Position.X = Math.Max(2, Player.Position.X - 2); // Move player left
                            obstacle.Enabled = false;
                            break;

                        case ObstacleType.ScorePenalty:
                            Score = Math.Max(0, Score - 10); // Decrease score by 10 when hitting this obstacle
                            PlaySound(300, 200);  // Play beep sound for penalty
                            Player.Color = ConsoleColor.Yellow; // Flash yellow
                            DrawFrame();
                            Thread.Sleep(200);
                            Player.Color = ConsoleColor.Green;
                            obstacle.Enabled = false;
                            break;

                        case ObstacleType.Deadly:
                            PlayerHealth -= 1; // Reduce health by 1 when hitting a deadly obstacle
                            if (PlayerHealth <= 0)
                            {
                                PlayerLives = 0; // If health is zero or below, the player loses a life
                                Player.Enabled = false; // End the game
                            }
                            PlaySound(1000, 200);  // Play beep sound for deadly collision
                            Player.Color = ConsoleColor.DarkRed; // Flash dark red
                            DrawFrame();
                            Thread.Sleep(200);
                            Player.Color = ConsoleColor.Green;
                            obstacle.Enabled = false;
                            break;
                    }
                }
            }

            // Check collisions with items
            foreach (var item in Items)
            {
                if (item.Position.X == Player.Position.X && item.Position.Y == Player.Position.Y)
                {
                    // Change screen color to green for item collision for 0.2 second
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.Clear();
                    PlaySound(600, 200);  // Play beep sound for item collision
                    Thread.Sleep(200);  // Display green screen for 0.2 second
                    Console.BackgroundColor = ConsoleColor.Black; // Restore background color
                    Console.Clear();

                    switch (item.ItemType)
                    {
                        case ItemType.HealthPowerUp:
                            // เพิ่มเงื่อนไขไม่ให้เพิ่มพลังชีวิตเกิน 10
                            if (PlayerHealth < 10)
                            {
                                PlayerHealth = Math.Min(10, PlayerHealth + 2); // Increase health by 2, but not above 10
                                item.Symbol = 'L';  // Change symbol to L
                                item.Enabled = false;
                            }
                            break;

                        case ItemType.ExtraLife:
                            // เพิ่มเงื่อนไขไม่ให้เพิ่มชีวิตเกิน 5
                            if (PlayerLives < 5)
                            {
                                PlayerLives++; // Increase PlayerLives by 1
                                item.Symbol = 'L';  // Change symbol to L
                                item.Enabled = false;
                            }
                            break;
                    }
                }
            }
        }

        static void CheckLevelUp()
        {
            Score++;
            if (Score % 200 == 0) // Level up every 200 points
            {
                Level++;
                Speed = Math.Max(100, Speed - 10); // Decrease speed for next level
                Console.ForegroundColor = ConsoleColor.Cyan;
                PlaySound(1000, 300);  // Play level up sound
                Thread.Sleep(200);
            }
        }

        static void DrawFrame()
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Level: {Level} Score: {Score} Lives: {PlayerLives} Health: {PlayerHealth}");

            // Draw TsunamiWave
            Console.SetCursorPosition(TsunamiWave.Position.X, TsunamiWave.Position.Y);
            Console.ForegroundColor = TsunamiWave.Color;
            Console.Write(TsunamiWave.Symbol);

            // Draw Obstacles
            foreach (var obstacle in Obstacles)
            {
                Console.SetCursorPosition(obstacle.Position.X, obstacle.Position.Y);
                Console.ForegroundColor = obstacle.Color;
                Console.Write(obstacle.Symbol);
            }

            // Draw Items
            foreach (var item in Items)
            {
                Console.SetCursorPosition(item.Position.X, item.Position.Y);
                Console.ForegroundColor = item.Color;
                Console.Write(item.Symbol);
            }

            // Draw Player
            Console.SetCursorPosition(Player.Position.X, Player.Position.Y);
            Console.ForegroundColor = Player.Color;
            Console.Write(Player.Symbol);
        }

        static void GameOver()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(W / 2 - 5, H / 2);
            Console.WriteLine("GAME OVER");
            Console.SetCursorPosition(W / 2 - 7, H / 2 + 1);
            Console.WriteLine($"Score: {Score}");
            Console.SetCursorPosition(W / 2 - 12, H / 2 + 2);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }
    }
}
