using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
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
    private int addPatternsPerIteration;
    private int maxPathsFromStartPoints;
    private int iterationCount;

    private int acceptedX = 0;
    private int acceptedY = 0;
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
        
        InitializeVariables();
        ResetMap();
        List<int[]> coords = new List<int[]>();
        coords.Add(new int[] { width / 2, height / 2 });
        coords = GenerateStructureFromStartPoint(coords, coords.Count);
        foreach (int[] coord in coords)
        {
            Debug.Log(coord[0] +"," + coord[1]);
        }
        TileMapLinker.Instance.SetUpTileMapForPaths(structureMap);
    }

    private void ResetMap()
    {
        structureMap = new int[width, height];
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height;y++)
            {
                structureMap[x, y] = 0;
            }
        }
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
        iterationCount = GenerationalValues.Instance.GetStructureIterationCount();
        addPatternsPerIteration = GenerationalValues.Instance.GetAddPatternsPerIteration();
    }

    /// <summary>
    /// Function to update the variables stored in a different class to their updated values
    /// </summary>
    private void UpdateOriginalVariables()
    {
        PCG_Manager.Instance.SetPCGValuesForPathsAndStructures(structureMap, height, width, structuresToGenerate, minDistBetweenStructures, pathsFromEachStructure);
    }

    /// <summary>
    /// Function to generate structures start points where 1 is a structure centre point and 2 is a path
    /// </summary>
    private void GenerateStructures()
    {
        structureMap = new int[width, height];
        int maxAmountOfStructuresForMapSize = width / minDistBetweenStructures + height / minDistBetweenStructures;
        if(maxAmountOfStructuresForMapSize < structuresToGenerate)
        {
            structuresToGenerate = maxAmountOfStructuresForMapSize;
        }
        int[,] coordinatesOfStartingPoints = new int[maxAmountOfStructuresForMapSize, maxAmountOfStructuresForMapSize];



        int startingStructureX = GenerationalValues.Instance.RandomValueBetween(0, width);
        int startingStructureY = GenerationalValues.Instance.RandomValueBetween(0, height);
        //Starting Structure
        structureMap[startingStructureX, startingStructureY] = 1;
        int structuresGenerated = 0;
        for (int i = 0; i < structuresToGenerate; i++)
        {
            if(GenerateStructureCentre(startingStructureX,startingStructureY))
            {
                structuresGenerated++;
                structureMap[acceptedX, acceptedY] = 1;
            }

            startingStructureX = GenerationalValues.Instance.RandomValueBetween(0, width);
            startingStructureY = GenerationalValues.Instance.RandomValueBetween(0, height);
        }
    }

    /// <summary>
    /// Function to generate the starting points before L system algorithm is applied
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>boolean that states whether the structure was generated or not</returns>
    private bool GenerateStructureCentre(int x, int y)
    {
        int randomAngle = GenerationalValues.Instance.RandomValueBetween(0, 360);
        int structureX = 0;
        int structureY = 0;
        bool structureAccepted = false;
        int attemptsMax = 5;
        int attemptCount = 0;
        while(!structureAccepted)
        {
            structureX = ((int)((float)minDistBetweenStructures * Mathf.Cos(randomAngle))) + x;
            structureY = ((int)((float)minDistBetweenStructures * Mathf.Sin(randomAngle))) + y;
            //Check if its in the allowed map
            if(structureX > 0 && structureX <= width && structureY > 0 && structureY <= height)
            {
                structureAccepted = true;
                acceptedX = structureX;
                acceptedY = structureY;
            }
            attemptCount++;
            if (attemptCount > attemptsMax) structureAccepted = false;
        }

        return structureAccepted;
    }

    /// <summary>
    /// Function that will create structures like cities and towns depending on settings selected in the editor,
    /// the function will generate road systems and later on add map triggers where assets would be placed.
    /// </summary>
    /// <param name="startCoords"></param>
    /// <param name="sizeOfArray"></param>
    private List<int[]> GenerateStructureFromStartPoint(List<int[]> startCoords, int sizeOfArray)
    {
        string pattern = "";
        while(pattern.Length < iterationCount && startCoords.Count >= 1)
        {
            int randomCoords = Random.Range(0,startCoords.Count);
            int[] currentCoords = startCoords[randomCoords];
            //Patterns can be NESW
            //N - North - x, +y
            //E - East - +x, y
            //S - South - x, -y
            //W - West - -x, y


            
            //Chose randomDirection
            
            switch (Random.Range(0, 4))
            {
                case 0:
                    pattern += "N";
                    break;
                case 1:
                    pattern += "E";
                    break;
                case 2:
                    pattern += "S";
                    break;
                case 3:
                    pattern += "W";
                    break;
                default:
                    pattern += "N";
                    break;
            }

            
            
            foreach (int[] coords in startCoords.ToList())
            {
                int pathsBlocked = 0;
                if (!(coords[0] < 0 || coords[0] > width || coords[1] < 0 || coords[1] > height ||
                    coords[0] + 1 < 0 || coords[0] + 1 > width || coords[1] + 1 < 0 || coords[1] + 1 > height ||
                    coords[0] - 1 < 0 || coords[0] - 1 > width || coords[1] - 1 < 0 || coords[1] - 1 > height))
                {
                    
                    //Check if each coord is still a valid point of expansion
                    if (structureMap[coords[0], coords[1] + 1] == 1)
                    {
                        pathsBlocked++;
                    }
                    if (structureMap[coords[0], coords[1] - 1] == 1)
                    {
                        pathsBlocked++;
                    }
                    if (structureMap[coords[0] + 1, coords[1]] == 1)
                    {
                        pathsBlocked++;
                    }
                    if (structureMap[coords[0] - 1, coords[1]] == 1)
                    {
                        pathsBlocked++;
                    }

                    if (pathsBlocked >= maxPathsFromStartPoints)
                    {
                        startCoords.Remove(coords);
                    }
                }
                
                

            }

            //Adding Patterns to the string
            for (int i = addPatternsPerIteration; i > 0; i--)
            {
                switch (Random.Range(0, 4))
                {
                    case 0:
                        pattern += "N";
                        break;
                    case 1:
                        pattern += "E";
                        break;
                    case 2:
                        pattern += "S";
                        break;
                    case 3:
                        pattern += "W";
                        break;
                    default:
                        pattern += "N";
                        break;
                }
            }

            char[] patternArrayChar = pattern.ToUpper().ToCharArray();
            foreach (char character in patternArrayChar)
            {
                if (character.Equals('N'))
                {
                    if (currentCoords[1] + 2 < height && currentCoords[1] + 2 >= 0)
                    {
                        structureMap[currentCoords[0], currentCoords[1]] = 1;
                        structureMap[currentCoords[0], currentCoords[1] + 1] = 1;
                        structureMap[currentCoords[0], currentCoords[1] + 2] = 1;
                        startCoords.Add(new int[] { currentCoords[0], currentCoords[1] + 2 });

                    }
                }
                else if (character.Equals('E'))
                {
                    if (currentCoords[0] + 2 < width && currentCoords[0] + 2 >= 0)
                    {
                        structureMap[currentCoords[0], currentCoords[1]] = 1;
                        structureMap[currentCoords[0] + 1, currentCoords[1]] = 1;
                        structureMap[currentCoords[0] + 2, currentCoords[1]] = 1;
                        startCoords.Add(new int[] { currentCoords[0] + 2, currentCoords[1] });

                    }
                }
                else if (character.Equals('S'))
                {
                    if (currentCoords[1] - 2 < height && currentCoords[1] - 2 >= 0)
                    {
                        structureMap[currentCoords[0], currentCoords[1]] = 1;
                        structureMap[currentCoords[0], currentCoords[1] - 1] = 1;
                        structureMap[currentCoords[0], currentCoords[1] - 2] = 1;
                        startCoords.Add(new int[] { currentCoords[0], currentCoords[1] - 2 });

                    }
                }
                else if (character.Equals('W'))
                {
                    if (currentCoords[0] - 2 < width && currentCoords[0] - 2 >= 0)
                    {
                        structureMap[currentCoords[0], currentCoords[1]] = 1;
                        structureMap[currentCoords[0] - 1, currentCoords[1]] = 1;
                        structureMap[currentCoords[0] - 2, currentCoords[1]] = 1;
                        startCoords.Add(new int[] { currentCoords[0] - 2, currentCoords[1] });
                    }

                }

            }
        }

        return startCoords;
    }

    private string RandomPattern()
    {
        string pattern = "";
        switch (GenerationalValues.Instance.RandomValueBetween(0, 3))
        {
            case 0:
                pattern += "N";
                break;
            case 1:
                pattern += "E";
                break;
            case 2:
                pattern += "S";
                break;
            case 3:
                pattern += "W";
                break;
            default:
                pattern += "N";
                break;
        }

        return pattern;
    }
}
