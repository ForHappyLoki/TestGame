using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Net.NetworkInformation;
using TestBuild.Code;

namespace TestBuild
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Texture2D peasantWithSpear;
        public Texture2D sandBG;
        Vector2 position = new Vector2(110, 110);
        Vector2 camPosition = new Vector2(0, 0);

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            // Устанавливаем полноэкранный режим
            _graphics.IsFullScreen = true;

            // Устанавливаем разрешение экрана на разрешение дисплея
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            // Применяем изменения
            _graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            peasantWithSpear = Content.Load<Texture2D>("Units/PeasantWithSpear");
            DataLoader.TextureListAdd(peasantWithSpear, "peasantWithSpear");
            var sandBG = Content.Load<Texture2D>("BG/SandBG");
            DataLoader.TextureListAdd(sandBG, "sandBG");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            //angle += 0.01f;

            KeyboardState state = Keyboard.GetState();
            bool leftArrowKeyDown = state.IsKeyDown(Keys.Left);

            if (leftArrowKeyDown)
            {
                position = new Vector2(position.X - 1, position.Y);
            }

            CheckMouse();

            base.Update(gameTime);
        }
        private Vector2 cursorPosition;
        Matrix _cameraTransform = Matrix.Identity;
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if(_mapPosition.X > 0) { _mapPosition.X = 0; }
            if (_mapPosition.Y > 0) { _mapPosition.Y = 0; }
            _spriteBatch.Begin(transformMatrix: Matrix.CreateTranslation(_mapPosition.X, _mapPosition.Y, 0) * Matrix.CreateScale(_zoom));

            DrawBG();

            Vector2 origin = new Vector2(0, 0);
            _spriteBatch.Draw(peasantWithSpear, position, new Rectangle(0, 0, 64, 128), Color.White);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
        protected void DrawBG()
        {
            var bgSand = DataLoader.ReturnFragmentBGSand();
            for (int j = 0; j < 9; j++)
            {
                for(int i = 0; i < 15; i++)
                {
                    var pos = new Vector2(128 * i, 128 * j);
                    _spriteBatch.Draw(bgSand.image, pos, new Rectangle( 0, 0, (int)bgSand.imageSize.X, (int)bgSand.imageSize.Y), Color.White);
                }
            }
        }

        private bool centerFirstMouseButton = true;
        private bool centerMouseButton = false;
        Vector2 oldCamPosition = new Vector2(0, 0);
        Vector2 newCamPosition = new Vector2(0, 0);

        // Переменные для хранения масштаба
        private Vector2 _mapPosition = new Vector2(0,0);
        private float _zoom = 1.0f;
        private Vector2 _lastMousePosition;
        private bool _isPanning = false;
        private MouseState _previousMouseState;
        protected void CheckMouse()
        {
            var mouseState = Mouse.GetState();
            var mousePosition = new Vector2(mouseState.X, mouseState.Y);
            var screenCenter = new Vector2(_graphics.PreferredBackBufferWidth / 2f, _graphics.PreferredBackBufferHeight / 2f);

            // Обработка масштабирования
            float scrollDelta = mouseState.ScrollWheelValue - _previousMouseState.ScrollWheelValue;
            if (scrollDelta != 0)
            {
                // Позиция центра экрана в мировых координатах до изменения масштаба
                Vector2 worldBeforeZoom = (screenCenter - _mapPosition) / _zoom;

                // Изменение масштаба
                float previousZoom = _zoom;
                _zoom += scrollDelta * 0.001f; // Настройте коэффициент для нужной скорости масштабирования
                _zoom = MathHelper.Clamp(_zoom, 0.1f, 1f);

                // Корректировка позиции карты
                _mapPosition = screenCenter - worldBeforeZoom * _zoom;
            }

            // Обработка перемещения
            if (mouseState.MiddleButton == ButtonState.Pressed)
            {
                if (!_isPanning)
                {
                    _isPanning = true;
                    _lastMousePosition = mousePosition;
                }
                else
                {
                    Vector2 delta = mousePosition - _lastMousePosition;
                    _mapPosition += delta / _zoom; // Скорость перемещения зависит от масштаба
                    _lastMousePosition = mousePosition;
                }
            }
            else
            {
                _isPanning = false;
            }

            _previousMouseState = mouseState;
        }
    }
}
