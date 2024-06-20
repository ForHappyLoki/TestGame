using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework;

namespace TestBuild.Code
{
    internal static class ModelFactory
    {
    }
    internal static class GameObjectsCreator
    {
        //public GameObjects Create()
        //{
        //    return new GameObjects();
        //}
    }
    internal static class SandBGCreator
    {
        public static Texture2D sandBG;
        public static BGImage Create()
        {
            if (sandBG == null)
            {
                sandBG = DataLoader.TextureListFind("sandBG");
            }
            return new BGImage(new Vector2(0,0), new Vector2(128, 128), sandBG); 
        }
    }
    internal static class PeasantWithSpearCreator
    {
        public static Texture2D peasantWithSpear;
        public static CommonUnits Create(Vector2 location)
        {
            if (peasantWithSpear == null)
            {
                peasantWithSpear = DataLoader.TextureListFind("peasantWithSpear");
            }
            return new CommonUnits(location, new Vector2(peasantWithSpear.Width, peasantWithSpear.Height), peasantWithSpear, 1, 1, 1);
        }
    }
}
