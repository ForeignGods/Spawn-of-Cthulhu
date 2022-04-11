using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using Pathfinding;
public class TileAutomata : MonoBehaviour {


    [Range(0,100)]
    public int iniChance;
    [Range(1,8)]
    public int birthLimit;
    [Range(1,8)]
    public int deathLimit;

    [Range(1,10)]
    public int numR;
    private int count = 0;



    private int[,] terrainMap;
    public Vector3Int tmpSize;
    public Tilemap topMap;
    public Tilemap botMap;
    public Tilemap wallMap;
    public Tilemap carpetMap;
    public Tilemap windowMap;
    public Tilemap windowCoverMap;
    public Tilemap fluidMap;
    public Tilemap toxicSmokeMap;
    
    public WallTile wallTile;
    public FloorTile topTile;
    public AnimatedTile botTile;
    public AnimatedTile fluidTile;
    public AnimatedTile toxicSmokeTile;
    public CarpetTile carpetTile;
    public CarpetVariationTile carpetVariationTile;
    public WindowTile windowTile;
    public WindowCoverTile windowCoverTile;

    public GameObject player;

    int width;
    int height;

    //PathUtilities:
    //https://arongranberg.com/astar/docs/class_pathfinding_1_1_path_utilities.php#a78c136feba9843851f499b21c86c1731
    //GraphNode:
    //https://arongranberg.com/astar/documentation/4_2_7_0b5deb87/graphnode.html    
    private GraphNode startingNode;
    public List<GraphNode> reachableNodes = new List<GraphNode>();   
    public List<GameObject> layers = new List<GameObject>();
    


    public void doSim(int nu)
    {
        clearMap(false);
        width = tmpSize.x;
        height = tmpSize.y;

        if (terrainMap==null)
        {
            terrainMap = new int[width, height];
            initPos();
        }


        for (int i = 0; i < nu; i++)
        {
            terrainMap = genTilePos(terrainMap);
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (terrainMap[x, y] == 1)
                {                
                    //Variation für walkableTiles(carpetTiles)
                    // int carpetDecider = Random.Range(0, 100);
                    // if(carpetDecider < 50 )
                    // {
                        // Debug.Log("normal"+carpetDecider);
                        //carpetMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), carpetTile);  
                        
                    // }
                    // else
                    // {
                    //     Debug.Log("variation"+carpetDecider);
                    //     carpetMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), carpetVariationTile); 
                    // }
                    
                    carpetMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), carpetTile);  
    
                    topMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), topTile);   
                    wallMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), wallTile);   
                    windowMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), windowTile);  
                    windowCoverMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), windowCoverTile);  
                }
            }
        }

        for (int x = 0; x < width*2; x++)
        {
            for (int y = 0; y < height*2; y++)
            {
                botMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), botTile);
                fluidMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), fluidTile);
                float r = Random.Range(1, 10.0f);
                if(r < 3)
                {
                    toxicSmokeMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), toxicSmokeTile);
                }
            }
        }
    }

    public void initPos()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                terrainMap[x, y] = Random.Range(1, 101) < iniChance ? 1 : 0;
            }

        }

    }

    public int[,] genTilePos(int[,] oldMap)
    {
        int[,] newMap = new int[width,height];
        int neighb;
        BoundsInt myB = new BoundsInt(-1, -1, 0, 3, 3, 1);


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                neighb = 0;
                foreach (var b in myB.allPositionsWithin)
                {
                    if (b.x == 0 && b.y == 0) continue;
                    if (x+b.x >= 0 && x+b.x < width && y+b.y >= 0 && y+b.y < height)
                    {
                        neighb += oldMap[x + b.x, y + b.y];
                    }
                    else
                    {
                        neighb++;
                    }
                }

                if (oldMap[x,y] == 1)
                {
                    if (neighb < deathLimit) newMap[x, y] = 0;

                        else
                        {
                            newMap[x, y] = 1;

                        }
                }

                if (oldMap[x,y] == 0)
                {
                    if (neighb > birthLimit) newMap[x, y] = 1;
                    else
                    {
                        newMap[x, y] = 0;
                    }
                }
            }
        }
        return newMap;
    }

    void Start()
    {
        doSim(numR);
        //ShadowCaster2DFromComposite.RebuildAll();
        // This holds all graph data
    }
	
    public int clicked=0;
    
    void Update () 
    {

        if (Input.GetMouseButtonDown(2))
        {
            doSim(numR);
        }


        if (Input.GetMouseButtonDown(1))
        {
            
    
            layers[clicked].SetActive(true);
            clicked++;
                
        }
            
        // if (Input.GetMouseButtonDown(0))
        //     {
        //         if(onlyOnce==0)
        //         {
        //             AstarPath.active.Scan();
        //             ShadowCaster2DFromComposite.RebuildAll();
        //             startingNode = AstarPath.active.GetNearest(player.transform.position).node;
        //             //reachableNodes = PathUtilities.GetReachableNodes(startingNode);
        //             reachableNodes = PathUtilities.BFS(startingNode,10);
        //             Debug.Log(reachableNodes.Count);
        //             onlyOnce++;
        //             for(int i = 0;i<reachableNodes.Count;i++)
        //             {
        //                 Int3 nodePosRaw = reachableNodes[i].position;
        //                 Vector3 nodePos;
        //                 nodePos = (Vector3)nodePosRaw;
        //                 float distance = Vector3.Distance(nodePos, player.transform.position);
        //                 if(distance < 2)
        //                 {
        //                     Instantiate(circle ,nodePos , Quaternion.identity);
        //                 }             
        //             }
        //         }
        //     }
    }

    public void SaveAssetMap()
    {
        string saveName = "tmapXY_" + count;
        var mf = GameObject.Find("Grid");

        if (mf)
        {
            var savePath = "Assets/" + saveName + ".prefab";
            if (PrefabUtility.CreatePrefab(savePath,mf))
            {
                EditorUtility.DisplayDialog("Tilemap saved", "Your Tilemap was saved under" + savePath, "Continue");
            }
            else
            {
                EditorUtility.DisplayDialog("Tilemap NOT saved", "An ERROR occured while trying to saveTilemap under" + savePath, "Continue");
            }
        }
    }

    public void clearMap(bool complete)
    {
        topMap.ClearAllTiles();
        botMap.ClearAllTiles();
        if (complete)
        {
            terrainMap = null;
        }
    }

}
