//Created by: Kamil Woloszyn
//In the Years 2024-2025
using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CustomEditor (typeof (GenerationalValues))]
public class AutoUpdatingInspector : Editor
{
    
    public override void OnInspectorGUI()
    {
        GenerationalValues mapGen = (GenerationalValues)target;
        if (DrawDefaultInspector())
        {
            if (Application.isPlaying)
            {
                if (mapGen.autoUpdate)
                {
                    AutoClearTileMap();
                }
            }

        }

         

        if (GUILayout.Button("Clear Map"))
        {
            AutoClearTileMap();
        }

    }

    private void AutoClearTileMap()
    {
        /*
        //Finding the tile map in the hirerarchy
        Tilemap tileMapReference = SceneAsset.FindFirstObjectByType<Tilemap>().GetComponent<Tilemap>() as Tilemap;
        if(tileMapReference == null)
        {
            tileMapReference = FindObjectOfType<Tilemap>() as Tilemap;
            if(tileMapReference == null)
            {
                tileMapReference = GameObject.Find("Tilemap").GetComponent<Tilemap>();
                if(tileMapReference == null)
                {
                    tileMapReference = GameObject.Find("TileMap").GetComponent<Tilemap>();
                    if(tileMapReference == null)
                    {
                        tileMapReference = GameObject.Find("tilemap").GetComponent<Tilemap>();
                    }
                }

            }
        }

        //Clearing the found tilemap
        if(tileMapReference != null)
        {
            //tileMapReference.ClearAllTiles();
        }
        */
    }
    
}
