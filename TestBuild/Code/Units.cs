using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBuild.Code
{
    public abstract class ModelOnMap
    {
        public bool collision;
        public Vector2 absolutePosition { get; set; }
        public Vector2 imageSize { get; set; }
        public Texture2D image { get; set; }
        public Texture2D mirriredImage;
        public Texture2D normalImage;
        public ModelOnMap(Vector2 AbsolutePosition, Vector2 ImageSize, Texture2D Image)
        {
            absolutePosition = AbsolutePosition;
            imageSize = ImageSize;
            image = Image;
        }
    }
    internal class BGImage : ModelOnMap
    {
        public BGImage(Vector2 AbsolutePosition, Vector2 ImageSize, Texture2D Image)
            : base(AbsolutePosition, ImageSize, Image)
        {
            base.collision = false;
        }
    }
    public class GameObjects : ModelOnMap
    {
        private static int _curentlyID = 0;
        public int _ID;
        public Rectangle collisionRectangle { get; set; }
        public GameObjects(Vector2 AbsolutePosition, Vector2 ImageSize, Texture2D Image) 
            : base(AbsolutePosition, ImageSize, Image)
        {
            collisionRectangle = new Rectangle((int)AbsolutePosition.X, (int)AbsolutePosition.Y + (int)ImageSize.Y / 2, (int)ImageSize.X, (int)ImageSize.Y/2);
            _ID = _curentlyID++;
            DataLoader.GAME_OBJECTS.Add(this);
            base.collision = false;
        }
    }
    public class StrokePanel : GameObjects
    {
        public StrokePanel(Rectangle rectangleCollision)
            : base(new Vector2(), new Vector2(), null)
        {
            collisionRectangle = rectangleCollision;
        }
        public void CollisionRectangleUpdate(Rectangle rectangle)
        {
            collisionRectangle = rectangle;
        }
    }
    public class Units : GameObjects
    {
        public int speed { get; set; }
        public int damage { get; set; }
        public int hp { get; set; }
        public int moral { get; set; }
        private bool isSelect = false;
        private float _angle;
        private bool _onMove = false;
        public Units(Vector2 AbsolutePosition, Vector2 ImageSize, Texture2D Image, int speed, int damage, int hp)
            : base(AbsolutePosition, ImageSize, Image)
        {
            this.speed = speed;
            this.damage = damage;
            this.hp = hp;
            DataLoader.UNIT_OBJECTS.Add(this);
            centerOfModel = AbsolutePosition + new Vector2(0, ImageSize.Y / 2) + (ImageSize - new Vector2(0, ImageSize.Y / 2)) / 2;
        }
        public void UnitUpdate()
        {
            if (_onMove)
            {
                Move();
            }
        }
        public void SelectingUnit()
        {
            isSelect = true;
            DataLoader.SELECT_UNITS.Add(this);
        }
        public void UnselectingUnit()
        {
            isSelect = false;
            DataLoader.SELECT_UNITS.Remove(this);
        }
        public bool SelectingReturn()
        {
            return isSelect;
        }
        public void SetTargerToMove(Vector2 target)
        {
            targetToMove = target;
            _onMove = true;
            CalculatingTheAngle();
        }
        public float CalculatingTheAngle()
        {
            _angle = HelpMethods.AngleBetweenPoints(centerOfModel, targetToMove);
            _xOffset = speed * (float)Math.Cos(_angle);
            _yOffset = speed * (float)Math.Sin(_angle);
            _offset = new Vector2(_xOffset, _yOffset);
            return _angle;
        }
        private float _xOffset;
        private float _yOffset;
        private Vector2 _offset;
        public Vector2 targetToMove { get; set; }
        public Vector2 centerOfModel;
        private void Move()
        {
            if (Vector2.Distance(centerOfModel, targetToMove) <= (speed + 1)/2.0)
            {
                _onMove = false;
            }
            else
            {
                absolutePosition = absolutePosition + _offset;
                centerOfModel = centerOfModel + _offset;
                collisionRectangle = new Rectangle((int) absolutePosition.X + (int)_xOffset, (int) absolutePosition.Y + (int) imageSize.Y / 2 + (int)_yOffset, (int)imageSize.X, (int)imageSize.Y / 2);
            }
        }
    }
    public class CommonUnits : Units
    {
        public CommonUnits(Vector2 AbsolutePosition, Vector2 ImageSize, Texture2D Image, int speed, int damage, int hp)
            : base(AbsolutePosition, ImageSize, Image, speed, damage, hp)
        {

        }
    }
    //public class HeroUnits : Units
    //{
    //    public HeroUnits(Vector2 AbsolutePosition, Vector2 ImageSize, Texture2D Image, int speed, int damage, int hp)
    //        : base(AbsolutePosition, ImageSize, Image, speed, damage, hp)
    //    {
    //    }
    //}
    //public class BuildUnits : Units
    //{
    //    public BuildUnits(Vector2 AbsolutePosition, Vector2 ImageSize, Texture2D Image, int speed, int damage, int hp)
    //        : base(AbsolutePosition, ImageSize, Image, speed, damage, hp)
    //    {
    //    }
    //}
}
