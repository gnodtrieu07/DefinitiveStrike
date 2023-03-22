using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class RemoveExcess : MonoBehaviour
    {
        [SerializeField] GameObject targetRemove = null;
        void Update()
        {
            //If effect no longer producing particles, so pretty simply just eliominate this
            if(!GetComponent<ParticleSystem>().IsAlive())
            {
                Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
