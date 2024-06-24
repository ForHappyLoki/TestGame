using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBuild.Code
{
    public static class HelpMethods
    {
        public static float AngleBetweenPoints(Vector2 point1, Vector2 point2)
        {
            // Вычисляем разницу между координатами точек
            Vector2 direction = point2 - point1;

            // Вычисляем угол в радианах
            float angleInRadians = (float)Math.Atan2(direction.Y, direction.X);

            return angleInRadians;
        }
        public static float AngleBetweenPointsDegree(Vector2 point1, Vector2 point2)
        {
            // Вычисляем разницу между координатами точек
            Vector2 direction = point2 - point1;

            // Вычисляем угол в радианах, используя Vector2.X и Vector2.Y
            float angleInRadians = (float)Math.Atan(direction.Y / direction.X);
            if (direction.X < 0)
            {
                if (direction.Y >= 0)
                    angleInRadians += (float)Math.PI; // 2nd quadrant
                else
                    angleInRadians -= (float)Math.PI; // 3rd quadrant
            }

            // Преобразуем угол в градусы
            float angleInDegrees = MathHelper.ToDegrees(angleInRadians);

            return angleInDegrees;
        }
    }
}
