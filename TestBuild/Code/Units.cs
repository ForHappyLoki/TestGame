using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TestBuild.Code
{
    internal abstract class ModelOnMap
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
    internal class GameObjects : ModelOnMap
    {
        public GameObjects(Vector2 AbsolutePosition, Vector2 ImageSize, Texture2D Image) 
            : base(AbsolutePosition, ImageSize, Image)
        {

        }
    }
    internal class Units : GameObjects
    {
        public int speed { get; set; }
        public int damage { get; set; }
        public int hp { get; set; }
        public Units(Vector2 AbsolutePosition, Vector2 ImageSize, Texture2D Image, int speed, int damage, int hp)
            : base(AbsolutePosition, ImageSize, Image)
        {
            this.speed = speed;
            this.damage = damage;
            this.hp = hp;
        }
    }
    internal class CommonUnits : Units
    {
        public CommonUnits(Vector2 AbsolutePosition, Vector2 ImageSize, Texture2D Image, int speed, int damage, int hp)
            : base(AbsolutePosition, ImageSize, Image, speed, damage, hp)
        {

        }
    }
    internal class HeroUnits : Units
    {
        public HeroUnits(Vector2 AbsolutePosition, Vector2 ImageSize, Texture2D Image, int speed, int damage, int hp)
            : base(AbsolutePosition, ImageSize, Image, speed, damage, hp)
        {
        }
    }
    internal class BuildUnits : Units
    {
        public BuildUnits(Vector2 AbsolutePosition, Vector2 ImageSize, Texture2D Image, int speed, int damage, int hp)
            : base(AbsolutePosition, ImageSize, Image, speed, damage, hp)
        {
        }
    }
}
