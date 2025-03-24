//Created by: Kamil Woloszyn
//In the Years 2024-2025
using UnityEditor;
using UnityEngine;

public class PerlinNoise : MonoBehaviour
{
    public static PerlinNoise Singleton = null;
    private int height;
    private int width;
    private int seed;
    private float scale;
    private float lacunarity;
    private float persistance;
    private int octaves;
    private Vector2 offset;


    private void Awake()
    {
        if(Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Destroy(this);
        }

    }

    private void InitializeVariables()
    {
        height = GenerationalValues.Singleton.GetHeight();
        width = GenerationalValues.Singleton.GetWidth();
        seed = GenerationalValues.Singleton.GetSeed().GetHashCode();
        scale = GenerationalValues.Singleton.GetScale();
        lacunarity = GenerationalValues.Singleton.GetLacunarity();
        persistance = GenerationalValues.Singleton.GetPersistance();
        octaves = GenerationalValues.Singleton.GetOctaves();
        offset = GenerationalValues.Singleton.GetMapOffset();
    }

    public float[,] GenerateHeightMap()
    {
        InitializeVariables();
        float[,] heightMap = GenerateNoiseMap();
        return heightMap;
    }

    

    public float[,] GenerateNoiseMap()
    {
        float[,] noiseMap = new float[width, height];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = width / 2f;
        float halfHeight = height / 2f;


        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }

}



#if UNITY_EDITOR
[CustomEditor(typeof(PerlinNoise))]
public class PerlinNoiseCustomInspector : Editor
{
    private void OnEnable()
    {
    }

    public override void OnInspectorGUI()
    {
        PerlinNoise pathfind = (PerlinNoise)target;
        if (DrawDefaultInspector())
        {
            //Include Function call here to auto update values in this inspector

        }

    }
}
#endif