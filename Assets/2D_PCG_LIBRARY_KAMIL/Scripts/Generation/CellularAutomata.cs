//Created by: Kamil Woloszyn
//In the Years 2024-2025
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UnityEngine.UIElements.Experimental;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEditor.Experimental.GraphView;

/// <summary>
/// Script that generates biomes using a custom cellular automata algorithm
/// </summary>
public class CellularAutomata : MonoBehaviour
{
    //Private variables
    private int[,] map;

    private int randomFillPercent;

    private int biomeCount;

    private int smoothingIterations;

    private int maxPlotCountPerBiome;

    private bool createBordersBetweenBiomes;

    private int similarityCutOff;

    private int width;
    private int height;

    private string seed;
    private bool useRandomSeed;

    private int sizeToExpandByEachSide;

    /// <summary>
    /// Singleton Instance
    /// </summary>
    public static CellularAutomata Singleton { get; private set; }

    private void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(this);
        }
        else
        {
            Singleton = this;
        }
    }

    private void Start()
    {
        //Assigning Variables
        InitializeVariables();

    }

    /// <summary>
    /// Function for initializing different global variables used inside of the script,
    /// the variables are assigned in the GenerationalValues which should be attached to an empty object in the editor
    /// </summary>
    private void InitializeVariables()
    {
        width = GenerationalValues.Singleton.GetWidth();
        height = GenerationalValues.Singleton.GetHeight();
        map = new int[width,height];
        //Initalizing Values
        map = GenerationalValues.Singleton.GetBiomeMap();
        randomFillPercent = GenerationalValues.Singleton.GetRandomFillPercent();
        biomeCount = GenerationalValues.Singleton.GetBiomeCount();
        smoothingIterations = GenerationalValues.Singleton.GetSmoothingIterations();
        maxPlotCountPerBiome = GenerationalValues.Singleton.GetMaxPlotCountPerBiomes();
        createBordersBetweenBiomes = GenerationalValues.Singleton.GetCreateBordersBetweenBiomes();
        similarityCutOff = GenerationalValues.Singleton.GetSimilarityCutOff();
        seed = GenerationalValues.Singleton.GetSeed();
        useRandomSeed = GenerationalValues.Singleton.GetUseRandomSeed();
        sizeToExpandByEachSide = GenerationalValues.Singleton.GetExpandMapBySize();
        if (useRandomSeed)
        {
            GenerationalValues.Singleton.SetSeed(DateTime.Now.ToString());
            GenerationalValues.Singleton.ChangeRandomizerSeed(seed);
            seed = DateTime.Now.ToString();
        }

    }

    /// <summary>
    /// Function that will update original values inside of the stored object variables (for use with other scripts)
    /// </summary>
    private void UpdateStoredVariables()
    {
        PCG_Manager.Singleton.SetPCGValuesForCellularAutomata(map, randomFillPercent, biomeCount, smoothingIterations, maxPlotCountPerBiome, createBordersBetweenBiomes, similarityCutOff, height, width, seed, useRandomSeed, sizeToExpandByEachSide);

    }

    /// <summary>
    /// Function to generate initial randomly filled map that will be then smoothed out
    /// </summary>
    public void GenerateMap_CA()
    {
        InitializeVariables();
        map = new int[width, height];
        RandomFill();
        SmoothBiomes();
        if (createBordersBetweenBiomes)
        {
            CreateBordersBetweenBiomes();
        }
        UpdateStoredVariables();
        
    }

    public void ExpandMap()
    {
        InitializeVariables();
        ExpandMapOut();
        if (createBordersBetweenBiomes)
        {
            CreateBordersBetweenBiomes();
        }
        UpdateStoredVariables();

    }

    /// <summary>
    /// Function that will randomly fill map with biome plots using PCG settings
    /// </summary>
    private void RandomFill()
    {
        List<int> remainingPlotsPerBiome = new List<int>();
        for(int i = 0; i < biomeCount; i++)
        {
            remainingPlotsPerBiome.Add(maxPlotCountPerBiome);
        }

        //Populate map with randomly placed biome plots
        for(int i = 1;i < biomeCount; i++)
        {
            foreach(var remainingPlots in remainingPlotsPerBiome)
            {
                for(int j = remainingPlots; j != 0;  j--)
                {
                    int x = GenerationalValues.Singleton.RandomValueBetween(0, width);
                    int y = GenerationalValues.Singleton.RandomValueBetween(0, height);
                    map[x, y] = i;
                }
                
            }
        }
    }

    /// <summary>
    /// Function that expands biome map out on all sides
    /// </summary>
    private void ExpandMapOut()
    {
        //TO-DO CHANGE sizeToExpandByEachSide to be included inside of GenerationalValues object
        int newWidth = width + (sizeToExpandByEachSide * 2);
        int newHeight = height + (sizeToExpandByEachSide * 2);
        int[,] newMap = new int[newWidth, newHeight];
        
        for (int x = 0; x < newWidth; x++)
        {
            for(int y = 0; y < newHeight; y++)
            {
                newMap[x, y] = 0;
                if(y <= sizeToExpandByEachSide || x <= sizeToExpandByEachSide) newMap[x, y] = GenerationalValues.Singleton.RandomValueBetween(0, 100) > 98 ? GenerationalValues.Singleton.RandomValueBetween(1, biomeCount) : 0;
                else if (x < width + sizeToExpandByEachSide && y < height + sizeToExpandByEachSide && x > sizeToExpandByEachSide && y > sizeToExpandByEachSide) newMap[x, y] = map[x - sizeToExpandByEachSide, y - sizeToExpandByEachSide];
                else if(y > newHeight - sizeToExpandByEachSide || x > newWidth - sizeToExpandByEachSide) newMap[x, y] = GenerationalValues.Singleton.RandomValueBetween(0, 100) > 98 ? GenerationalValues.Singleton.RandomValueBetween(1, biomeCount) : 0;
                
            }
        }
        newMap = SmoothBiomesInNewArea(newHeight, newWidth, newMap, sizeToExpandByEachSide);
        map = newMap;
        width = newWidth;
        height = newHeight;
    }

    /// <summary>
    /// Function that runs through the map vertically and horizontally smoothing out the map by expanding on newly generated biomes in new map sectors
    /// </summary>
    /// <param name="newHeight"> new height variable</param>
    /// <param name="newWidth"> new width varaible</param>
    /// <param name="newMap"> new map array</param>
    /// <param name="sizeExpandedBy"> size by which the map is expanded by</param>
    /// <returns>newMap array after smoothing</returns>
    private int[,] SmoothBiomesInNewArea(int newHeight, int newWidth, int[,] newMap, int sizeExpandedBy)
    {
        int generatedBiomeOverrideSize = sizeExpandedBy / 20;
        int xLoopLeft = sizeExpandedBy + generatedBiomeOverrideSize;
        int yLoopBottom = sizeExpandedBy + generatedBiomeOverrideSize;
        int xLoopRight = width + sizeExpandedBy - generatedBiomeOverrideSize;
        int yLoopTop = height + sizeExpandedBy - generatedBiomeOverrideSize;

        for (int i = 0; i != smoothingIterations; ++i)
        {

            for (int x = 0; x < newWidth; x++)
            {
                //Top
                for (int y = yLoopTop; y < newHeight; y++)
                {
                    if(y >= height - generatedBiomeOverrideSize)
                    {
                        if (newMap[x, y] > 0)
                        {
                            newMap = SmoothWithNeighbours1x1NewArea(x, y, newMap[x, y], newMap, newWidth, newHeight);
                        }
                    }
                    
                }
            
                //Bottom
                for (int y = newHeight / 2; y > 0; y--)
                {
                    if (y < sizeExpandedBy + generatedBiomeOverrideSize)
                    {
                        if (newMap[x, y] > 0)
                        {
                            newMap = SmoothWithNeighbours1x1NewArea(x, y, newMap[x, y], newMap, newWidth, newHeight);
                        }
                    }


                }
            }
            for (int y = 0; y < newHeight; y++)
            {
                //Left
                for (int x = newWidth / 2; x > 0; x--)
                {
                    if (x < sizeExpandedBy + generatedBiomeOverrideSize)
                    {
                        if (newMap[x, y] > 0)
                        {
                            newMap = SmoothWithNeighbours1x1NewArea(x, y, newMap[x, y], newMap, newWidth, newHeight);
                        }
                    }


                }
            
                //Right
                for (int x = xLoopRight; x < newWidth; x++)
                {
                    if (x >= width - generatedBiomeOverrideSize)
                    {
                        if (newMap[x, y] > 0)
                        {
                            newMap = SmoothWithNeighbours1x1NewArea(x, y, newMap[x, y], newMap, newWidth, newHeight);
                        }
                    }
                    
                }
            }


        }

        return newMap;
    }

    /// <summary>
    /// Function that runs through the whole map vertically expanding on each biome hit
    /// </summary>
    private void SmoothBiomes()
    {
        
        for (int i = 0; i != smoothingIterations; ++i)
        {
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (map[x,y] > 0)
                    {
                        SmoothWithNeighbours1x1(x, y, map[x, y]);
                    }
                    

                }
            }
            for (int x = width; x < 0; x--)
            {
                for (int y = height; y < 0; y--)
                {
                    if (map[x, y] > 0)
                    {
                        SmoothWithNeighbours1x1(x, y, map[x, y]);
                    }


                }
            }
            

        }
        
    }

    /// <summary>
    /// Function that will expand out initial map values by checking its neighbouring tiles, it will also override surrounding biomes in contact depending on random fill percent setting
    /// This Function is for smoothing out newly generated terrain
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="biomeToExpand"></param>
    private void SmoothWithNeighbours1x1(int x, int y, int biomeToExpand)
    {
        //Checking Neighbours
        for (int neighbouringX = x - 1; neighbouringX <= x + 1; neighbouringX++)
        {
            for (int neighbouringY = y - 1; neighbouringY <= y + 1; neighbouringY++)
            {
                if (neighbouringX >= 0 && neighbouringX < width && neighbouringY >= 0 && neighbouringY < height)
                {
                    if (map[neighbouringX, neighbouringY] > 0) map[neighbouringX, neighbouringY] = (GenerationalValues.Singleton.RandomValueBetween(0,100) > randomFillPercent) ? biomeToExpand : map[neighbouringX, neighbouringY];
                    else map[neighbouringX, neighbouringY] = biomeToExpand;
                }
                else
                {
                    map[x, x] = 0;
                }
            }
        }
    }

    /// <summary>
    /// Function that will expand out initial map values by checking its neighbouring tiles, it will also override surrounding biomes in contact depending on random fill percent setting 
    /// This Function is for smoothing out newly generated terrain after expanding the map
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="biomeToExpand"></param>
    /// <param name="newMap"></param>
    /// <param name="newWidth"></param>
    /// <param name="newHeight"></param>
    /// <returns></returns>
    private int[,] SmoothWithNeighbours1x1NewArea(int x, int y, int biomeToExpand, int[,] newMap, int newWidth, int newHeight)
    {
        //Checking Neighbours
        for (int neighbouringX = x - 1; neighbouringX <= x + 1; neighbouringX++)
        {
            for (int neighbouringY = y - 1; neighbouringY <= y + 1; neighbouringY++)
            {
                if (neighbouringX >= 0 && neighbouringX < newWidth && neighbouringY >= 0 && neighbouringY < newHeight)
                {
                    if (newMap[neighbouringX, neighbouringY] > 0) newMap[neighbouringX, neighbouringY] = (GenerationalValues.Singleton.RandomValueBetween(0, 100) > randomFillPercent) ? biomeToExpand : newMap[neighbouringX, neighbouringY];
                    else newMap[neighbouringX, neighbouringY] = biomeToExpand;


                }
                else
                {
                    newMap[x, x] = 0;
                }
            }
        }

        return newMap;
    }

    /// <summary>
    /// Function to create borders between biomes by setting the sections which have contact to 0 (will create collider objects in these areas in later code)
    /// </summary>
    private void CreateBordersBetweenBiomes()
    {
        int previousTile;
        int currentTile = map[0,0];
        for(int x = 0; x < width ; x++)
        {
            for(int y = 0; y < height ; y++)
            {
                if(y == 0)
                {
                    currentTile = map[x, y];
                    
                }
                else
                {
                    previousTile = currentTile;
                    currentTile = map[x, y];
                    if (previousTile != currentTile && currentTile != 0 && previousTile != 0)
                    {
                        map[x, y] = 0;
                        map[x, y - 1] = 0;

                        previousTile = 0;
                        currentTile = 0;
                    }
                }
                
            }
             
        }
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (x == 0)
                {
                    currentTile = map[x, y];

                }
                else
                {
                    previousTile = currentTile;
                    currentTile = map[x, y];
                    if (previousTile != currentTile && currentTile != 0 && previousTile != 0)
                    {
                        map[x, y] = 0;
                        map[x - 1, y] = 0;

                        previousTile = 0;
                        currentTile = 0;
                    }
                }

            }

        }
    }

}

/*
#if UNITY_EDITOR
[CustomEditor(typeof(CellularAutomata))]
public class CellularAutomataCustomInspector : Editor
{
    private void OnEnable()
    {
    }

    public override void OnInspectorGUI()
    {
        CellularAutomata pathfind = (CellularAutomata)target;
        if (DrawDefaultInspector())
        {
            //Include Function call here to auto update values in this inspector

        }

    }
}
#endif
*/