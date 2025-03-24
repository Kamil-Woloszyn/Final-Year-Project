using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
/// <summary>
/// Class of tile storing all relevant information on a specific tile
/// Values to be assigned inside of the inspector
/// </summary>
[System.Serializable]
public class Tile
{
    public TileBase tileTexture;
    public bool walkable;
    public bool hazardous;

}
