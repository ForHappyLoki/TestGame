using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TestBuild.Code
{
    internal static class CollisionHandler
    {
        private static QuadTree _quadTree;
        public static void CollisionLoader(int width, int height)
        {
            // Инициализация QuadTree
            _quadTree = new QuadTree(new Rectangle(0, 0, width, height));
        }
        public static List<GameObjects[]> CollisionUpdater()
        {
            List<GameObjects[]> _potentialCollisions = new List<GameObjects[]>();
            // Очистка QuadTree и вставка объектов заново
            _quadTree.Clear();
            foreach (var obj in DataLoader.GAME_OBJECTS)
            {
                _quadTree.Insert(obj);
            }
            // Проверка потенциальных столкновений
            _potentialCollisions.Clear();
            foreach (var obj in DataLoader.GAME_OBJECTS)
            {
                if (obj is Units)
                {
                    Units _obj = (Units)obj;
                    _obj.isSelect = false;
                }
                var collisions = _quadTree.Retrieve(obj);
                foreach (var collision in collisions)
                {
                    if (obj != collision && obj.collisionRectangle.Intersects(collision.collisionRectangle))
                    {
                        _potentialCollisions.Add(new GameObjects[] { collision, obj });
                    }
                }
            }
            return _potentialCollisions;
        }
    }
}
