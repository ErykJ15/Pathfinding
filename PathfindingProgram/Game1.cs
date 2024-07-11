using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Windows.Forms;

///Some of the following websites and works that helped during the development process
///https://github.com/davecusatis/A-Star-Sharp/blob/master/Astar.cs
///https://dotnetcoretutorials.com/a-search-pathfinding-algorithm-in-c/
///https://www.redblobgames.com/pathfinding/a-star/implementation.html
///https://www.youtube.com/watch?v=FflEY83irJo&ab_channel=BatholithEntertainment
///https://www.youtube.com/watch?v=-L-WgKMFuhE&list=PLFt_AvWsXl0cq5Umv3pMC9SPnKjfp9eGW&index=2&ab_channel=SebastianLague


namespace PathfindingProgram
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Map mMap;
        private AStar mPathfinder;  //comment here if you want Dijkstra algorithm
        //private Dijkstra mPathfinder;  //uncomment here if you want A* algorithm
        private Player mPlayer;
        Texture2D mWhiteTile;
        Texture2D mGreyTile;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 600;

            Window.Title = "Pathfinding Comparison";
            Content.RootDirectory = "Content";
            mMap = new Map();
            mPathfinder = new AStar(2, 1);//comment here if you want Dijkstra algorithm
            //mPathfinder = new Dijkstra(12, 5);//uncomment here if you want A* algorithm
            mPlayer = new Player(30, 15);

        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            mWhiteTile = Content.Load<Texture2D>("whitesquare");
            mGreyTile = Content.Load<Texture2D>("greysquare");
            mPathfinder.mTexture = Content.Load<Texture2D>("pathfinder");
            mPlayer.mTexture = Content.Load<Texture2D>("player");
            mMap.GenerateMap("Content/level.txt");
        }

        protected override void Update(GameTime gameTime)
        {
            Input.Update();
            //here we check for possible user input that would cause the target to move

            //UP
            if (Input.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                if (mMap.ValidatePosition(new Vector2(mPlayer.mGridPosition.X, mPlayer.mGridPosition.Y - 1)))
                    mPlayer.mGridPosition.Y -= 1;
            }
            //DOWN
            if (Input.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                if (mMap.ValidatePosition(new Vector2(mPlayer.mGridPosition.X, mPlayer.mGridPosition.Y + 1)))
                    mPlayer.mGridPosition.Y += 1;
            }
            //LEFT
            if (Input.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                if (mMap.ValidatePosition(new Vector2(mPlayer.mGridPosition.X - 1, mPlayer.mGridPosition.Y)))
                    mPlayer.mGridPosition.X -= 1;
            }
            //RIGHT
            if (Input.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                if (mMap.ValidatePosition(new Vector2(mPlayer.mGridPosition.X + 1, mPlayer.mGridPosition.Y)))
                    mPlayer.mGridPosition.X += 1;
            }

            mPathfinder.Update(mMap, mPlayer);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime mGameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            DrawGrid();
            spriteBatch.Draw(mPathfinder.mTexture, mPathfinder.GridToScreenPosition(), Color.White);
            spriteBatch.Draw(mPlayer.mTexture, mPlayer.gridToScreenPosition(), Color.White);
            spriteBatch.End();

            base.Draw(mGameTime);
        }

        private void DrawGrid()
        {
            for (int x = 0; x < mMap.mGridSize; x++)
            {
                for (int y = 0; y < mMap.mGridSize; y++)
                {//here we draw the grid checking the type of tile for each cell of the grid
                    Vector2 pos = new Vector2((x * 15), (y * 15));
                    if (mMap.mCells[x, y] == 0) spriteBatch.Draw(mWhiteTile, pos, Color.White);
                    else if (mMap.mCells[x, y] == 1) spriteBatch.Draw(mGreyTile, pos, Color.White);
                    else if (mMap.mCells[x, y] == 2) spriteBatch.Draw(mWhiteTile, pos, Color.Yellow);
                    else if (mMap.mCells[x, y] == 3) spriteBatch.Draw(mWhiteTile, pos, Color.SkyBlue);
                    else if (mMap.mCells[x, y] == 4) spriteBatch.Draw(mWhiteTile, pos, Color.LightGreen);
                }
            }
        }
    }
}


