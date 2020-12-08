/* ------------------------------------------------------------------------------
    Copyright (c) 2020 Christopher Whitley

    Permission is hereby granted, free of charge, to any person obtaining
    a copy of this software and associated documentation files (the
    "Software"), to deal in the Software without restriction, including
    without limitation the rights to use, copy, modify, merge, publish,
    distribute, sublicense, and/or sell copies of the Software, and to
    permit persons to whom the Software is furnished to do so, subject to
    the following conditions:
    
    The above copyright notice and this permission notice shall be
    included in all copies or substantial portions of the Software.
    
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
    LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
    OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
    WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
------------------------------------------------------------------------------ */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Aseprite.Documents;
using MonoGame.Aseprite.Graphics;

namespace MonoGame.Aseprite.Demo
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _pixel;
        private Rectangle _rect;
        private Point _gridCellSize;
        private Color _gridColorA;
        private Color _gridColorB;
        private int _gridColumnCount;
        private int _gridRowCount;
        private KeyboardState _prevKeyState;
        private KeyboardState _curKeyState;



        private Point _resolution;      //  The resolution of our game.
        private AnimatedSprite _sprite; //  The animated sprite of our character.
        private bool _playerCanMove;    //  Can the player move.

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            //  Set the game resolution
            _resolution = new Point(1280, 720);
            _graphics.PreferredBackBufferWidth = _resolution.X;
            _graphics.PreferredBackBufferHeight = _resolution.Y;
            _graphics.ApplyChanges();

            //  Setup the keyboard states
            _prevKeyState = new KeyboardState();
            _curKeyState = Keyboard.GetState();

            //  Instantiate the values for the grid
            _gridCellSize = new Point(64, 64);
            _gridColumnCount = (int)Math.Ceiling(_resolution.X / (float)_gridCellSize.X);
            _gridRowCount = (int)Math.Ceiling(_resolution.Y / (float)_gridCellSize.Y);
            _gridColorA = new Color(192, 192, 192, 255);
            _gridColorB = new Color(128, 128, 128, 255);

            //  Player can initially move
            _playerCanMove = true;

            base.Initialize();





        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //  Create the pixel texure for the grid rendering
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData<Color>(new Color[] { Color.White });

            //  Load the asprite file from the content pipeline.
            AsepriteDocument aseprite = Content.Load<AsepriteDocument>("adventurer");

            //  Create a new aniamted sprite instance using the aseprite doucment loaded.
            _sprite = new AnimatedSprite(aseprite);
            _sprite.Scale = new Vector2(5.0f, 5.0f);
            _sprite.Y = _resolution.Y - (_sprite.Height * _sprite.Scale.Y) - 16;
        }

        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //  Update our input states
            _prevKeyState = _curKeyState;
            _curKeyState = Keyboard.GetState();


            //  Check if the player is crouched down
            bool isCrouched = KeyCheck(Keys.Down) || KeyCheck(Keys.S);

            //  Calculate the player movement speed.
            float moveSpeed = isCrouched ? 100.0f : 300.0f;

            //  The following input actions can only be checked if
            //  the player input is active
            if (_playerCanMove)
            {
                if (KeyCheck(Keys.Left) || KeyCheck(Keys.A))
                {
                    _sprite.X -= moveSpeed * deltaTime;
                    _sprite.SpriteEffect = SpriteEffects.FlipHorizontally;
                    _sprite.Play(isCrouched ? "crouch-walk" : "run");
                }
                else if (KeyCheck(Keys.Right) || KeyCheck(Keys.D))
                {
                    _sprite.X += moveSpeed * deltaTime;
                    _sprite.SpriteEffect = SpriteEffects.None;
                    _sprite.Play(isCrouched ? "crouch-walk" : "run");
                }
                else if (isCrouched)
                {
                    _sprite.Play("crouch");
                }
                else if (AreNoKeysPressed())
                {
                    _sprite.Play("idle");
                }


                if (KeyPressed(Keys.Space))
                {
                    _sprite.Play("attack3");
                    _playerCanMove = false;

                    _sprite.OnAnimationLoop = () =>
                    {
                        _sprite.Play("idle");
                        _playerCanMove = true;
                        _sprite.OnAnimationLoop = null;
                    };

                }

            }

            _sprite.Update(deltaTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            DrawGrid();
            _sprite.Render(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawGrid()
        {
            for (int c = 0; c < _gridColumnCount; c++)
            {
                for (int r = 0; r < _gridRowCount; r++)
                {
                    _rect.X = c * _gridCellSize.X;
                    _rect.Y = r * _gridCellSize.Y;
                    _rect.Width = _gridCellSize.X;
                    _rect.Height = _gridCellSize.Y;

                    if ((c % 2 == 0 && r % 2 == 0) || (c % 2 != 0 && r % 2 != 0))
                    {
                        _spriteBatch.Draw(_pixel, _rect, _gridColorA);
                    }
                    else
                    {
                        _spriteBatch.Draw(_pixel, _rect, _gridColorB);
                    }
                }
            }
        }


        // ---------------------------------------------------------------
        //
        //  Input Helper Functions
        //
        // ---------------------------------------------------------------

        /// <summary>
        ///     Given a key, checks if it is pressed down.
        /// </summary>
        /// <param name="key">
        ///     The key to check.
        /// </param>
        /// <returns>
        ///     True if the key is pressed down; otherwise, false.
        /// </returns>
        private bool KeyCheck(Keys key)
        {
            return _curKeyState.IsKeyDown(key);
        }

        /// <summary>
        ///     Given a key, returns if the key was just pressed.
        /// </summary>
        /// <param name="key">
        ///     The key to check.
        /// </param>
        /// <returns>
        ///     True if the key was pressed first on the current frame;
        ///     otherwise, false.
        /// </returns>
        private bool KeyPressed(Keys key)
        {
            return _curKeyState.IsKeyDown(key) && _prevKeyState.IsKeyUp(key);
        }

        private bool AreNoKeysPressed()
        {
            return _curKeyState.GetPressedKeyCount() == 0;
        }
    }
}
