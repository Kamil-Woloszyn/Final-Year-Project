//Created by: Kamil Woloszyn
//In the Years 2024-2025
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using UnityEditor.PackageManager.UI;
using UnityEngine.TextCore.Text;

public class InstructionsTab : EditorWindow
{
    

    [MenuItem("PCG/Instructions")]
    public static void ShowWindow()
    {
        Debug.Log(System.Environment.CurrentDirectory + "/Assets/PDFs/Instructions.pdf");
        Application.OpenURL(System.Environment.CurrentDirectory + "/Assets/PDFs/Instructions.pdf");
    }

}
public class ProceduralGenerationToolEditorMode : EditorWindow
{
    string objectBaseName = "MapManager";
    int objectID = 1;
    GameObject objectToSpawn;
    float objectScale;
    float spawnRadius = 5f;

    //Tool Variables
    int randomFillPercent = 60;
    int biomeCount = 2;
    int SmoothingIterations = 1;
    int maxPlotCountPerBiome = 50;
    bool createBordersBetweenDifferentBiomes = false;
    int similarityCutOff = 3;
    int mapHeight = 500;
    int mapWidth = 500;
    string seed = "";
    bool useRandomSeed = true;
    Tilemap tileMap = null;

    //Editor Variables
    GUISkin skin;
    Texture2D headerTexture;
    Color headerBackgroundColor;
    Vector2 scrollPosition;

    [MenuItem("PCG/Editor Mode Procedural Tool")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ProceduralGenerationToolEditorMode));
    }

    private void OnEnable()
    {
        skin = Resources.Load<GUISkin>("GUIStyles/EditorSkin");
        headerTexture = new Texture2D(1, 1);
        headerBackgroundColor = new Color(13f / 255f, 32f / 255f, 44f / 255f, 1f);
        scrollPosition = new Vector2(0,0);
    }


    private void OnGUI()
    {
        Rect headerArea = new Rect(0, 0, Screen.width, 50);
        Rect bodyArea = new Rect(0, 50, Screen.width, 1500);
        headerTexture.SetPixel(0, 0, headerBackgroundColor);
        headerTexture.Apply();
        GUI.DrawTexture(headerArea, headerTexture);


        GUILayout.Label("Procedural Generation Editor (DEPRECATED)", skin.GetStyle("Header1"));
        GUILayout.Label("For use to compile map outside of play mode (Non Runtime Generation) \n", skin.GetStyle("Sub-Header1") );
        
        scrollPosition = GUI.BeginScrollView(bodyArea, scrollPosition, new Rect(0, 50, Screen.width, 0));
        objectBaseName = EditorGUILayout.TextField("Base Name", objectBaseName);
        objectID = EditorGUILayout.IntField("Object ID", objectID);
        objectScale = EditorGUILayout.Slider("Object Scale", objectScale, 0.5f, 3f);
        spawnRadius = EditorGUILayout.FloatField("Spawn Radius", spawnRadius);
        objectToSpawn = EditorGUILayout.ObjectField("Prefab to spawn", objectToSpawn, typeof(GameObject), false) as GameObject;

        randomFillPercent = EditorGUILayout.IntSlider("Random Fill Percent: ", randomFillPercent, 1, 100);
        biomeCount = EditorGUILayout.IntSlider("Biome Count: ", biomeCount, 2, 8);
        SmoothingIterations = EditorGUILayout.IntSlider("Smoothing Iterations: ",SmoothingIterations, 1, 10);
        maxPlotCountPerBiome = EditorGUILayout.IntSlider("Max Plots Per Biome: ",maxPlotCountPerBiome, 1, 100);
        createBordersBetweenDifferentBiomes = EditorGUILayout.Toggle("Borders Between Biomes? ",createBordersBetweenDifferentBiomes);
        similarityCutOff = EditorGUILayout.IntSlider("Similary Cut Off: ",similarityCutOff,2,6);
        mapHeight = EditorGUILayout.IntField("Height: ", mapHeight);
        mapWidth = EditorGUILayout.IntField("Width: ",mapWidth);
        seed = EditorGUILayout.TextField("Seed: ",seed);
        useRandomSeed = EditorGUILayout.Toggle("Use Random Seed? ",useRandomSeed);
        tileMap = EditorGUILayout.ObjectField("TileMap Object: ", objectToSpawn, typeof(Tilemap), false) as Tilemap; 

        if (GUILayout.Button("Spawn Object"))
        {
            SpawnObject();
        }
        if (GUILayout.Button("Clear Tile Map"))
        {
            ClearMap();
        }
        GUI.EndScrollView();

    }


    private void SpawnObject()
    {
        //if(objectToSpawn == null)
        //{
        //    Debug.LogError("Error: Please assign an object to be spawned.");
        //    return;
        // }
        if(objectBaseName == string.Empty)
        {
            Debug.LogError("Error: Please enter a base name for the object.");
            return;
        }

        Vector2 spawnCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = new Vector3(spawnCircle.x, 0f, spawnCircle.y);

        GameObject newObject = Instantiate(new GameObject(), spawnPos, Quaternion.identity);
        newObject.name = objectBaseName + objectID;
        newObject.AddComponent<CellularAutomata>();
        newObject.transform.localScale = Vector3.one * objectScale;

        objectID++;
    }

    private void ClearMap()
    {
        tileMap.ClearAllTiles();
    }
}

