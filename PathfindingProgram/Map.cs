using Microsoft.Xna.Framework;
using System.IO;


namespace PathfindingProgram
{
    public class Map
    {
        public int[,] mCells;
        public int mGridSize = 40;

        public Map()
        {
            //cell is an array that determine what type of tile there is for each space in our grid map
            //at start we initialize it to 0
            mCells = new int[mGridSize, mGridSize];
            for (int i = 0; i < mGridSize; i++)
                for (int j = 0; j < mGridSize; j++)
                    mCells[i, j] = 0;
        }

        public bool ValidatePosition(Vector2 pPos)
        {
            //return false if the position is out the grid or in a wall
            if (pPos.X < 0 || pPos.X >= mGridSize || pPos.Y < 0 || pPos.Y >= mGridSize || mCells[(int)pPos.X, (int)pPos.Y] == 1) return false;
            return true;
        }

        public void GenerateMap(string pPath)
        {
            //this function loads each line from the file, and for each line it loads the '.'
            //as empty white tiles, while '#' as walls. Then the file is read and at the same time everything is loaded onto the tiles array
            StreamReader file = new StreamReader(pPath);
            if (file != null)
            {
                string line = file.ReadLine();
                int x = 0;
                while (line != null)
                {
                    line = file.ReadLine();
                    if (line != null)
                    {
                        for (int y = 0; y < line.Length; y++)
                        {
                            char tile = line[y];
                            if (tile == '.') mCells[y, x] = 0;
                            else mCells[y, x] = 1;
                        }
                    }
                    x++;
                }
            }
        }

    }
}
