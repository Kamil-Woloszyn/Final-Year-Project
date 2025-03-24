//Created by: Kamil Woloszyn
//In the Years 2024-2025


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System;

public class TileMapLinker : MonoBehaviour
{
    //Global Variables
    private int[,] terrainMap;

    public Tilemap tileMap;
    List<Tile> tiles;

    bool variablesInstanciated = false;

    /// <summary>                                                                                                                                                                                                         
    /// Singleton instance
    /// </summary>
    public static TileMapLinker Singleton { get; private set; }

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
        InitializeVariables();
    }
    private void InitializeVariables()
    {
        tileMap = FindObjectOfType<Tilemap>().GetComponent<Tilemap>();
        tiles = GenerationalValues.Singleton.GetTiles();
    }


    /// <summary>
    /// Public function to setup tilemap using a numerical map
    /// </summary>
    public void SetTileMap()
    {
        terrainMap = GenerationalValues.Singleton.GetBiomeMap();

        SetUpTileMap();
    }

    /// <summary>
    /// Function to set up tile map with necessary parameters
    /// </summary>
    private void SetUpTileMap()
    {
        int width = GenerationalValues.Singleton.GetWidth();
        int height = GenerationalValues.Singleton.GetHeight();
        int heightOffset = height / 2;
        int widthOffset = width / 2;

        //Looping through 2D numerical map array
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tileMap.SetTile(new Vector3Int(x - widthOffset, y - heightOffset, 0), tiles[terrainMap[x, y]].tileTexture);
            }
        }

    }


    public void SetUpTileMapForPaths(int[,] map)
    {
        int width = GenerationalValues.Singleton.GetWidth();
        int height = GenerationalValues.Singleton.GetHeight();
        int heightOffset = height / 2;
        int widthOffset = width / 2;

        //Looping through 2D numerical map array
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x,y] != 0)
                {
                    tileMap.SetTile(new Vector3Int(x - widthOffset, y - heightOffset, 0), tiles[9].tileTexture);
                }
                
            }
        }

    }

    public void DisplayPerlinNoise()
    {
        float[,] perlinMap = PerlinNoise.Singleton.GenerateHeightMap();
        SetUpTileMapForPerlin(perlinMap);

    }

    public void SetUpTileMapForPerlin(float[,] map)
    {

        int width = GenerationalValues.Singleton.GetWidth();
        int height = GenerationalValues.Singleton.GetHeight();
        int heightOffset = height / 2;
        int widthOffset = width / 2;

        //Looping through 2D numerical map array
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] < 0.1)
                {
                    //Set the Tile to water
                    tileMap.SetTile(new Vector3Int(x - widthOffset, y - heightOffset, 0), tiles[1].tileTexture);
                }

            }
        }

    }

    public void ClearTileMap()
    {
        if(variablesInstanciated)
        {
            //tileMap.ClearAllEditorPreviewTiles();
            tileMap.ClearAllTiles();
        }
        else
        {
            InitializeVariables();
        }
    }

}
