//Created by: Kamil Woloszyn
//In the Years 2024-2025
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PCG_Manager : MonoBehaviour
{
    public static PCG_Manager Singleton;

    //Variables for Testing
    Button regenerateMapButton;
    Button enlargeMapButton;
    Button smoothMapButton;
    Button createPathsButton;
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
        CellularAutomata.Singleton.GenerateMap_CA();
        TileMapLinker.Singleton.SetTileMap();
        TileMapLinker.Singleton.SetUpTileMapForPerlin(PerlinNoise.Singleton.GenerateHeightMap());
        StructureGeneration.Singleton.StartGenerationOfStructuresAndPaths();
        
    }

    /// <summary>
    /// Function to override the current PCG variable data with new data
    /// </summary>
    /// <param name="map"></param>
    /// <param name="randomFillPercent"></param>
    /// <param name="biomeCount"></param>
    /// <param name="smoothingIterations"></param>
    /// <param name="maxPlotCountPerBiome"></param>
    /// <param name="createBordersBetweenBiomes"></param>
    /// <param name="similarityCutOff"></param>
    /// <param name="height"></param>
    /// <param name="width"></param>
    /// <param name="seed"></param>
    /// <param name="useRandomSeed"></param>
    /// <param name="sizeToExpandByEachSide"></param>
    public void SetPCGValuesForCellularAutomata(int[,] map, int randomFillPercent, int biomeCount, int smoothingIterations, int maxPlotCountPerBiome, bool createBordersBetweenBiomes, int similarityCutOff, int height, int width, string seed, bool useRandomSeed, int sizeToExpandByEachSide)
    {
        GenerationalValues.Singleton.SetBiomeMap(map);
        GenerationalValues.Singleton.SetRandomFillPercent(randomFillPercent);
        GenerationalValues.Singleton.SetBiomeCount(biomeCount);
        GenerationalValues.Singleton.SetSmoothingIterations(smoothingIterations);
        GenerationalValues.Singleton.SetMaxPlotCountPerBiomes(maxPlotCountPerBiome);
        GenerationalValues.Singleton.SetCreateBordersBetweenBiomes(createBordersBetweenBiomes);
        GenerationalValues.Singleton.SetSimilarityCutOff(similarityCutOff);
        GenerationalValues.Singleton.SetMapSize(width, height);
        GenerationalValues.Singleton.SetSeed(seed);
        GenerationalValues.Singleton.SetUseRandomSeed(useRandomSeed);
        GenerationalValues.Singleton.SetExpandMapBySize(sizeToExpandByEachSide);
    }

    /// <summary>
    /// Function to override the current PCG variable data with new data
    /// </summary>
    /// <param name="structureMap"></param>
    /// <param name="height"></param>
    /// <param name="width"></param>
    /// <param name="structuresToGenerate"></param>
    /// <param name="minDistBetweenStructures"></param>
    /// <param name="pathsFromEachStructure"></param>
    public void SetPCGValuesForPathsAndStructures(int[,] structureMap, int height, int width, int structuresToGenerate,int minDistBetweenStructures,int pathsFromEachStructure)
    {
        GenerationalValues.Singleton.SetStructureMap(structureMap);
        GenerationalValues.Singleton.SetMapSize(width,height);
        GenerationalValues.Singleton.SetStructuresToGenerate(structuresToGenerate);
        GenerationalValues.Singleton.SetMinDistanceBetweenStructures(minDistBetweenStructures);
        GenerationalValues.Singleton.SetPathsFromEachStructure(pathsFromEachStructure);
    }
    
}

#if UNITY_EDITOR
[CustomEditor(typeof(PCG_Manager))]
public class PCG_ManagerCustomInspector : Editor
{
    private void OnEnable()
    {
    }

    public override void OnInspectorGUI()
    {
        PCG_Manager pathfind = (PCG_Manager)target;
        if (DrawDefaultInspector())
        {
            //Include Function call here to auto update values in this inspector

        }

    }
}
#endif
