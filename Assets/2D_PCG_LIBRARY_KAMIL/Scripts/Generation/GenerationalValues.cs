//Created by: Kamil Woloszyn
//In the Years 2024-2025
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
///<summary>
/// This is a class that is used to store all of the different variables inside an object & allow access to the variables from different classes
/// </summary>
public class GenerationalValues : MonoBehaviour
{
    //Map Arrays
    private int[,] biomeMap;

    private int[,] pathAndStructureMap;

    private float[,] heightMap;

    public bool autoUpdate;
    public bool useRandomSeed;
    [Space(20f)]
    [Header("Global Generation Variables")]
    [SerializeField]
    public int width;

    [SerializeField]
    public int height;

    [SerializeField]
    public string seed;

    [Space(20f)]
    [Header("Biome Generation", order = 1)]
    [Range(30, 70)]
    [SerializeField]
    public int randomFillPercent;

    [Range(2, 10)]
    [SerializeField]
    public int biomeCount;

    [Range(0, 100)]
    [SerializeField]
    public int smoothingIterations;

    [Range(0, 100)]
    [SerializeField]
    public int maxPlotCountPerBiome;

    [SerializeField]
    public bool createBordersBetweenBiomes;

    [SerializeField]
    [Range(3, 5)]
    public int similarityCutOff;

    [SerializeField]
    public int expandMapBySize;


    [Space(20f)]  
    [Header("Path & Structure Generation", order = 1)]
    [SerializeField]
    public bool createStructures = false;

    [SerializeField]
    public bool pathsBetweenStructures;

    [SerializeField]
    [Range(2, 100)]
    public int structuresToGenerate;

    [SerializeField]
    [Range(50, 500)]
    public int minDistBetweenStructures;

    [SerializeField]
    public int addPatternsPerIteration;

    [SerializeField]
    public int maxPathsFromStartPoints;

    [SerializeField]
    public int iterationCount;

    [SerializeField]
    [Range(1, 2)]
    public int pathsFromEachStructure;



    
    [Space(20f)]
    [Header("Perlin Noise Generation")]
    [SerializeField]
    private float scale;

    [SerializeField]
    private int octaves;

    [SerializeField]
    private float persistance;

    [SerializeField]
    private float lacunarity;

    [SerializeField]
    private Vector2 offset;

    [Space(20f)]
    [Header("Tiles Used For Generation")]

    [SerializeField]
    private List<Tile> tiles;
    /// <summary>
    /// Singleton Instance
    /// </summary>
    private System.Random pseudoRandomGenerator;

    public static GenerationalValues Singleton { get; private set; }

