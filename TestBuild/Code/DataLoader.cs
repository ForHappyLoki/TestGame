using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TestBuild.Code
{
    internal static class DataLoader
    {
        private static List<Texture2D> textureList = new List<Texture2D>();
        private static List<string> textureListName = new List<string>();
        private static List<GameObjects> gameObjectsList = new List<GameObjects>();

        private static BGImage bGImage;

        public static void TextureListAdd(Texture2D texture, string name)
        {
            textureList.Add(texture);
            textureListName.Add(name);
        }
        public static Texture2D TextureListFind(string name)
        {
            int i = 0;
            foreach(string textureName in textureListName)
            {
                if(textureName == name)
                {
                    return textureList[i]; 
                }
                i++;
            }
            return null;
        }
        public static BGImage ReturnFragmentBGSand()
        {
            if (bGImage == null)
            {
                bGImage = SandBGCreator.Create();
            }
            return bGImage;
        }
        public static List<GameObjects> GAME_OBJECTS = new List<GameObjects>();
        public static List<Units> UNIT_OBJECTS = new List<Units>();
        public static void CreatePeasantWithSpear(Vector2 position)
        {
            var peasantWithSpear = PeasantWithSpearCreator.Create(position);
            GAME_OBJECTS.Add(peasantWithSpear);
        }
    }
}
