using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.Aseprite.Demo
{
    public class InputManager
    {
        private KeyboardState _prevState;
        private KeyboardState _curState;
        private Keys[] _moveLeftKeys;
        private Keys[] _moveRightKeys;
        private Keys[] _moveUpKeys;
        private Keys[] _crouchKeys;

        private Keys _action1Key;
        private Keys _action2Key;
        private Keys _action3Key;
        private Keys _action4Key;

        private Keys _exitGamePlayKey;

        public InputManager()
        {
            _prevState = new KeyboardState();
            _curState = Keyboard.GetState();

            _moveLeftKeys = new Keys[] { Keys.Left, Keys.A };
            _moveRightKeys = new Keys[] { Keys.Right, Keys.D };
            _moveUpKeys = new Keys[] { Keys.Up, Keys.W };
            _crouchKeys = new Keys[] { Keys.Down, Keys.S };

            _action1Key = Keys.Space;
        }

        public void Update()
        {
            _prevState = _curState;
            _curState = Keyboard.GetState();
        }


        public bool KeyCheck(Keys key) => _curState.IsKeyDown(key);
        public bool KeyPressed(Keys key) => _curState.IsKeyDown(key) && _prevState.IsKeyUp(key);
        public bool KeyReleased(Keys key) => _curState.IsKeyUp(key) && _prevState.IsKeyDown(key);

        public bool MoveLeftCheck()
        {
            for (int i = 0; i < _moveLeftKeys.Length; i++)
            {
                if (KeyCheck(_moveLeftKeys[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public bool MoveRightCheck()
        {
            for (int i = 0; i < _moveRightKeys.Length; i++)
            {
                if (KeyCheck(_moveRightKeys[i]))
                {
                    return true;
                }
            }
            return false;
        }


        public bool CrouchCheck()
        {
            for(int i = 0; i < _crouchKeys.Length; i++)
            {
                if(KeyCheck(_crouchKeys[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Action1Pressed() => KeyPressed(_action1Key);
        public bool Action2Pressed() => KeyPressed(_action2Key);
        public bool Action3Pressed() => KeyPressed(_action3Key);
        public bool Action4Pressed() => KeyPressed(_action4Key);

        public bool Action1Released() => KeyReleased(_action1Key);
        public bool Action2Released() => KeyReleased(_action2Key);
        public bool Action3Released() => KeyReleased(_action3Key);
        public bool Action4Released() => KeyReleased(_action4Key);

        public bool IdleCheck()
        {
            return _curState.GetPressedKeyCount() == 0;
        }



    }
}
