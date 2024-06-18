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

            MouseState mouseState = Mouse.GetState();
            CheckMouse(mouseState, gameTime);

            base.Update(gameTime);
        }
        private Vector2 cursorPosition;
        Matrix _cameraTransform = Matrix.Identity;
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            // Применяем матрицу преобразования камеры
            _spriteBatch.Begin(transformMatrix: _cameraTransform);

            DrawBG();

            Vector2 origin = new Vector2(0, 0);
            _spriteBatch.Draw(peasantWithSpear, CameraOffset(position), new Rectangle(0, 0, 64, 128), Color.White);

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
                    var pos = CameraOffset(128 * i, 128 * j);
                    _spriteBatch.Draw(bgSand.image, pos, new Rectangle( 0, 0, (int)bgSand.imageSize.X, (int)bgSand.imageSize.Y), Color.White);
                }
            }
        }
        //Тестовая надпись!
        private bool centerFirstMouseButton = true;
        private bool centerMouseButton = false;
        Vector2 oldCamPosition = new Vector2(0, 0);
        Vector2 newCamPosition = new Vector2(0, 0);

        // Переменные для хранения масштаба
        float cameraScale = 1.0f;
        const float minCameraScale = 0.1f;
        const float maxCameraScale = 2.0f;
        const float cameraScaleStep = 0.1f;
        private int scrollValue = 0;
        private int oldScrollValue = 0;
        private float cameraLerpFactor = 0.2f; // Коэффициент интерполяции
        private float cameraScaleMultiplier = 1.0f;
        protected void CheckMouse(MouseState mouseState, GameTime gameTime)
        {
            cursorPosition = new Vector2(mouseState.X, mouseState.Y);
            if (mouseState.MiddleButton == ButtonState.Pressed)
            {
                centerMouseButton = true;
                if (centerFirstMouseButton)
                {
                    oldCamPosition = new Vector2(mouseState.X, mouseState.Y);
                    centerFirstMouseButton = false;
                }
            }
            else
            {
                centerMouseButton = false;
                centerFirstMouseButton = true;
                camPosition = newCamPosition;
            }
            if (centerMouseButton)
            {
                cameraScaleMultiplier = MathHelper.Clamp(cameraScale, minCameraScale, maxCameraScale) / 1.0f;
                newCamPosition.X = MathHelper.Lerp(newCamPosition.X, camPosition.X - (oldCamPosition.X - mouseState.X) * cameraScaleMultiplier, cameraLerpFactor);
                newCamPosition.Y = MathHelper.Lerp(newCamPosition.Y, camPosition.Y - (oldCamPosition.Y - mouseState.Y) * cameraScaleMultiplier, cameraLerpFactor);
            }
            // Если колесо мыши прокручено
            scrollValue = mouseState.ScrollWheelValue;
            if (scrollValue != oldScrollValue)
            {
                // Увеличиваем или уменьшаем масштаб камеры на фиксированное значение
                if (scrollValue > oldScrollValue)
                {
                    cameraScale = MathHelper.Clamp(cameraScale + cameraScaleStep, minCameraScale, maxCameraScale);
                    oldScrollValue = scrollValue;
                }
                else if (scrollValue < oldScrollValue)
                {
                    cameraScale = MathHelper.Clamp(cameraScale - cameraScaleStep, minCameraScale, maxCameraScale);
                    oldScrollValue = scrollValue;
                }


                // Вычисляем смещение камеры, чтобы она была центрирована относительно курсора
                float translationX = (GraphicsDevice.Viewport.Width / 20.0f) - (cursorPosition.X * cameraScale);
                float translationY = (GraphicsDevice.Viewport.Height / 20.0f) - (cursorPosition.Y * cameraScale);

                // Создаем матрицу преобразования для камеры
                _cameraTransform = Matrix.CreateTranslation(translationX, translationY, 0) *
                                   Matrix.CreateScale(cameraScale, cameraScale, 1);
            }
        }
        protected Vector2 CameraOffset(int xOld, int yOld)
        {
            return new Vector2(xOld + (int)newCamPosition.X, yOld + (int)newCamPosition.Y);
        }
        protected Vector2 CameraOffset(Vector2 vector2)
        {
            return new Vector2(vector2.X + (int)newCamPosition.X, vector2.Y + (int)newCamPosition.Y);
        }
    }
}
