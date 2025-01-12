using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureGeneration : MonoBehaviour
{
    public static StructureGeneration Instance;
    private int[,] structureMap;
    private int height;
    private int width;
    private bool pathsBetweenStructures;
    private int structuresToGenerate;
    private int minDistBetweenStructures;
    private int pathsFromEachStructure;


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

    /// <summary>
    /// Function to begin the generation of structures and paths depending on settings selected by user
    /// </summary>
    public void StartGenerationOfStructuresAndPaths()
    {
        //InitializeVariables();

    }

    /// <summary>
    /// Function to initialize the variables used in this class
    /// </summary>
    private void InitializeVariables()
    {
        structureMap = GenerationalValues.Instance.GetStructureMap();
        height = GenerationalValues.Instance.GetHeight();
        width = GenerationalValues.Instance.GetWidth();
        structuresToGenerate = GenerationalValues.Instance.GetStructuresToGenerate();
        minDistBetweenStructures = GenerationalValues.Instance.GetMinDistanceBetweenStructures();
        pathsFromEachStructure = GenerationalValues.Instance.GetPathsFromEachStructure();
        pathsBetweenStructures = GenerationalValues.Instance.GetPathsBetweenStructures();
    }

    /// <summary>
    /// Function to update the variables stored in a different class to their updated values
    /// </summary>
    private void UpdateOriginalVariables()
    {
        PCG_Manager.Instance.SetPCGValuesForPathsAndStructures(structureMap, height, width, structuresToGenerate, minDistBetweenStructures, pathsFromEachStructure);
    }
}
