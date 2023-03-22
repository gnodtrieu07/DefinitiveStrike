using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class WatchWay : MonoBehaviour
    {
        // Start is called before the first frame update
        private void OnDrawGizmos()
        {
            const float gizmosWaypointRadius = 0.5f;

            //This loop for will go from 0 to childCount - 1 (waypoint2,waypoint1,waypoint0)
            for(int i = 0; i < transform.childCount; i++)
            {
                int j = ConnectNextIndex(i);
                Gizmos.DrawSphere(GetPositionWPoint(i), gizmosWaypointRadius);
                //Draw straight line from point A to point B
                Gizmos.DrawLine(GetPositionWPoint(i), GetPositionWPoint(j));
            }
        }

        public int ConnectNextIndex(int i)
        {
            //The first Waypoint is parent
            //If there have many point child and it repeat the loop exactly
            if(i + 1 == transform.childCount)
            {
                return 0;
            }
            return i + 1;
        }

        public Vector3 GetPositionWPoint(int i)
        {
            return transform.GetChild(i).position;
        }
    }

}