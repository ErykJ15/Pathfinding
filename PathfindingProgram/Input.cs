using Microsoft.Xna.Framework.Input;

namespace PathfindingProgram
{
    public static class Input
    {
        //this class is used to improve and make more efficient reading of user inputs
        private static KeyboardState mKeyboardState = Keyboard.GetState();
        private static KeyboardState mLastKeyboardState;

        public static void Update()
        {
            mLastKeyboardState = mKeyboardState;
            mKeyboardState = Keyboard.GetState();
        }

        public static bool IsKeyDown(Keys pInput)
        {
            return mKeyboardState.IsKeyDown(pInput);
        }

        public static bool IsKeyUp(Keys pInput)
        {
            return mKeyboardState.IsKeyUp(pInput);
        }

        public static bool KeyPressed(Keys pInput)
        {
            if (mKeyboardState.IsKeyDown(pInput) == true && mLastKeyboardState.IsKeyDown(pInput) == false)
                return true;
            else
                return false;
        }

    }

}