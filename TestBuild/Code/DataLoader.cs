using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace TestBuild.Code
{
    internal static class DataLoader
    {
        private static List<Texture2D> textureList = new List<Texture2D>();
        private static List<string> textureListName = new List<string>();

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
    }
}
