//Created by: Kamil Woloszyn
//Date: 27th Dec 2024
//Function Generating cave like stuructures directly into an array

using System.Security.Cryptography;
using System.Text;
using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Standalone Class For Use In generating custom structures outside of the main map, this class in particular dictates precedural generation of cave like structyures on a 2D plane
/// </summary>
public class CellularAutomataCaves : MonoBehaviour
{
    //Total width of the map
    [SerializeField]
    public int width;

    //Total height of the map
    [SerializeField]
    public int height;

    //Boolean value to check if a random seed should be used 
    [SerializeField]
    public bool useRandomSeed;

    //Percentage of the map to be randomly filled at the beggining
    [SerializeField]
    [Range(0, 100)]
    public int randomFillPercent;

    //The seed which will be used to 
    [SerializeField]
    public string seed;

    [SerializeField]
    public int smoothingIterations;

    int[,] map;

    /// <summary>
    /// Function to generate the Cellular Automata map in a 2d array which is returned at the end
    /// </summary>
    /// <returns>int[,]</returns>
    public int[,] GenerateMap_CA()
    {
        map = new int[width, height];
        RandomFill();

        for (int i = 0; i < smoothingIterations; i++)
        {
            SmoothMap();
        }

        return map;
    }

    /// <summary>
    /// Function to randomly fill inital map with random values which are either 1 or 0 representing wall, and floor
    /// </summary>
    private void RandomFill()
    {
        if (useRandomSeed)
        {
            //Getting voletile data that changes frequently & data which pertains to user to seed randomly
            seed = Time.time.ToString() + SystemInfo.graphicsDeviceID + DateTime.Now.ToString() + Environment.MachineName;
        }

        System.Random pseudoRandomNumberGenerator = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = 1;
                }
                else
                {
                    map[x, y] = (pseudoRandomNumberGenerator.Next(0, 100) < randomFillPercent) ? 1 : 0;
                }
            }
        }
    }

    /// <summary>
    /// Function to smooth out the map by checking each positions neighbours, if the position has more than 4 wall neighbours then then the position is a wall otherwise its a floor
    /// </summary>
    private void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallsCount(x, y);

                if (neighbourWallTiles > 4)
                    map[x, y] = 1;
                else if (neighbourWallTiles < 4)
                    map[x, y] = 0;
            }
        }
    }

    /// <summary>
    /// This function checks the amount of walls near a position on the map the amount is then returned
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private int GetSurroundingWallsCount(int x, int y)
    {
        int wallCount = 0;
        for (int neighbouringX = x - 1; neighbouringX <= x + 1; neighbouringX++)
        {
            for (int neighbouringY = y - 1; neighbouringY <= y + 1; neighbouringY++)
            {
                if (neighbouringX >= 0 && neighbouringX < width && neighbouringY >= 0 && neighbouringY < height)
                {
                    if (neighbouringX != x || neighbouringY != y)
                    {
                        wallCount += map[neighbouringX, neighbouringY];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }
        return wallCount;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CellularAutomataCaves))]
public class CellularAutomataCavesCustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        if (DrawDefaultInspector())
        {
            //Include Function call here to auto update values in this inspector
            
        }

        
    }
}
#endif
