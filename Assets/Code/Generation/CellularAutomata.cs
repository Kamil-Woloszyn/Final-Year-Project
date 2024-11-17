using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CellularAutomata : MonoBehaviour
{
    int[,] map;
    [Range(0,100)]
    public int randomFillPercent;

    [Range(2,10)]
    public int biomeCount;

    [Range(0, 100)]
    public int smoothingIterations;

    public int width;
    public int height;

    public string seed;
    public bool useRandomSeed;

    public System.Random psudoRandomGenerator;

    Button regenerateMapButton;
    Button smoothMapButton;
    List<Color> colorsList;

    //Private Variables
    public int similarityCutOff;
    private float time = 0;

    private void Start()
    {
        //Assigning Variables
        psudoRandomGenerator = new System.Random(seed.GetHashCode());
        colorsList = new List<Color>();

        //Adding buttons + listeners
        regenerateMapButton = GameObject.FindGameObjectWithTag("GenerateButton").GetComponent<Button>();
        regenerateMapButton.onClick.AddListener(RegenerateMap);

        smoothMapButton = GameObject.FindGameObjectWithTag("SmoothButton").GetComponent<Button>();
        smoothMapButton.onClick.AddListener(SmoothMapOut);

        //Functions
        PopulateColorList();
        GenerateMap();
    }
    private void Update()
    {
        time += Time.deltaTime;
        if(time >= 0.1f)
        {
            SmoothMapOut();
            time = 0;
        }
    }

    void RegenerateMap()
    {
        PopulateColorList();
        GenerateMap();
    }

    void GenerateMap()
    {
        map = new int[width,height];
        RandomFillMap();
    }

    void RandomFillMap()
    {
        if(useRandomSeed)
        {
            seed = Time.time.ToString();
        }


        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                if(x == 0 || y == 0 || x == width - 1 || y == height - 1)
                {
                    map[x, y] = 0;
                }
                else
                {
                    map[x, y] = (psudoRandomGenerator.Next(0, 100) < randomFillPercent)? psudoRandomGenerator.Next(biomeCount):0;
                }

                
            }
        }
    }

    void SmoothMapOut()
    {
        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int surroundingNoBiomeTiles = GetSurroundingSimilarBiomeCount(x, y, map[x,y]);

                //If Map Tile doesnt have enough relative similar biomes it will look for the most likely pair
                if((surroundingNoBiomeTiles >= similarityCutOff))
                {
                    map[x, y] = FindHighestOccouringBiomeInRange(x, y);
                }
            }
        }
        for(int i = 0; i != smoothingIterations; ++i)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    map[x, y] = FindHighestOccouringBiomeInRange(x, y);
                }
            }
            
        }
        
    }

    int FindHighestOccouringBiomeInRange(int gridX, int gridY)
    {
        //Creating and initializing array to store counts
        int[] arrayOfBiomeCount = new int[100];
        for(int  i = 0; i != 100; ++i)
        {
            arrayOfBiomeCount[i] = 0;
        }

        //Checking Neighbours
        for (int neighbourX = gridX - 2; neighbourX <= gridX + 2; neighbourX++)
        {
            for (int neighbourY = gridY - 2; neighbourY <= gridY + 2; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        arrayOfBiomeCount[map[neighbourX,neighbourY]] += 1;
                    }
                }
            }
        }
        int maxValue = arrayOfBiomeCount.Max();
        int maxIndex = arrayOfBiomeCount.ToList().IndexOf(maxValue);
        if (maxIndex == 0)
        {
            List<int> listOfBiomeCount = arrayOfBiomeCount.ToList();
            listOfBiomeCount.RemoveAt(maxIndex);
            maxIndex = listOfBiomeCount.IndexOf(listOfBiomeCount.Max());
        }
        return maxIndex;
    }

    int GetSurroundingSimilarBiomeCount(int gridX, int gridY, int biomeTile)
    {
        int similarBiomeCount = 0;
        for(int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if(neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    if(neighbourX != gridX || neighbourY != gridY)
                    {
                        if (map[neighbourX,neighbourY] == biomeTile)
                        {
                            similarBiomeCount++;
                        }
                    }
                }
                
            }
        }

        return similarBiomeCount;
    }


    private void OnDrawGizmos()
    {
        
        if(map != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (map[x, y] != 0)
                    {
                        Gizmos.color = colorsList[map[x, y]];
                    }
                    else
                    {
                        Gizmos.color = Color.black;
                    }

                    Vector3 position = new Vector3(-width / 2 + x + 0.5f, 0, -height / 2 + y + 0.05f);
                    Gizmos.DrawCube(position, Vector3.one);
                }
            }
        }
    }

    void PopulateColorList()
    {
        colorsList.Add(Color.white);
        colorsList.Add(Color.red);
        colorsList.Add(Color.yellow);
        colorsList.Add(Color.blue);
        colorsList.Add(new Color(1f,50f,32f,255));
        colorsList.Add(Color.cyan);
        colorsList.Add(Color.grey);
        colorsList.Add(Color.magenta);
        colorsList.Add(new Color(214f, 126f, 44f, 225));
        colorsList.Add(new Color(240f, 128f, 128f, 225));



    }
}
