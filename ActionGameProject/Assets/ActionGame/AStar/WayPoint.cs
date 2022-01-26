using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    public List<GameObject> neighbors;

    private void OnDrawGizmos()
    {
        if(neighbors != null && neighbors.Count > 0)
        {
            foreach(GameObject i in neighbors)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(this.transform.position, i.transform.position);
            }
        }
    }
}
