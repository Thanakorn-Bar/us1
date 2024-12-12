using System.IO.Compression;
using System.Threading;

namespace Sharkrain;

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

class GameObject
{
    public Coordinate Position;
    public char Symbol;
    public ConsoleColor Color;
    public int Lives;
    public bool Enabled;
}

class Program
{
    static int W = 40;

    static int H = 20;

    static GameObject Me;
    static List<GameObject> SharkList;
    static List<GameObject> HeartList;
    static Random RG;
    static int Score;

    static void Main(string[] args)
    {
        //Initialize
        SetupScreen();

        //Setup player
        Me = new GameObject();
        Me.Lives = 3;
        Me.Symbol = 'Q';
        Me.Color = ConsoleColor.Magenta;
        Me.Position.X = W/2;
        Me.Position.Y = H-2;
        //Setup Shark
        RG = new Random();
        SharkList = new List<GameObject>();
        HeartList = new List<GameObject>();

       
        while(true)
        {
            //Get input
            GetIntput();

            //Update game
            CreateShark();
            UpdateShark();
            CollisionDetection();

            //Draw
            Console.Clear();
            DrawSybol(Me.Position,Me.Symbol,Me.Color);
            DrawShark();
            DrawWord(new Coordinate(5,1),"Score: ",+Score,ConsoleColor.White);
            DrawWord(new Coordinate(27,1), "Live: ",Me.Lives, ConsoleColor.White);


            //Render
            Thread.Sleep(200);
        }


        RestoreScreen();

    }

    static void SetupScreen()
    {
        Console.Title = "Sharkrain";
        Console.BufferHeight = Console.WindowHeight = H;
        Console.BufferWidth = Console.WindowWidth = W;
        Console.CursorVisible = false;

    }

    static void RestoreScreen()
    {
        Console.CursorVisible = true;
        Console.ResetColor();
    }

    static void DrawSybol(Coordinate pos, char symbol, ConsoleColor color)
    {
        Console.SetCursorPosition(pos.X,pos.Y);
        Console.ForegroundColor = color;
        Console.Write(symbol);
    }

    static void DrawWord(Coordinate pos, string text, ConsoleColor color)
    {
        Console.SetCursorPosition(pos.X,pos.Y);
        Console.ForegroundColor = color;
        Console.Write(text);
    }

    static void GetIntput()
    {
        if(Console.KeyAvailable)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            while(Console.KeyAvailable)
            {
                Console.ReadKey();
            }

            if(keyInfo.Key == ConsoleKey.LeftArrow)
            {
                if(Me.Position.X > 0)
                {
                    Me.Position.X--;
                }
            }

            if(keyInfo.Key == ConsoleKey.RightArrow)
            {
                if(Me.Position.X < W-1)
                {
                    Me.Position.X++;
                }
            }

        }
    }

    static void CreateShark()
    {
        int spawnRate = RG.Next(0,10);
        
        if(spawnRate > 5)
        {
            GameObject newShark = new GameObject();
            newShark.Position.Y = 3;
            newShark.Position.X = RG.Next(1,W-1);
            newShark.Color = ConsoleColor.Blue;
            newShark.Symbol = '*';
            newShark.Enabled = true;
            SharkList.Add(newShark);
        }
    }

    static void CreateHeart()
    {
        int spawnRate = RG.Next(0,10);
        
        if(spawnRate > 5)
        {
            GameObject newHeart = new GameObject();
            newHeart.Position.Y = 3;
            newHeart.Position.X = RG.Next(1,W-1);
            newHeart.Color = ConsoleColor.Green;
            newHeart.Symbol = 'V';
            newHeart.Enabled = true;
            HeartList.Add(newHeart);
        }
    }

    static void UpdateHeart()
    {
        List<GameObject> newList = new
        List<GameObject>();

        for (int i=0; i < HeartList.Count; i++)
        {
            GameObject oldHeart = HeartList{i};
            GameObject newHeart = new GameObject();
            newHeart.Position.X = oldHeart.Position.X:
            newHeart.Position.Y = oldHeart.Position.Y+1;
            newHeart.Symbol = oldHeart.Symbol;
            newHeart.Color = oldHeart.Color;
            newHeart.Enabled = oldHeart.Enabled;

            if(newHeart.Position.Y >= H)
            {
                newHeart.Enabled = false;
                Score++;
            }

            if(newHeart.Enabled)
            {
                newList.Add(newHeart);
            }
        }
        HeartList = newList;
    }

    static void UpdateShark()
    {
        List<GameObject> newList = new
        List<GameObject>();

        for (int i=0; i < SharkList.Count; i++)
        {
            GameObject oldShark = SharkList{i};
            GameObject newShark = new GameObject();
            newShark.Position.X = oldShark.Position.X:
            newShark.Position.Y = oldShark.Position.Y+1;
            newShark.Symbol = oldShark.Symbol;
            newShark.Color = oldShark.Color;
            newShark.Enabled = oldShark.Enabled;

            if(newShark.Position.Y >= H)
            {
                newShark.Enabled = false;
                Score++;
            }

            if(newShark.Enabled)
            {
                newList.Add(newShark);
            }
        }
        SharkList = newList;
    }

    static void DrawShark()
    {
        for(int i=0; i < SharkList.Count; i++)
        {
            GameObject.ball = BallList[i];
            DrawSybol(ball.Position,ball.Symbol,ball.Color);    
        }
    }

    static void DrawHeart()
    {
        for(int i=0; i < HeartList.Count; i++)
        {
            GameObject.ball = BallList[i];
            DrawSybol(ball.Position,ball.Symbol,ball.Color);    
        }
    }

    static void CollisionDetection()
    {
        for(int i=0; i < SharkList.Count; i++)
        {
            GameObject Shark = SharkList[i];
            if((Shark.Position.X = Me.Position.X)&&(Shark.Position.Y == Me.Position.Y))
            {
                Me.Lives--;
                Console.BackgroundColor = ConsoleColor.Red;
                Console.Clear();
                Console.Beep(200,100);
                Console.BackgroundColor = ConsoleColor.Black;
            }
        }
    }

    static void CollisionDetection()
    {
        for(int i=0; i < HeartList.Count; i++)
        {
            GameObject Heart = HeartList[i];
            if((Heart.Position.X = Me.Position.X)&&(Heart.Position.Y == Me.Position.Y))
            {
                Me.Lives++;
                Console.BackgroundColor = ConsoleColor.Green;
                Console.Clear();
                Console.Beep(500,90);
                Console.BackgroundColor = ConsoleColor.Black;
            }
        }
    }

}

