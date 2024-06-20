using Microsoft.Xna.Framework;
using System.Collections.Generic;

public class QuadTreeNode
{
    private const int MAX_OBJECTS = 4;
    private const int MAX_LEVELS = 5;

    private int level;
    private List<Rectangle> objects;
    private Rectangle bounds;
    private QuadTreeNode[] nodes;

    public QuadTreeNode(int level, Rectangle bounds)
    {
        this.level = level;
        this.bounds = bounds;
        this.objects = new List<Rectangle>();
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

    private int GetIndex(Rectangle pRect)
    {
        int index = -1;
        double verticalMidpoint = bounds.X + (bounds.Width / 2);
        double horizontalMidpoint = bounds.Y + (bounds.Height / 2);

        bool topQuadrant = (pRect.Y < horizontalMidpoint && pRect.Y + pRect.Height < horizontalMidpoint);
        bool bottomQuadrant = (pRect.Y > horizontalMidpoint);

        if (pRect.X < verticalMidpoint && pRect.X + pRect.Width < verticalMidpoint)
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
        else if (pRect.X > verticalMidpoint)
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

    public void Insert(Rectangle pRect)
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

    public List<Rectangle> Retrieve(List<Rectangle> returnObjects, Rectangle pRect)
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
    public void Insert(Rectangle pRect)
    {
        root.Insert(pRect);
    }
    public List<Rectangle> Retrieve(Rectangle pRect)
    {
        List<Rectangle> returnObjects = new List<Rectangle>();
        return root.Retrieve(returnObjects, pRect);
    }
}