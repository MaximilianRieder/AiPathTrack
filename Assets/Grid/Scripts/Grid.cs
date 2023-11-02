using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class Grid<TGridObject>
{
    private int width;
    private int height;
    private int[,] gridArray;
    private float cellSize;
    private TGridObject[,] algArray;

    public Grid(int width, int height, float cellSize, int[,] kridArray, Func<Grid<TGridObject>, int, int, bool, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        /*
        0 = player can go tresspass
        1 = player cant go / tresspass
        2 = start
        3 = end
        */
        //array for the values in the grid
        this.gridArray = kridArray;

        //array with node objects for the algorithm
        this.algArray = new TGridObject[width, height];
        for (int x = 0; x < algArray.GetLength(0); x++)
        {
            for (int y = 0; y < algArray.GetLength(1); y++)
            {
                if (gridArray[x, y] == 1)
                {
                    algArray[x, y] = createGridObject(this, x, y, false);
                }
                else
                {
                    algArray[x, y] = createGridObject(this, x, y, true);
                }
            }
        }

        //draw the Grid and print some values
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {   
                //for debug
                //printText(algArray[x, y].ToString() + " " + gridArray[x, y].ToString(), getPositionInGame(x, y), 20);
                DrawLine(getPositionInGame(x, y), getPositionInGame(x, y + 1), Color.white);
                DrawLine(getPositionInGame(x, y), getPositionInGame(x + 1, y), Color.white);
            }
        }
        DrawLine(getPositionInGame(0, height), getPositionInGame(width, height), Color.white);
        DrawLine(getPositionInGame(width, 0), getPositionInGame(width, height), Color.white);

        //adjust the camera to the grid
        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = (float)(gridArray.GetLength(0) * cellSize) / (float)(gridArray.GetLength(1) * cellSize);
        if (screenRatio >= targetRatio)
        {
            Camera.main.orthographicSize = (gridArray.GetLength(1) * cellSize) / 2;
        }
        else
        {
            float differenceInSize = targetRatio / screenRatio;
            Camera.main.orthographicSize = (gridArray.GetLength(1) * cellSize) / 2 * differenceInSize;
        }
        GameObject.Find("Main Camera").transform.position = new Vector3((width * cellSize) / 2, (height * cellSize) / 2, -10);
    }

    //get cell position in game  
    //array first position indicates the height but the x (first) indicates the width in a coordinate system
    public Vector3 getPositionInGame(int x, int y)
    {
        return new Vector3(x, y) * cellSize;
    }

    //get the position of the center of a gridcell
    public Vector3 getPositionInGameWithOffset(int x, int y)
    {
        return (new Vector3(x, y) * cellSize) + (Vector3.one * cellSize * 0.5f);
    }

    public void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }

    public TGridObject GetGridObject(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return algArray[x, y];
        }
        else
        {
            return default(TGridObject);
        }
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public float GetCellSize()
    {
        return cellSize;
    }

    public int[,] GetGameArray()
    {
        return this.gridArray;
    }

    public void SetValue(int x, int y, int value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
        }
    }

    //print text of cell (for debug)
    /*
    public TextMesh printText(string text, Vector3 localPosition, int fontSize)
    {
        GameObject ngo = new GameObject("Text", typeof(TextMesh));
        Transform transform = ngo.transform;
        //evt. set parent here
        transform.SetParent(null, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = ngo.GetComponent<TextMesh>();
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = Color.white;
        return textMesh;
    }

    public void DrawLineMiddle(Vector3 start, Vector3 end, Color color)
    {
        start = start + new Vector3(cellSize / 2, cellSize / 2, 0);
        end = end + new Vector3(cellSize / 2, cellSize / 2, 0);
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        //lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(color, color);
        lr.SetWidth(0.05f, 0.05f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }*/
}
