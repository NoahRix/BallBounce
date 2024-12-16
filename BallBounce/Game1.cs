using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;

namespace BallBounce
{
    public class Ball
    {
        public Vector2 _position;
        public Vector2 _velocity;
        public float _radius;
        public Color _color;
        public int _diameter
        {
            get { return Convert.ToInt32(_radius * 2); }
        }

        public Ball(Vector2 position, Vector2 velocity, float radius, Color color)
        {
            if (true)
            {
                _position = position;
                _velocity = velocity;
                _radius = 10f;
                _color = color;
            }
            else
            {
                _position = position;
                _velocity = velocity;
                _radius = radius;
                _color = color;
            }
        }
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private List<Ball> _balls;
        private Random _random;
        private int _screenWidth = 800;
        private int _screenHeight = 800;
        private bool _isFullScreen;
        private Texture2D _texture2D;

        private List<Color> _colors = new List<Color>
        {
            Color.White,
            Color.Red,
            Color.Green,
            Color.Blue,
            Color.Cyan,
            Color.Yellow,
            Color.Magenta,
        };

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = false,
                PreferredBackBufferWidth = 800,
                PreferredBackBufferHeight = 800
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 60.0); // Default 60 FPS
            IsFixedTimeStep = false; // Allow dynamic FPS
        }

        protected override void Initialize()
        {
            base.Initialize();

            int diameter = 20;
            Color color = Color.White;

            _texture2D = new Texture2D(_graphics.GraphicsDevice, diameter, diameter);
            Color[] data = new Color[diameter * diameter];
            Vector2 center = new Vector2(diameter / 2);

            for (int y = 0; y < diameter; y++)
            {
                for (int x = 0; x < diameter; x++)
                {
                    var distance = Vector2.Distance(new Vector2(x, y), center);
                    if (distance <= diameter / 2)
                    {
                        data[y * diameter + x] = color;
                    }
                    else
                    {
                        data[y * diameter + x] = Color.Transparent;
                    }
                }
            }

            _texture2D.SetData(data);

            _random = new Random();
            _balls = new List<Ball>();
            InitializeBalls();
        }
        private void InitializeBalls()
        {
            int ballCount = 1000;

            Vector2 centerPosition = new Vector2(_screenWidth / 2f, _screenHeight / 2f);

            for (int i = 0; i < ballCount; i++)
            {
                Vector2 velocity = new Vector2(
                    _random.Next(-200, 200) / 100f,
                    _random.Next(-200, 200) / 100f
                );

                // Color color = new Color(_random.Next(256), _random.Next(256), _random.Next(256));
                // Color color = Color.White;

                Color color = _colors[_random.Next(_colors.Count)];

                _balls.Add(new Ball(centerPosition, velocity, .11f, color));
            }
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            // long memoryStart = GC.GetTotalMemory(true);

            KeyboardState keyboardState = Keyboard.GetState();

            // Exit with Escape
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // Toggle Fullscreen with F11 or Alt + Enter
            if (keyboardState.IsKeyDown(Keys.F11) || (keyboardState.IsKeyDown(Keys.LeftAlt) && keyboardState.IsKeyDown(Keys.Enter)))
            {
                ToggleFullScreen();
            }

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (var ball in _balls)
            {
                ball._position += ball._velocity * 200 * deltaTime; // Adjust speed

                // Bounce off walls logic, handle all 4 cardinal directions
                // |
                // V

                if (ball._position.X <= 0)
                {
                    ball._velocity.X = Math.Abs(ball._velocity.X);
                    //return;
                }

                if (ball._position.X + ball._diameter >= _screenWidth)
                {
                    ball._velocity.X = Math.Abs(ball._velocity.X) * -1;
                    //return;
                }

                if (ball._position.Y <= 0)
                {
                    ball._velocity.Y = Math.Abs(ball._velocity.Y);
                    //return;
                }

                if (ball._position.Y + ball._diameter >= _screenHeight)
                {
                    ball._velocity.Y = Math.Abs(ball._velocity.Y) * -1;
                    //return;
                }
            }

            base.Update(gameTime);

            // long memoryEnd = GC.GetTotalMemory(true);
        }

        private void ToggleFullScreen()
        {
            float oldWidth = _screenWidth;
            float oldHeight = _screenHeight;

            _isFullScreen = !_isFullScreen;
            _graphics.IsFullScreen = _isFullScreen;

            if (_isFullScreen)
            {
                _screenWidth = GraphicsDevice.DisplayMode.Width;
                _screenHeight = GraphicsDevice.DisplayMode.Height;
            }
            else
            {
                _screenWidth = 800;
                _screenHeight = 800;
            }

            _graphics.PreferredBackBufferWidth = _screenWidth;
            _graphics.PreferredBackBufferHeight = _screenHeight;
            _graphics.ApplyChanges();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            
            foreach (var ball in _balls)
            {
                _spriteBatch.Draw(_texture2D, ball._position, ball._color);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
