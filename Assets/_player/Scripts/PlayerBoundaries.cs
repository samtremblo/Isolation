using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoundaries : MonoBehaviour
{

    private enum Zone
    {
        hallWay,
        room,
        door,
        none
    }

    private Renderer faceHidderRenderer;
    private Coroutine hideFace;


    private PlayerMovements playerMovements;
   
    private Zone nextZone = Zone.none;
    private Zone lastZone = Zone.none;
    [HideInInspector] public string currentZone= "";
    private bool isOutsideBounds = false;
    //private bool isChanging = false;

    private void Awake()
    {
        faceHidderRenderer = GameObject.FindWithTag("MainCamera").transform.GetChild(0).GetComponent<Renderer>();
        playerMovements = FindObjectOfType<PlayerMovements>();
        isOutsideBounds = false;
    }

    /*********************************************************************************/

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "HallWaysCollider")
        {
            if(isOutsideBounds && lastZone == Zone.hallWay )
            {
                if (hideFace != null) StopCoroutine(hideFace);
                hideFace = StartCoroutine(ShowVision());
            }
            else 
                nextZone = Zone.hallWay;
        }
        else if (other.name == "Roomcolliders")
        {
            if (isOutsideBounds && lastZone == Zone.room)
            {
                if (hideFace != null) StopCoroutine(hideFace);
                hideFace = StartCoroutine(ShowVision());
            }
            else 
                nextZone = Zone.room;
        }
        else if(other.name == "Doorwaycollider")
        {
            if (isOutsideBounds && lastZone == Zone.door)
            {
                if (hideFace != null) StopCoroutine(hideFace);
                hideFace = StartCoroutine(ShowVision());
            }
            else 
                nextZone = Zone.door;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "HallWaysCollider")
        {
            if (!isOutsideBounds) lastZone = Zone.hallWay;

            if (nextZone == Zone.none || nextZone == Zone.room)
            {
                if (hideFace != null) StopCoroutine(hideFace);
                hideFace = StartCoroutine(HideVision());

            }
            nextZone = Zone.none;

        }
        else if (other.name == "Roomcolliders")
        {
            if (!isOutsideBounds) lastZone = Zone.room;
            if (nextZone == Zone.none || nextZone == Zone.hallWay)
            {
                if (hideFace != null) StopCoroutine(hideFace);
                hideFace = StartCoroutine(HideVision());
            }
            
            nextZone = Zone.none;
        }
        else if (other.name == "Doorwaycollider")
        {
            if (!isOutsideBounds) lastZone = Zone.door;
            if (nextZone == Zone.none)
            {
                if (hideFace != null) StopCoroutine(hideFace);
                hideFace = StartCoroutine(HideVision());
            }
            else if (nextZone == Zone.room)
                currentZone = other.transform.GetChild(0).name;
            else if (nextZone == Zone.hallWay)
                currentZone = "none";         
            nextZone = Zone.none;
        }
    }
    
    /*********************************************************************************/

    private IEnumerator HideVision()
    {
        playerMovements.IsInWall = true;
        isOutsideBounds = true;
        faceHidderRenderer.enabled = true;
        float alpha = 0;
        while (alpha < 1)
        {
            alpha += 0.1f;
            faceHidderRenderer.material.SetColor("_Color", new Color(0, 0, 0, alpha));
            yield return null;
        }
        yield return null;
    }

    private IEnumerator ShowVision()
    {
        playerMovements.IsInWall = false;
        isOutsideBounds = false;
        float alpha = 1;
        while (alpha > 0)
        {
            alpha -= 0.1f;
            faceHidderRenderer.material.SetColor("_Color", new Color(0, 0, 0, alpha));
            yield return null;
        }       
        faceHidderRenderer.enabled = false;
        yield return null;
    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "HallWaysCollider" || other.name == "Roomcolliders" || other.name == "Doorwaycollider")
        {
            if (isOutsideBounds)
            {
                if (hideFace != null) StopCoroutine(hideFace);
                hideFace = StartCoroutine(ShowVision());
            }
            else
                isChanging = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "HallWaysCollider" || other.name == "Roomcolliders" || other.name == "Doorwaycollider")
        {
            if(isChanging == true)
            {
                isChanging = false;
            }
            else
            {
                if (hideFace != null) StopCoroutine(hideFace);
                hideFace = StartCoroutine(ShowVision());
            }
                
        }
    }
    */

    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Doorwaycollider" && boxListHall.Count > 0) inDoor = true;
        else if(other.name == "Doorwaycollider" && boxListRoom.Count > 0) inDoor = false;

        if (other.name == "HallWaysCollider")
        {
            boxListHall.Add(other);
            playerMovements.IsInWall = false;
            if (isOutsideBounds && !inDoor)
            {
                if (hideFace != null) StopCoroutine(hideFace);
                hideFace = StartCoroutine(Showvision());
            }
            faceHidderRenderer.enabled = false;

        }

        else if(other.name == "Roomcolliders")
        {
            boxListRoom.Add(other);
            playerMovements.IsInWall = false;
            if (isOutsideBounds && inDoor)
            {
                if (hideFace != null) StopCoroutine(hideFace);
                hideFace = StartCoroutine(Showvision());
            }
            faceHidderRenderer.enabled = false;
        }
    }
    */
    /*********************************************************************************/
    /*
    private void OnTriggerExit(Collider other)
    {

        if (other.name == "HallWaysCollider")
        {
            boxListHall.Remove(other);
            if (boxListHall.Count == 0 && !inDoor)
            {
                playerMovements.IsInWall = true;
                faceHidderRenderer.enabled = true;
                if (hideFace != null) StopCoroutine(hideFace);
                hideFace = StartCoroutine(HideVision());
            }
        }

        else if (other.name == "Roomcolliders")
        {
            boxListRoom.Remove(other);
            if (inDoor && boxListRoom.Count == 0)
            {
                playerMovements.IsInWall = true;
                faceHidderRenderer.enabled = true;
                if (hideFace != null) StopCoroutine(hideFace);
                hideFace = StartCoroutine(HideVision());
            }
            
        }
    }
    */
    /*********************************************************************************/



}
