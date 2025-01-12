using System;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;


[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class ConditionalHideAttribute : PropertyAttribute
{
    //The name of the bool field that will be in control
    public string ConditionalSourceField = "";
    //TRUE = Hide in inspector / FALSE = Disable in inspector 
    public bool HideInInspector = false;

    public ConditionalHideAttribute(string conditionalSourceField)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.HideInInspector = false;
    }

    public ConditionalHideAttribute(string conditionalSourceField, bool hideInInspector)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.HideInInspector = hideInInspector;
    }
}

///<summary>
/// This is a class that is used to store all of the different variables inside an object & allow access to the variables from different classes
/// </summary>
public class GenerationalValues : MonoBehaviour
{

    

    private int[,] biomeMap;

    [Header("Global Generation Variables")]
    [SerializeField]
    private int width;

    [SerializeField]
    private int height;

    [SerializeField]
    private string seed;

    [SerializeField]
    private bool useRandomSeed;

    [Header("Biome Generation", order = 1)]
    [Range(30, 70)]
    [SerializeField]
    private int randomFillPercent;

    [Range(2, 10)]
    [SerializeField]
    private int biomeCount;

    [Range(0, 100)]
    [SerializeField]
    private int smoothingIterations;

    [Range(0, 100)]
    [SerializeField]
    private int maxPlotCountPerBiome;

    [SerializeField]
    private bool createBordersBetweenBiomes;

    [SerializeField]
    [Range(3, 5)]
    private int similarityCutOff;

    [SerializeField]
    private int expandMapBySize;


    [Header("Path & Structure Generation", order = 1)]
    [SerializeField]
    private bool createStructures = false;

    [SerializeField]
    private bool pathsBetweenStructures;

    [SerializeField]
    [Range(2, 100)]
    private int structuresToGenerate;

    [SerializeField]
    [Range(50, 500)]
    private int minDistBetweenStructures;

    [SerializeField]
    [Range(1, 2)]
    private int pathsFromEachStructure;

    private int[,] pathAndStructureMap;



    private System.Random pseudoRandomGenerator;


    /// <summary>
    /// Singleton Instance
    /// </summary>
    public static GenerationalValues Instance { get; private set; }

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
        //Initlaizing the randomizer dependant on the setting set
        if (useRandomSeed) seed = Time.time.ToString();
        
        //Setting Up Random Generator
        pseudoRandomGenerator = new System.Random(seed.GetHashCode());

        //Initializing Map
        biomeMap = new int[width, height];
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
}
