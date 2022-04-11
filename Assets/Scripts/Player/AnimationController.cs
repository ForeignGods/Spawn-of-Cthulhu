using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AnimationController : MonoBehaviour
{
    public AILerp ailerp;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
            // var buffer = new List<Vector3>();
			// ailerp.GetRemainingPath(buffer, out bool stale);
			// GetComponent<LineRenderer>().positionCount = buffer.Count;
			// GetComponent<LineRenderer>().SetPositions(buffer.ToArray());
            Vector3 direction = ailerp.velocity;  
            Debug.Log(direction);
			//Debug.Log(direction);
			if(direction.x < 0)
			{
				
				if(direction.y < 0)
				{
					//Debug.Log("linksUnten");
					GetComponent<Transform>().GetChild(0).gameObject.GetComponent<Animator>().SetFloat("dirStatus", 4f);
					//linksUnten
				}
				else if(direction.y > 0)
				{
					//Debug.Log("linksOben");
					GetComponent<Transform>().GetChild(0).gameObject.GetComponent<Animator>().SetFloat("dirStatus", 2f);
					//linksOben
				}
				else if(direction.y==0)
				{
					//Debug.Log("links");
					GetComponent<Transform>().GetChild(0).gameObject.GetComponent<Animator>().SetFloat("dirStatus", 3f);
				}
			}
			else if((direction.x > 0))
			{
				
				if(direction.y < 0)
				{
					//Debug.Log("rechtsUnten");
					GetComponent<Transform>().GetChild(0).gameObject.GetComponent<Animator>().SetFloat("dirStatus", 6f);
					//rechtsUnten
				}
				else if(direction.y > 0)
				{
					//Debug.Log("rechtsOben");
					GetComponent<Transform>().GetChild(0).gameObject.GetComponent<Animator>().SetFloat("dirStatus", 8f);
					//rechtsOben
				}
				else if(direction.y==0)
				{
					//Debug.Log("rechts");
					GetComponent<Transform>().GetChild(0).gameObject.GetComponent<Animator>().SetFloat("dirStatus", 7f);
				}
			}
			else if(direction.y < 0)
			{
				
				//Debug.Log("Unten");
				GetComponent<Transform>().GetChild(0).gameObject.GetComponent<Animator>().SetFloat("dirStatus", 5f);
			}
			else if(direction.y > 0)
			{
				
				//Debug.Log("oben");
				GetComponent<Transform>().GetChild(0).gameObject.GetComponent<Animator>().SetFloat("dirStatus", 1f);
			}
			// else
			// {
			// 	GetComponent<Transform>().GetChild(0).gameObject.GetComponent<Animator>().SetFloat("dirStatus", 0f);
			// 	//Debug.Log("standing");
			// }
			
			if(direction==new Vector3(0,0,0))
			{
				GetComponent<Transform>().GetChild(0).gameObject.GetComponent<Animator>().SetBool("isMoving", false);
			}
			else
			{
				GetComponent<Transform>().GetChild(0).gameObject.GetComponent<Animator>().SetBool("isMoving", true);
			}
    }
}
