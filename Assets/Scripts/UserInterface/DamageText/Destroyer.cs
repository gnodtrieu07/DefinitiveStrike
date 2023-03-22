using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UserInterface.DamageText
{
    public class Destroyer : MonoBehaviour
    {
        [SerializeField] GameObject targetToDestroy = null;
        public void DesstroyTarget()
        {
            Destroy(targetToDestroy);
        }
    }
}
