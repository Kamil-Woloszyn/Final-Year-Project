//Created by: Kamil Woloszyn
//In the Years 2024-2025
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Pathfind_Astar : MonoBehaviour
{
    private class Cell
    {
        //The position of the cell
        public Vector2 position;

        //The total estimated cost of the path through node (n), calculated as f(n) = g(n) + h(n). 
        public int fCost = int.MaxValue;

        //the total path cost, calculated by adding the cost to reach the current node
        public int gCost = int.MaxValue;

        //the estimated cost to reach the goal from the current node
        public int hCost = int.MaxValue;

        //link between nodes
        public Vector2 connection;

        //Boolean to check if the cell is a wall
        public bool isWall;

        public Cell(Vector2 pos)
        {
            position = pos;
        }
    }

    /// <summary>
    /// Width of a cell
    /// </summary>
    private int cellWidth = 1;

    /// <summary>
    /// Height of a cell
    /// </summary>
    private int cellHeight = 1;

    /// <summary>
    /// Dictionary object which stores all of the cells by indexing their position
    /// </summary>
    private Dictionary<Vector2, Cell> cells;

    /// <summary>
    /// List of cells to search
    /// </summary>
    public List<Vector2> cellsToSearch;

    /// <summary>
    /// List of the cells searched
    /// </summary>
    public List<Vector2> searchedCells;

    /// <summary>
    /// List of the final lowest cost path found
    /// </summary>
    public List<Vector2> finalPath;

    /// <summary>
    /// Modified A* Algorithm which about the best path by generating random issues based on a rng
    /// </summary>
    /// <param name="startingPoint"></param>
    /// <param name="destination"></param>
    public List<Vector2> FindPathBetweenStructures(Vector2 startingPoint, Vector2 destination)
    {
        cellsToSearch = new List<Vector2> { startingPoint };
        searchedCells = new List<Vector2>();
        finalPath = new List<Vector2>();

        cells[startingPoint].gCost = 0; 
        cells[startingPoint].hCost = GetDistance(startingPoint, destination);
        cells[startingPoint].fCost = GetDistance(startingPoint, destination);

        while (cellsToSearch.Count > 0)
        {
            Vector2 cellToSearch = cellsToSearch[0];

            foreach (Vector2 pos in cellsToSearch)
            {
                Cell c = cells[pos];
                if (c.fCost < cells[cellToSearch].fCost ||
                    c.fCost == cells[cellToSearch].fCost && c.hCost == cells[cellToSearch].hCost)
                {
                    cellToSearch = pos;
                }
            }


            cellsToSearch.Remove(cellToSearch);
            searchedCells.Add(cellToSearch);

            if (cellToSearch == destination)
            {
                Cell pathCell = cells[destination];

                while (pathCell.position != startingPoint)
                {
                    finalPath.Add(pathCell.position);
                    pathCell = cells[pathCell.connection];
                }

                finalPath.Add(startingPoint);
            }

            SearchCellNeighbors(cellToSearch, destination);
        }

        if (finalPath.Count == 0)
        {
            Debug.Log("Path not found");
        }

        return finalPath;
    }

    /// <summary>
    /// Function of the Astar top find a path between two different points and return the coordinates of the path as a list of vector2
    /// </summary>
    /// <param name="startingPoint"></param>
    /// <param name="destination"></param>
    private List<Vector2> FindPath(Vector2 startingPoint, Vector2 destination)
    {
        cellsToSearch = new List<Vector2> { startingPoint };
        searchedCells = new List<Vector2>();
        finalPath = new List<Vector2>();

        cells[startingPoint].gCost = 0;
        cells[startingPoint].hCost = GetDistance(startingPoint, destination);
        cells[startingPoint].fCost = GetDistance(startingPoint, destination);

        while (cellsToSearch.Count > 0)
        {
            Vector2 cellToSearch = cellsToSearch[0];

            foreach (Vector2 pos in cellsToSearch)
            {
                Cell c = cells[pos];
                if (c.fCost < cells[cellToSearch].fCost ||
                    c.fCost == cells[cellToSearch].fCost && c.hCost == cells[cellToSearch].hCost)
                {
                    cellToSearch = pos;
                }
            }


            cellsToSearch.Remove(cellToSearch);
            searchedCells.Add(cellToSearch);

            if (cellToSearch == destination)
            {
                Cell pathCell = cells[destination];

                while (pathCell.position != startingPoint)
                {
                    finalPath.Add(pathCell.position);
                    pathCell = cells[pathCell.connection];
                }

                finalPath.Add(startingPoint);
            }

            SearchCellNeighbors(cellToSearch, destination);
        }

        if (finalPath.Count == 0)
        {
            Debug.Log("Path not found");
        }

        return finalPath;
    }
  
    //Function to search all the neighbouring cells using cell position and end position
    private void SearchCellNeighbors(Vector2 cellPos, Vector2 endPos)
    {
        //For loops which go through all of the neighbour positions of the cell position
        for (float x = cellPos.x - cellWidth; x <= cellWidth + cellPos.x; x += cellWidth)
        {
            for (float y = cellPos.y - cellHeight; y <= cellHeight + cellPos.y; y += cellHeight)
            {
                Vector2 neighborPos = new Vector2(x, y);

                if (cells.TryGetValue(neighborPos, out Cell c) && !searchedCells.Contains(neighborPos) && !cells[neighborPos].isWall)
                {
                    int GcostToNeighbour = cells[cellPos].gCost + GetDistance(cellPos, neighborPos);
                    //Checking if the neighbour cell is inside of the grid
                    if (GcostToNeighbour < cells[neighborPos].gCost)
                    {
                        Cell neighbourNode = cells[neighborPos];

                        neighbourNode.connection = cellPos;
                        neighbourNode.gCost = GcostToNeighbour;
                        neighbourNode.hCost = GetDistance(neighborPos, endPos);
                        neighbourNode.fCost = neighbourNode.gCost + neighbourNode.hCost;

                        //Checking if neighbour position is on the cellstoSearch list
                        if (!cellsToSearch.Contains(neighborPos))
                        {
                            cellsToSearch.Add(neighborPos);
                        }
                    }
                }
            }
        }
    }
 

    /// <summary>
    /// Function which returns the distance between the two positions;
    /// </summary>
    /// <param name="position1"></param>
    /// <param name="position2"></param>
    /// <returns></returns>
    private int GetDistance(Vector2 position1, Vector2 position2)
    {
        Vector2Int dist = new Vector2Int(Mathf.Abs((int)position1.x), Mathf.Abs((int)position1.y - (int)position2.y));

        int lowestDistance = Mathf.Min(dist.x, dist.y);
        int highestDistance = Mathf.Max(dist.x, dist.y);

        int horizontalMovesRequired = highestDistance - lowestDistance;

        return lowestDistance * 14 + horizontalMovesRequired * 10;
    }

    /// <summary>
    /// Function which returns the distance between the two positions but for this one we decide to lie about the distance which will allow unique path generation
    /// </summary>
    /// <param name="position1"></param>
    /// <param name="position2"></param>
    /// <returns></returns>
    private int GetDistanceSpecial(Vector2 position1, Vector2 position2)
    {
        Vector2Int dist = new Vector2Int(Mathf.Abs((int)position1.x), Mathf.Abs((int)position1.y - (int)position2.y));

        int lowestDistance = Mathf.Min(dist.x, dist.y);
        int highestDistance = Mathf.Max(dist.x, dist.y);

        int horizontalMovesRequired = highestDistance - lowestDistance;

        return lowestDistance * 14 + horizontalMovesRequired;
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(Pathfind_Astar))]
public class Pathfind_AstarCustomInspector : Editor
{
    private void OnEnable()
    {
    }

    public override void OnInspectorGUI()
    {
        Pathfind_Astar pathfind = (Pathfind_Astar)target;
        if (DrawDefaultInspector())
        {
            //Include Function call here to auto update values in this inspector

        }

    }
}
#endif
