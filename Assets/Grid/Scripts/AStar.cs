using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
 * 
 * 
 * Astar algorithm without diagonal movement
 * 
 * 
 * 
 */
public class AStar
{
    //Costs for Movement
    private const int MOVEMENT_COST = 1;

    private Grid<Node> grid;
    private List<Node> remainingList;
    private List<Node> completedList;
    public AStar(int width, int height, float cellSize, int[,] gridArray)
    {
        grid = new Grid<Node>(width, height, cellSize, gridArray, (Grid<Node> g, int x, int y, bool walkable) => new Node(g, x, y, walkable));
    }

    public List<Node> FindPath(int fromX, int fromY, int toX, int toY)
    {
        Node startNode = grid.GetGridObject(fromX, fromY);
        Node targetNode = grid.GetGridObject(toX, toY);

        remainingList = new List<Node> { startNode };
        completedList = new List<Node> { };

        //initialize the nodes in the Grid
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                Node node = grid.GetGridObject(x, y);
                node.SetG(int.MaxValue);
                node.SetH(int.MaxValue);
                node.CalculateF();
                node.SetNodePrev(null);
            }
        }

        //initialize starting node
        startNode.SetG(0);
        startNode.SetH(ShortestDistanceWithoutObstacles(startNode, targetNode));
        startNode.CalculateF();

        //cycle throu remaining list
        while (remainingList.Count > 0)
        {
            //priority queue
            Node currentNode = GetLowestFNode(remainingList);
            
            //if finished
            if (currentNode == targetNode)
                return makePath(currentNode);

            //searched -> mark as completed
            remainingList.Remove(currentNode);
            completedList.Add(currentNode);

            //look at neighbour cells
            foreach (Node neighbour in GetNeighbours(currentNode))
            {
                //already searched
                if (completedList.Contains(neighbour))
                    continue;
                //is obstacle
                if (!neighbour.GetWalkable()) {
                    completedList.Add(neighbour);
                    continue;
                }

                //calculate new G cost with heuristic
                int newG = currentNode.GetG() + ShortestDistanceWithoutObstacles(currentNode, neighbour);
                //update neighbour if new shortest path is found
                if (newG < neighbour.GetG())
                {
                    neighbour.SetNodePrev(currentNode);
                    neighbour.SetG(newG);
                    neighbour.SetH(ShortestDistanceWithoutObstacles(neighbour, targetNode));
                    neighbour.CalculateF();
                    if (!remainingList.Contains(neighbour))
                        remainingList.Add(neighbour);
                }
            }
        }
        //went through whole map and didnt find target -> cant reach
        return null;
    }

    //heuristic (manhatten)
    private int ShortestDistanceWithoutObstacles(Node from, Node to)
    {
        int xDistance = Mathf.Abs(from.GetX() - to.GetX());
        int yDistance = Mathf.Abs(from.GetY() - to.GetY());
        return (yDistance + xDistance) * MOVEMENT_COST;
    }

    //like a priority queue
    private Node GetLowestFNode(List<Node> nodeList)
    {
        Node lowestFNode = nodeList[0];
        for (int i = 1; i < nodeList.Count; i++)
        {
            if (nodeList[i].GetF() < lowestFNode.GetF())
            {
                lowestFNode = nodeList[i];
            }
        }
        return lowestFNode;
    }

    //make a list of nodes that are in a path
    private List<Node> makePath(Node endNode)
    {
        List<Node> path = new List<Node>();
        path.Add(endNode);
        Node currentNode = endNode;
        while (currentNode.GetNodePrev() != null)
        {
            path.Add(currentNode.GetNodePrev());
            currentNode = currentNode.GetNodePrev();
        }
        path.Reverse();
        return path;
    }

    //get the left up right and down neighbours
    private List<Node> GetNeighbours(Node currentNode)
    {
        List<Node> neighbours = new List<Node>();
        //left
        if (currentNode.GetX() - 1 >= 0)
        {
            neighbours.Add(GetNode(currentNode.GetX() - 1, currentNode.GetY()));
        }
        //right
        if (currentNode.GetX() + 1 < grid.GetWidth())
        {
            neighbours.Add(GetNode(currentNode.GetX() + 1, currentNode.GetY()));
        }
        //down
        if (currentNode.GetY() - 1 >= 0)
        {
            neighbours.Add(GetNode(currentNode.GetX(), currentNode.GetY() - 1));
        }
        //up
        if (currentNode.GetY() + 1 < grid.GetHeight())
        {
            neighbours.Add(GetNode(currentNode.GetX(), currentNode.GetY() + 1));
        }

        return neighbours;
    }

    public Node GetNode(int x, int y)
    {
        return grid.GetGridObject(x, y);
    }

    public Grid<Node> GetGrid() {
        return grid;
    }
}
