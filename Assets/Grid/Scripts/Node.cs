using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Node
{
    private Grid<Node> grid;
    private int x;
    private int y;

    private int g;
    private int h;
    private int f;

    private bool walkable;

    public Node nodePrev;

    public Node(Grid<Node> grid, int x, int y, bool walkable)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        this.walkable = walkable;
    }

    public override string ToString()
    {
        return x + " " + y;
    }

    public void CalculateF()
    {
        f = g + h;
    }

    public int GetX()
    {
        return x;
    }

    public int GetY()
    {
        return y;
    }

    public int GetG()
    {
        return g;
    }
    public int GetH()
    {
        return h;
    }
    public int GetF()
    {
        return f;
    }

    public Grid<Node> GetGrid() {
        return grid;
    }

    public bool GetWalkable() {
        return walkable;
    }

    public Node GetNodePrev()
    {
        return nodePrev;
    }

    public void SetX(int x)
    {
        this.x = x;
    }

    public void SetY(int y)
    {
        this.y = y;
    }
    public void SetG(int g)
    {
        this.g = g;
    }
    public void SetH(int h)
    {
        this.h = h;
    }
    public void SetF(int f)
    {
        this.f = f;
    }

    public void SetNodePrev(Node n)
    {
        this.nodePrev = n;
    }
}
