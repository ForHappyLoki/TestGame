using Microsoft.Xna.Framework;
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

            // Преобразуем угол в градусы
            //float angleInDegrees = MathHelper.ToDegrees(angleInRadians);

            return angleInRadians;
        }
    }
}
