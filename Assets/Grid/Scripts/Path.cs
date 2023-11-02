using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Path
{
    public List<Vector2> points;
    public float radius;

    public Path()
    {
        //radius of the path
        radius = 0.2f;
        points = new List<Vector2>();
    }

    // Add a point to the path
    public void addPoint(float x, float y)
    {
        Vector2 point = new Vector2(x, y);
        points.Add(point);
    }
}
