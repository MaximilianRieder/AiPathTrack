using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public GameObject whiteBlock;
    public GameObject blackBlock;
    public GameObject player;
    public GameObject target;
    public GameObject targetNotFound;
    public List<Vector2> pathSteering;
    public AStar aStar;
    public int[,] gridArray;

    void Start()
    {
        //height
        int counterLines = 0;
        //width
        int maxColumnLength = 0;
        string line;

        //get size of map
        StreamReader file = new StreamReader(Application.dataPath + "/map.txt");
        while ((line = file.ReadLine()) != null)
        {
            char[] charArr = line.ToCharArray();
            for (int k = 0; k < charArr.Length; k++)
            {
                //right most x is width
                if ((charArr[k] == 'X') && (k + 1 > maxColumnLength))
                    maxColumnLength = k + 1;
            }
            counterLines++;
        }
        file.Close();

        //make temporary array from the map file
        int[,] tempArray = new int[counterLines, maxColumnLength];
        StreamReader file2 = new StreamReader(Application.dataPath +  "/map.txt");
        int i = 0;
        while ((line = file2.ReadLine()) != null)
        {
            char[] charArr = line.ToCharArray();
            int j = 0;
            Debug.Log(line);
            foreach (char c in charArr)
            {
                tempArray[i, j] = MapToInt(c);
                j++;
            }
            i++;
        }
        file2.Close();

        //transpose the matrix and switch columns to match the x y coordinates of the map the positions in the array considering that the left under corner is x = 0 y = 0
        gridArray = new int[maxColumnLength, counterLines];
        for (int r = 0; r < counterLines; r++)
        {

            for (int j = 0; j < maxColumnLength; j++)
                gridArray[j, r] = tempArray[r, j];
        }
        ReverseRowsInPlace(gridArray);

        //instantiate the pathfinding class
        aStar = new AStar(maxColumnLength, counterLines, 1f, gridArray);

        //instantiate white blocks(obstacles)
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                //obstacles
                if (gridArray[x, y] == 1)
                    Instantiate(whiteBlock, aStar.GetGrid().getPositionInGameWithOffset(x, y), Quaternion.identity);
                //set boundarys around the map
                if (x == 0)
                    Instantiate(whiteBlock, aStar.GetGrid().getPositionInGameWithOffset(x - 1, y), Quaternion.identity);
                if (y == 0)
                    Instantiate(whiteBlock, aStar.GetGrid().getPositionInGameWithOffset(x, y - 1), Quaternion.identity);
                if (x == gridArray.GetLength(0) - 1)
                    Instantiate(whiteBlock, aStar.GetGrid().getPositionInGameWithOffset(x + 1, y), Quaternion.identity);
                if (y == gridArray.GetLength(1) - 1)
                    Instantiate(whiteBlock, aStar.GetGrid().getPositionInGameWithOffset(x, y + 1), Quaternion.identity);
            }
        }

        //get start and end positions (palyer and target)
        int startX = 0;
        int startY = 0;
        int targetX = 0;
        int targetY = 0;
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                if (gridArray[x, y] == 2)
                {
                    startX = x;
                    startY = y;
                }
                if (gridArray[x, y] == 3)
                {
                    targetX = x;
                    targetY = y;
                }
            }
        }

        //calculate the path
        List<Node> path = aStar.FindPath(startX, startY, targetX, targetY);

        //no possible path found
        if (path == null)
        {
            GameObject tfb = Instantiate(targetNotFound, aStar.GetGrid().getPositionInGameWithOffset(gridArray.GetLength(0) / 2, gridArray.GetLength(1) / 2), Quaternion.identity);
            tfb.transform.position = new Vector3(tfb.transform.position.x, tfb.transform.position.y, -8);
            //0.128 is hacky solution same with 0.446 -> is good with initial setup
            tfb.transform.localScale = new Vector3(gridArray.GetLength(0) * aStar.GetGrid().GetCellSize() * 0.128f, gridArray.GetLength(1) * aStar.GetGrid().GetCellSize() * 0.446f, 1);
            return;
        }

        //add the path in the pathsteering variable used in the player script
        foreach (Node n in path)
        {
            this.pathSteering.Add(new Vector2(aStar.GetGrid().GetCellSize() * 0.5f + n.GetX(), aStar.GetGrid().GetCellSize() * 0.5f + n.GetY()));
        }

        //instantiate player after path was found + target
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                if (gridArray[x, y] == 2)
                {
                    Instantiate(player, aStar.GetGrid().getPositionInGameWithOffset(x, y), Quaternion.identity);
                }
                if (gridArray[x, y] == 3)
                {
                    Instantiate(target, aStar.GetGrid().getPositionInGameWithOffset(x, y), Quaternion.identity);
                }
            }
        }

        //draw path in black
        foreach (Node n in path)
        {
            if (n.GetNodePrev() != null)
                Instantiate(blackBlock, aStar.GetGrid().getPositionInGameWithOffset(n.GetX(), n.GetY()), Quaternion.identity);
        }
    }

    /*
    0 = player can go tresspass
    1 = player cant go / tresspass
    2 = start
    3 = end
    */
    private int MapToInt(char c)
    {
        switch (c)
        {
            case ' ':
                return 0;
            case 'X':
                return 1;
            case 'S':
                return 2;
            case 'G':
                return 3;
            default:
                return 0;
        }
    }

    //reverses the order of rows in an array
    public static void ReverseRowsInPlace(int[,] matrix)
    {

        for (int row = 0; row < matrix.GetLength(0); row++)
        {
            for (int col = 0; col < matrix.GetLength(1) / 2; col++)
            {
                int temp = matrix[row, col];
                matrix[row, col] = matrix[row, matrix.GetLength(1) - col - 1];
                matrix[row, matrix.GetLength(1) - col - 1] = temp;
            }
        }
    }
}
