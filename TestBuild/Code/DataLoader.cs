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
        public static List<Units> SELECT_UNITS = new List<Units>();
        public static List<RectangleForSelectUnits> RECTANGLE_FOR_SELECT_UNITS = new List<RectangleForSelectUnits>();
        public static void RectangleForSelectUnitsCreator()
        {
            RECTANGLE_FOR_SELECT_UNITS.Clear();
            for (int i = 0; i < SELECT_UNITS.Count; i++)
            {
                RECTANGLE_FOR_SELECT_UNITS.Add(new RectangleForSelectUnits(SELECT_UNITS[i].collisionRectangle));
            }
        }
        public static void CreatePeasantWithSpear(Vector2 position)
        {
            var peasantWithSpear = PeasantWithSpearCreator.Create(position);
            GAME_OBJECTS.Add(peasantWithSpear);
        }
    }
    public class RectangleForSelectUnits
    {
        public Vector2 position;
        public Vector2 centralPosition;
        public Rectangle rectangle;
        public RectangleForSelectUnits(Rectangle rectangle)
        {
            this.rectangle = new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }
        public RectangleForSelectUnits(RectangleForSelectUnits rectangleForSelectUnits)
        {
            position = rectangleForSelectUnits.position;
            centralPosition = rectangleForSelectUnits.centralPosition;
            rectangle = rectangleForSelectUnits.rectangle;
        }
        public void SetPosition(Vector2 position)
        {
            centralPosition = position;
            this.position = position - new Vector2(rectangle.Width/2, rectangle.Height/2);
            rectangle = new Rectangle((int)this.position.X, (int)this.position.Y, rectangle.Width, rectangle.Height);
        }
    }
}
