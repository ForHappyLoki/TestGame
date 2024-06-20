using Microsoft.Xna.Framework;
using System.Collections.Generic;
using TestBuild.Code;

public class QuadTreeNode
{
    private const int MAX_OBJECTS = 4;
    private const int MAX_LEVELS = 6;

    private int level;
    private List<GameObjects> objects;
    private Rectangle bounds;
    private QuadTreeNode[] nodes;

    public QuadTreeNode(int level, Rectangle bounds)
    {
        this.level = level;
        this.bounds = bounds;
        this.objects = new List<GameObjects>();
        this.nodes = new QuadTreeNode[4];
    }

    public void Clear()
    {
        objects.Clear();

        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i] != null)
            {
                nodes[i].Clear();
                nodes[i] = null;
            }
        }
    }
    private void Split()
    {
        int subWidth = bounds.Width / 2;
        int subHeight = bounds.Height / 2;
        int x = bounds.X;
        int y = bounds.Y;

        nodes[0] = new QuadTreeNode(level + 1, new Rectangle(x + subWidth, y, subWidth, subHeight));
        nodes[1] = new QuadTreeNode(level + 1, new Rectangle(x, y, subWidth, subHeight));
        nodes[2] = new QuadTreeNode(level + 1, new Rectangle(x, y + subHeight, subWidth, subHeight));
        nodes[3] = new QuadTreeNode(level + 1, new Rectangle(x + subWidth, y + subHeight, subWidth, subHeight));
    }

    private int GetIndex(GameObjects pRect)
    {
        int index = -1;
        double verticalMidpoint = bounds.X + (bounds.Width / 2);
        double horizontalMidpoint = bounds.Y + (bounds.Height / 2);

        bool topQuadrant = (pRect.collisionRectangle.Y < horizontalMidpoint && pRect.collisionRectangle.Y + pRect.collisionRectangle.Height < horizontalMidpoint);
        bool bottomQuadrant = (pRect.collisionRectangle.Y > horizontalMidpoint);

        if (pRect.collisionRectangle.X < verticalMidpoint && pRect.collisionRectangle.X + pRect.collisionRectangle.Width < verticalMidpoint)
        {
            if (topQuadrant)
            {
                index = 1;
            }
            else if (bottomQuadrant)
            {
                index = 2;
            }
        }
        else if (pRect.collisionRectangle.X > verticalMidpoint)
        {
            if (topQuadrant)
            {
                index = 0;
            }
            else if (bottomQuadrant)
            {
                index = 3;
            }
        }

        return index;
    }

    public void Insert(GameObjects pRect)
    {
        if (nodes[0] != null)
        {
            int index = GetIndex(pRect);

            if (index != -1)
            {
                nodes[index].Insert(pRect);
                return;
            }
        }

        objects.Add(pRect);

        if (objects.Count > MAX_OBJECTS && level < MAX_LEVELS)
        {
            if (nodes[0] == null)
            {
                Split();
            }

            int i = 0;
            while (i < objects.Count)
            {
                int index = GetIndex(objects[i]);
                if (index != -1)
                {
                    nodes[index].Insert(objects[i]);
                    objects.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }
    }

    public List<GameObjects> Retrieve(List<GameObjects> returnObjects, GameObjects pRect)
    {
        int index = GetIndex(pRect);
        if (index != -1 && nodes[0] != null)
        {
            nodes[index].Retrieve(returnObjects, pRect);
        }

        returnObjects.AddRange(objects);

        return returnObjects;
    }
}

public class QuadTree
{
    private QuadTreeNode root;
    public QuadTree(Rectangle bounds)
    {
        root = new QuadTreeNode(0, bounds);
    }
    public void Clear()
    {
        root.Clear();
    }
    public void Insert(GameObjects pRect)
    {
        root.Insert(pRect);
    }
    public List<GameObjects> Retrieve(GameObjects pRect)
    {
        List<GameObjects> returnObjects = new List<GameObjects>();
        return root.Retrieve(returnObjects, pRect);
    }
}