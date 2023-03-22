using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Control;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponsRentPick : MonoBehaviour, IRaycastable
    {
        [SerializeField] Weapons weapons = null;
        [SerializeField] float respawnTime = 5;
        [SerializeField] float healthToRestore = 0;
        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Player")
            {
                Pickup(other.gameObject);
            }
        }

        private void Pickup(GameObject subject)
        {
            if(weapons != null)
            {
                subject.GetComponent<Fighter>().DeployWeapon(weapons);
            }

            if(healthToRestore > 0)
            {
                subject.GetComponent<Health>().Heal(healthToRestore);
            }
            //Check if this is Player so equip the weapon 
            subject.GetComponent<Fighter>().DeployWeapon(weapons);
            //And after that start the disappear for 5 second
            StartCoroutine(DisappearForSec(respawnTime));
        }

        //Beacause the rent weapon is lost when we pickup and we can't pick up and equip again
        //So we create a Revival process, instead of destroy it will be return
        private IEnumerator DisappearForSec(float seconds)
        {
            //When we pickup the weapon, it will hile
            AppearWeaponPick(false);
            //Wait for 5 sec
            yield return new WaitForSeconds(seconds);
            //It will showing up again and can pickup
            AppearWeaponPick(true); 
        }
        private void AppearWeaponPick(bool shouldAppear)
        { 
            //Call this show pickup method which does two things, disables the collider and disables all the 
            GetComponent<Collider>().enabled = shouldAppear;
            //transform.GetChild(0).gameObject.SetActive(shouldAppear);
            //transform child in transform gets all the child transform of the current transform
            foreach(Transform child in transform)
            {
                child.gameObject.SetActive(shouldAppear); 
            }
        }

        public bool HandleRayCast(PlayerController callingController)
        {
            if(Input.GetMouseButton(0))
            {
                Pickup(callingController.gameObject);
            }
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }
    }
}
