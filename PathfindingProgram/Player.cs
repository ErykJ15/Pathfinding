using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PathfindingProgram
{
    public class Player
    {
        public Vector2 mGridPosition;
        public Texture2D mTexture;
        //the target simply has a texture and position as attributes. The "gridToScreenPosition" method returns the real position used in the draw function

        public Player(int pX, int pY)
        {
            mGridPosition = new Vector2(pX, pY);
        }

        public Vector2 gridToScreenPosition()
        {
            return (mGridPosition * 15) + ((mGridPosition * 15) - (mGridPosition * 15));
        }
    }
}