using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TestBuild.Code;

namespace TestBuild
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

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
            CollisionHandler.CollisionLoader(2000, 2000);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            var peasantWithSpear = Content.Load<Texture2D>("Units/PeasantWithSpear");
            DataLoader.TextureListAdd(peasantWithSpear, "peasantWithSpear");
            var sandBG = Content.Load<Texture2D>("BG/SandBG");
            DataLoader.TextureListAdd(sandBG, "sandBG");


            for(int i  = 0; i < 10; i++)
            {
                DataLoader.CreatePeasantWithSpear(new Vector2(100*i, 100));
            }
        }
        private List<GameObjects> _potentialCollisions;
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
            _potentialCollisions = CollisionHandler.CollisionUpdater();
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

            foreach(CommonUnits commonUnits in DataLoader.PeasantsWithSpear)
            {
                _spriteBatch.Draw(commonUnits.image, commonUnits.absolutePosition, new Rectangle(0, 0, (int)commonUnits.imageSize.X, (int)commonUnits.imageSize.Y), Color.White);
            }
            foreach(GameObjects unit in _potentialCollisions)
            {
                _spriteBatch.Draw(unit.image, unit.absolutePosition, new Rectangle(0, 0, (int)unit.imageSize.X, (int)unit.imageSize.Y), Color.Red);
            }

            DrawStrokePanel();
            _spriteBatch.End();
            _spriteBatch.Begin();
            DrawStrokePanel();
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

        private Vector2 mousePosition = new Vector2(0, 0);

        // Переменные для хранения масштаба
        private Vector2 _mapPosition = new Vector2(0,0);
        private float _zoom = 1.0f;
        private Vector2 _lastMousePosition;
        private bool _isPanning = false;
        private MouseState _previousMouseState;
        private bool _frameIt = false;
        private Vector2 _frameItFirstPosition = new Vector2(0, 0);
        protected void CheckMouse()
        {
            var mouseState = Mouse.GetState();
            mousePosition = new Vector2(mouseState.X, mouseState.Y);
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

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (!_frameIt)
                {
                    _frameItFirstPosition = mousePosition;
                }
                _frameIt = true;
            }
            else
            {
                _frameIt = false;
            }

            _previousMouseState = mouseState;
        }
        private Rectangle _strokePanel = new Rectangle();
        protected void DrawStrokePanel()
        {
            if (_frameIt)
            {
                int strokePanelx;
                int strokePanely;
                int strokePanelHeight;
                int strokePanelWidth;
                if (mousePosition.X > _frameItFirstPosition.X)
                {
                    strokePanelx = (int)_frameItFirstPosition.X;
                    strokePanelHeight = (int)mousePosition.X - (int)_frameItFirstPosition.X;
                }
                else
                {
                    strokePanelx = (int)mousePosition.X;
                    strokePanelHeight = -((int)mousePosition.X - (int)_frameItFirstPosition.X);
                }
                if (mousePosition.Y > _frameItFirstPosition.Y)
                {
                    strokePanely = (int)_frameItFirstPosition.Y;
                    strokePanelWidth = (int)mousePosition.Y - (int)_frameItFirstPosition.Y;
                }
                else
                {
                    strokePanely = (int)mousePosition.Y;
                    strokePanelWidth = -((int)mousePosition.Y - (int)_frameItFirstPosition.Y);
                }
                _strokePanel = new Rectangle(strokePanelx, strokePanely, strokePanelHeight, strokePanelWidth);
                _spriteBatch.DrawRectangle(_strokePanel, Color.White);
            }
        }
    }
}
