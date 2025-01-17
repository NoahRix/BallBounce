using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Windows.Forms.VisualStyles;

namespace BallBounce
{
    /// <summary>
    /// Ball class to represent each ball in disregard to the texture rendering from the main game class.
    /// </summary>
    public class Ball
    {
        public Vector2 _position;
        public Vector2 _velocity;
        public Color _color;
        public int _radius;
        public int _diameter;

        public Ball(Vector2 position, Vector2 velocity, int radius, Color color)
        {
            _position = position;
            _velocity = velocity;
            _radius = radius;
            _diameter = _radius * 2;
            _color = color;
        }
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;    // Grapics device manager that should be global in the sense of this app.
        private SpriteBatch _spriteBatch;           // The sprites to render in Draw()
        private List<Ball> _balls;                  // All circles to render as a collection.
        private Random _random;                     
        private int _screenWidth = 800;             // If windowed, then set the screen resolution to this
        private int _screenHeight = 800;            // ↓
        private bool _isFullScreen;
        private Texture2D _texture2D;               // Each ball (circle rendered shape) should reference the same "global" texture for optimal GPU efficiency

        // Lookup table for the centralized ball texture color.
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

            ToggleFullScreen(); // If you want the app to start in fullscreen on start.

            Color color = Color.White;
            int diameter = 20;

            _texture2D = new Texture2D(_graphics.GraphicsDevice, diameter, diameter);
            Color[] data = new Color[diameter * diameter];
            Vector2 center = new Vector2(diameter / 2);

            for (int y = 0; y < diameter; y++)
            {
                for (int x = 0; x < diameter; x++)
                {
                    // Distance from the center
                    float dx = x - diameter / 2;
                    float dy = y - diameter / 2;
                    float distanceSquared = dx * dx + dy * dy;

                    if (distanceSquared <= diameter / 2 * diameter / 2)
                    {
                        // Inside the circle
                        float distance = MathF.Sqrt(distanceSquared);
                        float alpha = MathF.Max(0, MathF.Min(1, ((diameter / 2) - distance) / 1.5f)); // Smooth edge
                        data[y * diameter + x] = color * alpha; // Apply alpha blending
                    }
                    else
                    {
                        // Outside the circle
                        data[y * diameter + x] = Color.Transparent;
                    }
                }
            }

            _texture2D.SetData(data);

            _random = new Random();
            _balls = new List<Ball>();
             InitializeBalls(diameter);
        }
        private void InitializeBalls(int diameter)
        {
            int ballCount = 100;

            Vector2 centerPosition = new Vector2(_screenWidth / 2f, _screenHeight / 2f);

            for (int i = 0; i < ballCount; i++)
            {
                Vector2 velocity = new Vector2(
                    _random.Next(-200, 200) / 100f,
                    _random.Next(-200, 200) / 100f
                );

                Color color = _colors[_random.Next(_colors.Count)];

                _balls.Add(new Ball(centerPosition, velocity, diameter / 2, color));
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
                ball._position += ball._velocity * 100 * deltaTime; // Adjust speed

                // Bounce off walls logic, handle all 4 cardinal directions
                // |
                // V

                if (ball._position.X <= 0)
                {
                    ball._velocity.X = Math.Abs(ball._velocity.X);
                }

                if (ball._position.X >= _screenWidth - ball._diameter)
                {
                    ball._velocity.X = Math.Abs(ball._velocity.X) * -1;
                }

                if (ball._position.Y <= 0)
                {
                    ball._velocity.Y = Math.Abs(ball._velocity.Y);
                }

                if (ball._position.Y >= _screenHeight - ball._diameter)
                {
                    ball._velocity.Y = Math.Abs(ball._velocity.Y) * -1;
                }
            }

            base.Update(gameTime);
        }

        private void ToggleFullScreen()
        {
            _isFullScreen = !_isFullScreen;
            _graphics.IsFullScreen = _isFullScreen;
            _graphics.HardwareModeSwitch = true; // Force exclusive full-screen

            if (_isFullScreen)
            {
                _screenWidth = _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                _screenHeight = _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                IsMouseVisible = false;
            }
            else
            {
                IsMouseVisible = true;
                _screenWidth = _graphics.PreferredBackBufferWidth = 800;
                _screenHeight = _graphics.PreferredBackBufferHeight = 800;
            }

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
