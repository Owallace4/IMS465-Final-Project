using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityFunctions : MonoBehaviour
{
    // Uses a Bezier curve to simulate the movement of my flying enemies, found a video talking about using this method and it seemed cool so i used it. 
    // This translates the Bezier curve formula into C#

    public static Vector3 CalculateQuadraticBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        // return = (1 - t)2 P0 + 2(1 - t)tP1 + t2P2
        //             u              u         tt
        //            uu * P0 + 2 * u * t * P1 + tt * P2
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u; 
        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
        return p; 
    }
}
