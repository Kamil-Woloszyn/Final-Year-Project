using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PCG_Manager : MonoBehaviour
{
    public static PCG_Manager Instance;

    //Variables for Testing
    Button regenerateMapButton;
    Button enlargeMapButton;
    Button smoothMapButton;
    Button createPathsButton;
    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {

        //Adding buttons + listeners
        regenerateMapButton = GameObject.FindGameObjectWithTag("GenerateButton").GetComponent<Button>();
        regenerateMapButton.onClick.AddListener(CellularAutomata.Instance.GenerateMap);

        enlargeMapButton = GameObject.FindGameObjectWithTag("EnlargeButton").GetComponent<Button>();
        enlargeMapButton.onClick.AddListener(CellularAutomata.Instance.ExpandMap);

        smoothMapButton = GameObject.FindGameObjectWithTag("SmoothButton").GetComponent<Button>();
        smoothMapButton.onClick.AddListener(TileMapLinker.Instance.SetTileMap);

        createPathsButton = GameObject.FindGameObjectWithTag("CreatePaths").GetComponent<Button>();
        createPathsButton.onClick.AddListener(StructureGeneration.Instance.StartGenerationOfStructuresAndPaths);
        
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
        GenerationalValues.Instance.SetBiomeMap(map);
        GenerationalValues.Instance.SetRandomFillPercent(randomFillPercent);
        GenerationalValues.Instance.SetBiomeCount(biomeCount);
        GenerationalValues.Instance.SetSmoothingIterations(smoothingIterations);
        GenerationalValues.Instance.SetMaxPlotCountPerBiomes(maxPlotCountPerBiome);
        GenerationalValues.Instance.SetCreateBordersBetweenBiomes(createBordersBetweenBiomes);
        GenerationalValues.Instance.SetSimilarityCutOff(similarityCutOff);
        GenerationalValues.Instance.SetMapSize(width, height);
        GenerationalValues.Instance.SetSeed(seed);
        GenerationalValues.Instance.SetUseRandomSeed(useRandomSeed);
        GenerationalValues.Instance.SetExpandMapBySize(sizeToExpandByEachSide);
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
        GenerationalValues.Instance.SetStructureMap(structureMap);
        GenerationalValues.Instance.SetMapSize(width,height);
        GenerationalValues.Instance.SetStructuresToGenerate(structuresToGenerate);
        GenerationalValues.Instance.SetMinDistanceBetweenStructures(minDistBetweenStructures);
        GenerationalValues.Instance.SetPathsFromEachStructure(pathsFromEachStructure);
    }
    
}
