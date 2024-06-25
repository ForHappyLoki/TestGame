using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Reflection;
using TestBuild.Code;
using System.Linq;

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

            for (int j = 0; j < 20; j++)
            {
                for(int i  = 0; i < 20; i++)
                {
                    DataLoader.CreatePeasantWithSpear(new Vector2(100*i, 100 + 100*j));
                }
            }
        }
        private List<GameObjects[]> _potentialCollisions;
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            CheckKeyboard();
            CheckMouse();
            if (_rightButtonON)
            {
                if (DataLoader.SELECT_UNITS.Count > 0)
                {
                    if (!groupAccommodationMode)
                    {
                        groupAccommodationMode = true;
                        DataLoader.RectangleForSelectUnitsCreator();
                    }
                    GroupAccommodationMode();
                }
            }
            else
            {
                groupAccommodationMode = false;
            }
            //Кусок ниже снимает селект со всех юнитов, выборка селекта ниже
            if (_deselectionUnits || _frameIt)
            {
                _deselectionUnits = false;
                DataLoader.GAME_OBJECTS.OfType<Units>()
                    .ToList()
                    .ForEach(b => b.UnselectingUnit());
            }
            if (_orderActivation)
            {
                //_orderActivation = false;
                //DataLoader.GAME_OBJECTS.OfType<Units>()
                //    .Where(b => b.SelectingReturn())
                //    .ToList()
                //    .ForEach(b => b.SetTargerToMove(_orderPosition));
            }
            DataLoader.GAME_OBJECTS.OfType<Units>()
                .ToList()
                .ForEach(b => b.UnitUpdate());
            _potentialCollisions = CollisionHandler.CollisionUpdater();
            //Кусок ниже перебирает все пересечения
            foreach (var collision in _potentialCollisions)
            {
                bool findStrokePanel = false;
                bool findUnits = false;
                Units unit = null;
                foreach (var obj in collision)
                {
                    if (obj is StrokePanel)
                    {
                        findStrokePanel = true;
                    }
                    if (obj is Units)
                    {
                        findUnits = true;
                        unit = obj as Units;
                    }
                }
                //Кусок ниже селектит юнита, если он обведен в рамочку
                if (findStrokePanel && findUnits && unit != null)
                {
                    unit.SelectingUnit();
                }
            }
            DataLoader.UNIT_OBJECTS.Sort((x, y) => x.absolutePosition.Y.CompareTo(y.absolutePosition.Y));
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
            
            foreach(Units gameObjects in DataLoader.UNIT_OBJECTS)
            {
                Color color;
                if (gameObjects.SelectingReturn())
                {
                    color = Color.Red;
                }
                else
                {
                    color = Color.White;
                }
                _spriteBatch.Draw(gameObjects.image, gameObjects.absolutePosition, new Rectangle(0, 0, (int)gameObjects.imageSize.X, (int)gameObjects.imageSize.Y), color);
            }

            _spriteBatch.End();
            /////////
            _spriteBatch.Begin();
            DrawStrokePanel();
            foreach(Units gameObjects in DataLoader.UNIT_OBJECTS)
            {
                if (gameObjects.SelectingReturn())
                {
                    if (gameObjects._onMove)
                    {
                    _spriteBatch.DrawRectangle(new Rectangle((int)((gameObjects.targetRectangle.rectangle.X + _mapPosition.X) * _zoom), (int)((gameObjects.targetRectangle.rectangle.Y + _mapPosition.Y) * _zoom),
                        (int)(gameObjects.targetRectangle.rectangle.Width * _zoom), (int)(gameObjects.targetRectangle.rectangle.Height * _zoom)), Color.Green);
                    }
                    _spriteBatch.DrawRectangle(new Rectangle((int)((gameObjects.collisionRectangle.X + _mapPosition.X) * _zoom), (int)((gameObjects.collisionRectangle.Y + _mapPosition.Y) * _zoom),
                        (int)(gameObjects.collisionRectangle.Width * _zoom), (int)(gameObjects.collisionRectangle.Height * _zoom)), Color.White);
                }
            }
            if (groupAccommodationMode)
            {
                foreach (RectangleForSelectUnits rect in DataLoader.RECTANGLE_FOR_SELECT_UNITS)
                {
                    Console.WriteLine(rect.ToString());
                    _spriteBatch.DrawRectangle(new Rectangle((int)((rect.rectangle.X + _mapPosition.X)*_zoom), (int)((rect.rectangle.Y + _mapPosition.Y) * _zoom),
                        (int)(rect.rectangle.Width*_zoom), (int)(rect.rectangle.Height * _zoom)), Color.White);
                }
            }
            _spriteBatch.End();
            /////////
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

        private bool centerMouseButton = false;
        Vector2 oldCamPosition = new Vector2(0, 0);
        Vector2 newCamPosition = new Vector2(0, 0);

        protected void CheckKeyboard()
        {
            KeyboardState state = Keyboard.GetState();
            bool leftArrowKeyDown = state.IsKeyDown(Keys.Left);
        }
        // Переменные управления мышью
        // Переменные для хранения масштаба
        private Vector2 mousePosition = new Vector2(0, 0);

        private Vector2 _mapPosition = new Vector2(0,0);
        private float _zoom = 1.0f;
        private Vector2 _lastMousePosition;
        private bool _isPanning = false;
        private MouseState _previousMouseState;
        private bool _frameIt = false;
        private Vector2 _frameItFirstPosition = new Vector2(0, 0);
        private bool _deselectionUnits = false;
        private Vector2 _orderPosition = new Vector2(0, 0);
        private bool _orderActivation = false;
        private bool _rightButtonON = false;
        private Vector2 _startGroupAccommodationModePosition;
        private Vector2 _endGroupAccommodationModePosition;
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
                    _deselectionUnits = true;
                    _frameItFirstPosition = mousePosition;
                }
                _frameIt = true;
            }
            else if (mouseState.LeftButton != ButtonState.Pressed)
            {
                _frameIt = false;
                _strokePanel.CollisionRectangleUpdate(new Rectangle(0, 0, 0, 0));
            }

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                _rightButtonON = true;
                if (DataLoader.SELECT_UNITS.Count > 0)
                {
                    _orderPosition = (mousePosition / _zoom - _mapPosition);
                    if (!_orderActivation)
                    {
                        _startGroupAccommodationModePosition = _orderPosition;
                    }
                    _endGroupAccommodationModePosition = _orderPosition;
                    _orderActivation = true;
                }
            }    
            else
            {
                if (_rightButtonON && _orderActivation && groupAccommodationMode)
                {
                    GiveTheOrder();
                }
                _rightButtonON = false;
                _orderActivation = false;
            }
            _previousMouseState = mouseState;
        }
        private Rectangle _trueStrokePanel = new Rectangle();
        private StrokePanel _strokePanel = new StrokePanel(new Rectangle());
        private bool selectedIsActivity;
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
                _trueStrokePanel = new Rectangle((int)(strokePanelx /_zoom - _mapPosition.X ), (int)(strokePanely / _zoom - _mapPosition.Y ), 
                    (int)(strokePanelHeight/ _zoom), (int)(strokePanelWidth/ _zoom));
                _strokePanel.CollisionRectangleUpdate(_trueStrokePanel);
                var strokePanel = new Rectangle(strokePanelx, strokePanely, strokePanelHeight, strokePanelWidth);
                _spriteBatch.DrawRectangle(strokePanel, Color.White);
            }
        }
        private bool groupAccommodationMode = false;
        private float accommodationDistance;
        private bool accommodationAngleHorizontal = true;
        public void GroupAccommodationMode()
        {
            accommodationDistance = Vector2.Distance(_startGroupAccommodationModePosition, _endGroupAccommodationModePosition);
            float accommodationAngle = HelpMethods.AngleBetweenPointsDegree(_startGroupAccommodationModePosition, _endGroupAccommodationModePosition); 
            float a = (_endGroupAccommodationModePosition.Y - _startGroupAccommodationModePosition.Y) / (_endGroupAccommodationModePosition.X - _startGroupAccommodationModePosition.X);
            if (_endGroupAccommodationModePosition.X == _startGroupAccommodationModePosition.X)
            {
                a = 0;
            }
            System.Diagnostics.Debug.WriteLine("a = " + a.ToString());
            float b = _startGroupAccommodationModePosition.Y - a * _startGroupAccommodationModePosition.X;
            if (b == 0)
            {
                b = 0.01f;
            }
            if (_startGroupAccommodationModePosition != _endGroupAccommodationModePosition)
            {
                if ((accommodationAngle < -135 || accommodationAngle > 135) || (accommodationAngle > -45 && accommodationAngle < 45))
                {
                    accommodationAngleHorizontal = true;
                }
                else
                {
                    accommodationAngleHorizontal = false;
                }

                if (accommodationAngleHorizontal)
                {
                    float lineY = 0f;
                    float lineYmodifaer = 0f;
                    float endX;
                    float startX;
                    float currentX;
                    if (_endGroupAccommodationModePosition.X > _startGroupAccommodationModePosition.X)
                    {
                        endX = _endGroupAccommodationModePosition.X;
                        startX = _startGroupAccommodationModePosition.X;
                        currentX = startX;

                        DataLoader.RECTANGLE_FOR_SELECT_UNITS.Sort((r1, r2) => r2.rectangle.Width.CompareTo(r1.rectangle.Width));

                        foreach (var rect in DataLoader.RECTANGLE_FOR_SELECT_UNITS)
                        {
                            lineY = currentX * a + b + lineYmodifaer;

                            rect.SetPosition(new Vector2(currentX - rect.rectangle.Width/2, lineY));

                            currentX += rect.rectangle.Width + 10f; // добавляем небольшой отступ между прямоугольниками
                            if (currentX + rect.rectangle.Width / 2 > endX)
                            {
                                currentX = startX;
                                lineYmodifaer += rect.rectangle.Height + 10f; // добавляем небольшой отступ между линиями
                            }
                        }
                    }
                    else
                    {
                        startX = _endGroupAccommodationModePosition.X;
                        endX = _startGroupAccommodationModePosition.X;
                        currentX = endX;
                        DataLoader.RECTANGLE_FOR_SELECT_UNITS.Sort((r1, r2) => r2.rectangle.Width.CompareTo(r1.rectangle.Width));

                        foreach (var rect in DataLoader.RECTANGLE_FOR_SELECT_UNITS)
                        {
                            lineY = currentX * a + b + lineYmodifaer;

                            rect.SetPosition(new Vector2(currentX - rect.rectangle.Width / 2, lineY));

                            currentX -= rect.rectangle.Width + 10f; // добавляем небольшой отступ между прямоугольниками
                            if (currentX - rect.rectangle.Width / 2 < startX)
                            {
                                currentX = endX;
                                lineYmodifaer += rect.rectangle.Height + 10f; // добавляем небольшой отступ между линиями
                            }
                        }
                    }
                }
                else
                {
                    float lineX = 0f;
                    float lineXmodifaer = 0f;
                    float endY = _endGroupAccommodationModePosition.Y;
                    float startY = _startGroupAccommodationModePosition.Y;
                    if (_endGroupAccommodationModePosition.Y > _startGroupAccommodationModePosition.Y)
                    {
                        endY = _endGroupAccommodationModePosition.Y;
                        startY = _startGroupAccommodationModePosition.Y;
                        float currentY = startY;

                        DataLoader.RECTANGLE_FOR_SELECT_UNITS.Sort((r1, r2) => r2.rectangle.Width.CompareTo(r1.rectangle.Height));

                        foreach (var rect in DataLoader.RECTANGLE_FOR_SELECT_UNITS)
                        {
                            if (a != 0)
                            {
                                lineX = (currentY - b) / a + lineXmodifaer;
                            }
                            else { lineX = _endGroupAccommodationModePosition.X + lineXmodifaer; }

                            rect.SetPosition(new Vector2(lineX - rect.rectangle.Width / 2, currentY));

                            currentY += rect.rectangle.Height + 10f; // добавляем небольшой отступ между прямоугольниками
                            if (currentY + rect.rectangle.Height / 2 > endY)
                            {
                                currentY = startY;
                                lineXmodifaer += rect.rectangle.Width + 10f; // добавляем небольшой отступ между линиями
                            }
                        }
                    }
                    else
                    {
                        startY = _endGroupAccommodationModePosition.Y;
                        endY = _startGroupAccommodationModePosition.Y;
                        float currentY = endY;

                        DataLoader.RECTANGLE_FOR_SELECT_UNITS.Sort((r1, r2) => r2.rectangle.Width.CompareTo(r1.rectangle.Height));

                        foreach (var rect in DataLoader.RECTANGLE_FOR_SELECT_UNITS)
                        {
                            //System.Diagnostics.Debug.WriteLine("lineY " + lineY.ToString());
                            if (a != 0)
                            {
                                lineX = (currentY - b) / a + lineXmodifaer;
                            }
                            else { lineX = _endGroupAccommodationModePosition.X + lineXmodifaer; }

                            rect.SetPosition(new Vector2(lineX - rect.rectangle.Width / 2, currentY));

                            currentY -= rect.rectangle.Height + 10f; // добавляем небольшой отступ между прямоугольниками
                            if (currentY - rect.rectangle.Height / 2 < startY)
                            {
                                currentY = endY;
                                lineXmodifaer += rect.rectangle.Width + 10f; // добавляем небольшой отступ между линиями
                            }
                        }
                    }
                }
            }
            else
            {
                var accommodationDistance = _endGroupAccommodationModePosition - DataLoader.SELECT_UNITS[0].centerOfModel ;
                for(int i = 0; i < DataLoader.SELECT_UNITS.Count; i++)
                {
                    DataLoader.RECTANGLE_FOR_SELECT_UNITS[i].SetPosition(DataLoader.SELECT_UNITS[i].centerOfModel + accommodationDistance);
                }
            }
            if (DataLoader.SELECT_UNITS.Count > 0)
                System.Diagnostics.Debug.WriteLine("DataLoader.SELECT_UNITS[i].centerOfModel = " + DataLoader.SELECT_UNITS[0].centerOfModel.ToString());
                System.Diagnostics.Debug.WriteLine("DataLoader.RECTANGLE_FOR_SELECT_UNITS[0].centralPosition = " + DataLoader.RECTANGLE_FOR_SELECT_UNITS[0].centralPosition.ToString());

        }
        public void GiveTheOrder()
        {
            if (DataLoader.SELECT_UNITS.Count > 0)
            {
                DataLoader.SELECT_UNITS.Sort((r1, r2) => r2.absolutePosition.X.CompareTo(r1.absolutePosition.X));
                DataLoader.RECTANGLE_FOR_SELECT_UNITS.Sort((r1, r2) => r2.centralPosition.X.CompareTo(r1.centralPosition.X));
                for (int i = 0; i < DataLoader.SELECT_UNITS.Count; i++)
                {
                    DataLoader.SELECT_UNITS[i].SetTargerToMove(DataLoader.RECTANGLE_FOR_SELECT_UNITS[i]);
                }
            }
        }
    }
}
