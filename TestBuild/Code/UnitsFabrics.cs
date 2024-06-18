using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Reflection.Metadata;

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
}
