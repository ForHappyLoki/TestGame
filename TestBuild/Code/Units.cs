﻿using Microsoft.Xna.Framework;
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
        public bool isSelect = false;
        public Units(Vector2 AbsolutePosition, Vector2 ImageSize, Texture2D Image, int speed, int damage, int hp)
            : base(AbsolutePosition, ImageSize, Image)
        {
            this.speed = speed;
            this.damage = damage;
            this.hp = hp;
            DataLoader.UNIT_OBJECTS.Add(this);
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
