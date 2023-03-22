using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using RPG.Attributes;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Bullets : MonoBehaviour
    {
        [SerializeField] float pace = 1;
        [SerializeField] bool inComing = true;
        [SerializeField] GameObject impactEffect = null;
        [SerializeField] float climaxlifeTime = 10;
        [SerializeField] GameObject[] eliminateByDamage = null;
        [SerializeField] float alifeAfterwardImpact = 2;
        [SerializeField] UnityEvent onHit;
        GameObject instigator; 
        Health target = null;
        float damage = 0;

        private void Start() {
            transform.LookAt(DestBulletTouch());
        }
        public void Update() {
            if(target == null)
            {
                return;
            }

            //incoming = true so we going to look at the target  
            //And if target is dead so bulllet don't look them
            if(inComing && !target.Decedent())
            {
                //Consider the target position
                transform.LookAt(DestBulletTouch());
            }
            //This per frame by speed and then multiply it per frame by time
            transform.Translate(Vector3.forward * pace * Time.deltaTime);
        }
        
        //We create a determine target 
        public void SetTarget(Health target, GameObject instigator ,float damage)
        {
            //And it looking for the target with the Health
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;

            //Destroy projectile after it finished
            Destroy(gameObject,climaxlifeTime);
        }

        private Vector3 DestBulletTouch()
        {
            //The destination will collide with the capsule's mesh
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if(targetCapsule == null)
            {
                //If target not have capsule then it return to the present position of target
                return target.transform.position;
            }
            //Return the position of target and hit the middle of capsule (Vector3.up + multiply this by the target capsule height divided)
            //Vector3 it on the Y axis
            return target.transform.position + Vector3.up * targetCapsule.height / 2;
        }

        private void OnTriggerEnter(Collider other) {
            //Bullet contact with target
            if(other.GetComponent<Health>() != target)
            {
                return;
            }
            //If target is dead so don't try to be giving any damage
            if(target.Decedent())
            {
                return;
            }
            target.TakeDamagee(instigator,damage);

            pace = 0;
            onHit.Invoke();

            if(impactEffect != null)
            {
                Instantiate(impactEffect, DestBulletTouch(),transform.rotation);
            }

            foreach(GameObject toEliminate in eliminateByDamage)
            {
                Destroy(toEliminate);
            }

            Destroy(gameObject, alifeAfterwardImpact);
        }
    }
}
