using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using Pathfinding;
using DG.Tweening;

public class TurnManager : MonoBehaviour
{
    public bool wasRunning;
    public bool reachUpdated;
    public bool selectedNotDestroyed;
    public bool alreadyPlaced;

    public int distance = 5;
    public Vector3 selectedTilePos = new Vector3(0,0,0);

    public GameObject player;
    public GameObject selectedTile;
    public GameObject selectedTileRed;
    public GameObject secondPath;
    public GameObject reachableTile;

    public SpriteRenderer playerTileSprite;


    public Camera cam;
    
    public AILerp pathRunning;
    public AILerp pathStanding;
       
    private GraphNode startingNode;
    public List<GraphNode> reachableNodes = new List<GraphNode>();  
    public List<GameObject> placedNodes = new List<GameObject>();  


    public int moveCount;

    void Start()
    {
        Invoke("BuildBorder", 1);
    }

    void BuildBorder()
    {
        AstarPath.active.Scan();
        ShadowCaster2DFromComposite.RebuildAll();
    }

    IEnumerator updateReach()
    {
        startingNode = AstarPath.active.GetNearest(player.transform.position).node;
        //reachableNodes = PathUtilities.GetReachableNodes(startingNode);
        reachableNodes = PathUtilities.BFS(startingNode,distance);
    
        for(int i = 0;i<reachableNodes.Count;i++)
        {
            Int3 nodePosRaw = reachableNodes[i].position;
            Vector3 nodePos;
            nodePos = (Vector3)nodePosRaw;
            float distance = Vector3.Distance(nodePos, player.transform.position);

            placedNodes.Add(Instantiate(reachableTile ,nodePos , Quaternion.identity));
            yield return new WaitForSeconds(0.01f);
            
            
        }
    }
    
    void clearReach()
    {
        reachableNodes.Clear();
        for(int i = 0;i<placedNodes.Count;i++)
        {
            Destroy(placedNodes[i]);
        }
    }
    void Update()
    {
        //var linePos = new List<Vector3>();
        //pathRunning.GetRemainingPath(linePos, out bool staleRunning);
        //player.GetComponent<LineRenderer>().positionCount = linePos.Count;
        //player.GetComponent<LineRenderer>().SetPositions(linePos.ToArray());
        
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        GraphNode selectedNode = AstarPath.active.GetNearest(mousePos).node;
        Vector3 selectedNodePos = (Vector3)selectedNode.position;
        
        //The array of positions to retrieve. The array passed should be of at least positionCount in size.
        //Vector3 direction = ailerp.velocity;          
        
        if(pathRunning.velocity == new Vector3(0,0,0))
        {
            if(reachUpdated==false)
            {
                
                reachUpdated=true;
                wasRunning=false;
                StartCoroutine(updateReach());
                
            }
            if(secondPath.GetComponent<LineRenderer>().positionCount < 7 )
            {
                secondPath.GetComponent<LineRenderer>().startColor=Color.green;
                secondPath.GetComponent<LineRenderer>().endColor=Color.green;
                playerTileSprite.color=Color.green;
                selectedTile.transform.position=secondPath.GetComponent<LineRenderer>().GetPosition(secondPath.GetComponent<LineRenderer>().positionCount-1);
                selectedTileRed.SetActive(false);
                selectedTile.SetActive(true);
                
            }
            else
            {
                secondPath.GetComponent<LineRenderer>().startColor=Color.red;
                secondPath.GetComponent<LineRenderer>().endColor=Color.red;
                playerTileSprite.color=Color.red;
                selectedTileRed.transform.position=secondPath.GetComponent<LineRenderer>().GetPosition(secondPath.GetComponent<LineRenderer>().positionCount-1);
                selectedTile.SetActive(false);
                selectedTileRed.SetActive(true);
            }
            var buffer = new List<Vector3>();
            pathStanding.GetRemainingPath(buffer, out bool staleStanding);
            secondPath.GetComponent<LineRenderer>().positionCount = buffer.Count;
            secondPath.GetComponent<LineRenderer>().SetPositions(buffer.ToArray());
           
            selectedTile.transform.position=secondPath.GetComponent<LineRenderer>().GetPosition(secondPath.GetComponent<LineRenderer>().positionCount-1);

        }
        else
        {
            if(reachUpdated==true)
            {
                clearReach();
                reachUpdated=false;
                wasRunning=true;
            }

            pathStanding.Teleport(player.transform.position);
            var buffer = new List<Vector3>();
            pathStanding.GetRemainingPath(buffer, out bool staleStanding);
            secondPath.GetComponent<LineRenderer>().positionCount = buffer.Count;
            secondPath.GetComponent<LineRenderer>().SetPositions(buffer.ToArray());
        }
        
    }
}