public class ProceduralGenerationToolPlayMode : EditorWindow
{
    //Variables for this window
    GameObject newObject;
    //Editor Variables
    GUISkin skin;
    Texture2D headerTexture;
    Color headerBackgroundColor;
    bool managerSpawnedInHirearchy;

    [MenuItem("PCG/Playmode Procedural Tool")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ProceduralGenerationToolPlayMode));
    }

    private void OnEnable()
    {
        skin = Resources.Load<GUISkin>("GUIStyles/EditorSkin");
        headerTexture = new Texture2D(1, 1);
        headerBackgroundColor = new Color(13f / 255f, 90f / 255f, 0f, 1f);
        managerSpawnedInHirearchy = false;
    }



    private void OnGUI()
    {
        Rect headerArea = new Rect(0, 0, Screen.width, 50);
        headerTexture.SetPixel(0, 0, headerBackgroundColor);
        headerTexture.Apply();
        GUI.DrawTexture(headerArea, headerTexture);
        

        GUILayout.Label("Procedural Generation Play Mode", skin.GetStyle("Header1"));
        GUILayout.Label("For use to create and set up the generation object (For Runtime Generation) \n", skin.GetStyle("Sub-Header1"));

        if (GameObject.FindAnyObjectByType<GenerationalValues>() != null) managerSpawnedInHirearchy = false;

        if (GUILayout.Button("Spawn Object"))
        {
            if (!managerSpawnedInHirearchy)
            {
                newObject = new GameObject();
                
                newObject.AddComponent<GenerationalValues>();
                newObject.AddComponent<TileMapLinker>();
                newObject.AddComponent<Pathfind_Astar>();
                newObject.AddComponent<PCG_Manager>();
                newObject.AddComponent<CellularAutomata>();
                newObject.AddComponent<StructureGeneration>();
                newObject.AddComponent<PerlinNoise>();
                newObject.name = "PCG_Map_Manager";
                newObject.tag = "PCG_Manager";

                managerSpawnedInHirearchy = true;
                
            }
            managerSpawnedInHirearchy = true;
           
        }
        if(managerSpawnedInHirearchy)
        {
            if (GUILayout.Button("Generate Map"))
            {
                //TO DO
            }
            if (GUILayout.Button("Smooth Map"))
            {
                //TO DO
            }
            if (GUILayout.Button("Enlarge Map"))
            {
                //TO DO
            }
            if (GUILayout.Button("Fill In Map"))
            {
                //TO DO
            }
        }
        

    }
}