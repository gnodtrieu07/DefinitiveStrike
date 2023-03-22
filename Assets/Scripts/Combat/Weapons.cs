using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapons", menuName = "Weapons/Make New Weapon", order =0)]
    public class Weapons : ScriptableObject
    {   
        //Find and override weapon
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] Weapon WeaponsEquipPrefab = null;
        [SerializeField] float weaponDamage = 5f;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float percentageBonus = 0;
        //Set character default favor right hand
        [SerializeField] bool thatRightHandHold = true; 
        [SerializeField] Bullets bullets = null; 

        //Only using weapon with name
        const string weaponName = "Weapon";

        //Create the weapon
        public Weapon CreateW(Transform rightHandTrans, Transform leftHandTrans, Animator animator)
        {

            EliminateOldWeapon(rightHandTrans, leftHandTrans);

            Weapon weapon = null;
            if(WeaponsEquipPrefab != null)
            {
                Transform handTransform = HandsAction(rightHandTrans, leftHandTrans);
                //If prefab of weapon differ null so it start to create the weapont quickly
                //And if the character does not have any weapons added, it will not try to create prefab for any equipment
                weapon = Instantiate(WeaponsEquipPrefab, handTransform);
                weapon.gameObject.name = weaponName;
            }
            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if ( animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
            else if(overrideController != null)
            { 
                 animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            return weapon;
        }

        private void EliminateOldWeapon(Transform righHandTrans, Transform leftHandTrans)
        {
            Transform oldWeapon = righHandTrans.Find(weaponName);
            if(oldWeapon == null)
            {
                //If don't have weapon in right hand so it find in left hand
                oldWeapon = leftHandTrans.Find(weaponName);
            }
            if(oldWeapon == null)
            {
                //If all hand don't have so return
                return;
            }

            oldWeapon.name = "DESTROYING";

            Destroy(oldWeapon.gameObject);
        }

        private Transform HandsAction(Transform rightHandTrans, Transform leftHandTrans)
        {
            Transform handTransform;
            //If the character favor right hand, so it's use right hand
            if (thatRightHandHold)
            {
                handTransform = rightHandTrans;
            }
            //Otherwise must use the left hand
            else
            {
                handTransform = leftHandTrans;
            }

            return handTransform;
        }

        public float GainedDamage()
        {
            return weaponDamage;
        }

        public float GetPercentageBonus()
        {
            return percentageBonus;
        }

        public float GainedRange()
        {
            return weaponRange;
        }
        //This function help know we equipped  bullets or not
        public bool GainedBulletsWay()
        {
            return bullets != null;
        }
        //Ready to fire the bullet
        public void FireBulletsWay(Transform rightHandTrans, Transform leftHandTrans, Health target, GameObject instigator, float figureDamage)
        {
            //Provide right hand information with the left hand and vice versa
            Bullets bulletsCaseOut = Instantiate(bullets, HandsAction(rightHandTrans, leftHandTrans).position, Quaternion.identity);
            bulletsCaseOut.SetTarget(target, instigator , figureDamage); 
        }
    }
}