    private void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(this);
        }
        else
        {
            Singleton = this;
            DontDestroyOnLoad(this);
        }

        if(useRandomSeed)
        {
            seed = DateTime.Now.ToString();
        }
    }

    private void Start()
    {
        //Initlaizing the randomizer dependant on the setting set
        if (useRandomSeed) seed = Time.time.ToString();
        
        //Setting Up Random Generator
        pseudoRandomGenerator = new System.Random(seed.GetHashCode());

        //Initializing Map
        biomeMap = new int[width, height];
    }

    private void OnValidate()
    {
        if(tiles.Count < 1)
        {
            //Give the user a warning to let them know they forgot to add tiles to inspector
            Debug.LogError("Please Add Your Own Tiles in the Inspector Window");
        }
    }
    /// <summary>
    ///  Getter Function
    /// </summary>
    public int[,] GetBiomeMap()
    {
        return biomeMap;
    }

    /// <summary>
    ///  Getter Function
    /// </summary>
    public int GetRandomFillPercent()
    {
        return randomFillPercent;
    }

    /// <summary>
    ///  Getter Function
    /// </summary>
    public int GetBiomeCount()
    {
        return biomeCount;
    }

    /// <summary>
    ///  Getter Function
    /// </summary>
    public int GetSmoothingIterations()
    {
        return smoothingIterations;
    }

    /// <summary>
    ///  Getter Function
    /// </summary>
    public int GetMaxPlotCountPerBiomes()
    {
        return maxPlotCountPerBiome;
    }

    /// <summary>
    ///  Getter Function
    /// </summary>
    public bool GetCreateBordersBetweenBiomes()
    {
        return createBordersBetweenBiomes;
    }

    /// <summary>
    ///  Getter Function
    /// </summary>
    public int GetSimilarityCutOff()
    {
        return similarityCutOff;
    }

    /// <summary>
    ///  Getter Function
    /// </summary>
    public int GetHeight()
    {
        return height;
    }

    /// <summary>
    ///  Getter Function
    /// </summary>
    public int GetWidth() 
    { 
        return width; 
    }

    /// <summary>
    ///  Getter Function
    /// </summary>
    public string GetSeed()
    {
        return seed;
    }

    /// <summary>
    ///  Getter Function
    /// </summary>
    public bool GetUseRandomSeed()
    {
        return useRandomSeed;
    }

    /// <summary>
    ///  Getter Function
    /// </summary>
    public int GetExpandMapBySize()
    {
        return expandMapBySize;
    }

    /// <summary>
    ///  Getter Function
    /// </summary>
    public int[,] GetStructureMap()
    {
        return pathAndStructureMap;
    }

    /// <summary>
    ///  Getter Function
    /// </summary>
    public int GetStructuresToGenerate()
    {
        return structuresToGenerate;
    }

    /// <summary>
    ///  Getter Function
    /// </summary>
    public int GetMinDistanceBetweenStructures()
    {
        return minDistBetweenStructures;
    }

    /// <summary>
    ///  Getter Function
    /// </summary>
    public int GetPathsFromEachStructure()
    {
        return pathsFromEachStructure;
    }

    /// <summary>
    ///  Getter Function
    /// </summary>
    public bool GetCreateStructure()
    {
        return createStructures;
    }

    /// <summary>
    ///  Getter Function
    /// </summary>
    public bool GetPathsBetweenStructures()
    {
        return pathsBetweenStructures;
    }

    /// <summary>
    ///  Getter Function
    /// </summary>
    public int GetStructureIterationCount()
    {
        return iterationCount;
    }

    /// <summary>
    ///  Getter Function
    /// </summary>
    public int GetAddPatternsPerIteration()
    {
        return addPatternsPerIteration;
    }

    /// <summary>
    ///  Getter Function
    /// </summary>
    public float GetScale()
    {
        return scale;
    }

    /// <summary>
    ///  Getter Function
    /// </summary>
    public float GetLacunarity()
    {
        return lacunarity;
    }

    /// <summary>
    ///  Getter Function
    /// </summary>
    public float GetPersistance()
    {
        return persistance;
    }

    /// <summary>
    ///  Getter Function
    /// </summary>
    public int GetOctaves()
    {
        return octaves;
    }

    /// <summary>
    ///  Getter Function
    /// </summary>
    public Vector2 GetMapOffset()
    {
        return offset;
    }

    /// <summary>
    ///  Getter Function
    /// </summary>
    public List<Tile> GetTiles()
    {
        return tiles;
    }

    /// <summary>
    ///  Getter Function
    /// </summary>
    public System.Random GetSeededRandomizerObject()
    {
        return pseudoRandomGenerator;
    }

    /// <summary>
    ///  Setter Function
    /// </summary>
    public void SetBiomeMap(int[,] biomeMap)
    {
        this.biomeMap = biomeMap;
    }

    /// <summary>
    ///  Setter Function
    /// </summary>
    public void SetRandomFillPercent(int randomFillPercent)
    {
        this.randomFillPercent = randomFillPercent;
    }

    /// <summary>
    ///  Setter Function
    /// </summary>
    public void SetBiomeCount(int biomeCount)
    {
        this.biomeCount = biomeCount;
    }

    /// <summary>
    ///  Setter Function
    /// </summary>
    public void SetSmoothingIterations(int smoothingIterations)
    {
        this.smoothingIterations = smoothingIterations;
    }

    /// <summary>
    ///  Setter Function
    /// </summary>
    public void SetMaxPlotCountPerBiomes(int maxPlotCountPerBiome)
    {
        this.maxPlotCountPerBiome = maxPlotCountPerBiome;
    }

    /// <summary>
    ///  Setter Function
    /// </summary>
    public void SetCreateBordersBetweenBiomes(bool createBordersBetweenBiomes)
    {
        this.createBordersBetweenBiomes = createBordersBetweenBiomes;
    }

    /// <summary>
    ///  Setter Function
    /// </summary>
    public void SetSimilarityCutOff(int similarityCutOff)
    {
        this.similarityCutOff = similarityCutOff;
    }

    /// <summary>
    ///  Setter Function
    /// </summary>
    public void SetHeight(int height)
    {
        this.height = height;
    }

    /// <summary>
    ///  Setter Function
    /// </summary>
    public void SetWidth(int width)
    {
        this.width = width;
    }

    /// <summary>
    ///  Setter Function
    /// </summary>
    public void SetSeed(string seed)
    {
        this.seed = seed;
    }

    /// <summary>
    ///  Setter Function
    /// </summary>
    public void SetUseRandomSeed(bool useRandomSeed)
    {
        this.useRandomSeed = useRandomSeed;
    }

    /// <summary>
    ///  Setter Function
    /// </summary>
    public void SetExpandMapBySize(int expandMapBySize)
    {
        this.expandMapBySize = expandMapBySize;
    }

    /// <summary>
    ///  Setter Function for both height and width
    /// </summary>
    public void SetMapSize(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    /// <summary>
    ///  Setter Function
    /// </summary>
    public void SetStructureMap(int[,] pathAndStructureMap)
    {
        this.pathAndStructureMap = pathAndStructureMap;
    }

    /// <summary>
    ///  Setter Function
    /// </summary>
    public void SetStructuresToGenerate(int structuresToGenerate)
    {
        this.structuresToGenerate = structuresToGenerate;
    }

    /// <summary>
    ///  Setter Function
    /// </summary>
    public void SetMinDistanceBetweenStructures(int minDistBetweenStructures)
    {
        this.minDistBetweenStructures = minDistBetweenStructures;
    }

    /// <summary>
    ///  Setter Function
    /// </summary>
    public void SetPathsFromEachStructure(int pathsFromEachStructure)
    {
        this.pathsFromEachStructure = pathsFromEachStructure;
    }



    /*
     * CUSTOM FUNCTIONS FOR PSEUDO RANDOM NUMBER GENERATION
     */

    public int RandomValueBetween(int min, int max)
    {
        return pseudoRandomGenerator.Next(min, max);
    }

    public void ChangeRandomizerSeed(string seed)
    {
        //Setting Up Random Generator
        pseudoRandomGenerator = new System.Random(seed.GetHashCode());
    }

    public void ChangeRandomizerSeed()
    {
        //Initlaizing the randomizer dependant on the setting set
        this.seed = Time.time.ToString();

        //Setting Up Random Generator
        pseudoRandomGenerator = new System.Random(this.seed.GetHashCode());
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GenerationalValues))]
public class GenerationalValuesCustomInspector : Editor
{

    private void OnEnable()
    {

    }

    public override void OnInspectorGUI()
    {
        if (DrawDefaultInspector())
        {
            //Include Function call here to auto update values in this inspector

        }


    }
}
#endif