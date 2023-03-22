using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class FollowingCamera : MonoBehaviour
    {
 
        [SerializeField] Transform target;

        void LateUpdate()
        {
            transform.position = target.position;    
        }
    }

}