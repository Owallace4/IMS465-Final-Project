using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float timeOffset;
    [SerializeField] Vector3 offsetPos;

    [SerializeField] Vector3 boundsMin;
    [SerializeField] Vector3 boundsMax; 

    private void LateUpdate()
    {
        if (player != null)
        {
            //Finds where we are and where to move 
            Vector3 startPos = transform.position;
            Vector3 targetPos = player.position;

            //Sets an offset 
            targetPos.x += offsetPos.x;
            targetPos.y += offsetPos.y;
            targetPos.z += transform.position.z;

            //Makes sure the target is within the bounds 
            targetPos.x = Mathf.Clamp(targetPos.x, boundsMin.x, boundsMax.x);
            targetPos.y = Mathf.Clamp(targetPos.y, boundsMin.y, boundsMax.y);

            //Finally moves the camera to where we want 
            float t = 1f - Mathf.Pow(1f - timeOffset, Time.deltaTime * 30);
            transform.position = Vector3.Lerp(startPos, targetPos, t); 
        }
    }
}
