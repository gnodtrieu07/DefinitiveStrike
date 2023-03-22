using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class RentWeapon : MonoBehaviour
    {
        [SerializeField] Weapons weapons = null;

        private void OnTriggerEnter(Collider other) {
            if(other.gameObject.tag == "Player")
            {
                //If a player collides with weapons components, he will receive that weapon in his hand
                other.GetComponent<Fighter>().DeployWeapon(weapons);
                Destroy(gameObject); 
            }
        }
    }
}
