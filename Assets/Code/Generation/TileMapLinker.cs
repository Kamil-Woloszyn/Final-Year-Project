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
    public TileBase[] tiles;

    private GenerationalValues generationalValues;

    /// <summary>
    /// Singleton instance
    /// </summary>
    public static TileMapLinker Instance {  get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Public function to setup tilemap using a numerical map
    /// </summary>
    public void SetTileMap()
    {
        terrainMap = GenerationalValues.Instance.GetBiomeMap();

        SetUpTileMap();
    }

    /// <summary>
    /// Function to set up tile map with necessary parameters
    /// </summary>
    private void SetUpTileMap()
    {
        int width = GenerationalValues.Instance.GetWidth();
        int height = GenerationalValues.Instance.GetHeight();
        int heightOffset = height / 2;
        int widthOffset = width / 2;

        //Looping through 2D numerical map array
        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                tileMap.SetTile(new Vector3Int(x - widthOffset, y - heightOffset, 0), tiles[terrainMap[x, y]]);
            }
        }
        
    }
}
