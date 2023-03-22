using UnityEngine;
using RPG.Movement;
using RPG.Control;
using RPG.Combat;
using RPG.Core;
using System;
using RPG.Statistics;
using RPG.Saving;
using RPG.Attributes;
using System.Collections.Generic;
using RPG.Utils;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifier
    {
       // [SerializeField] float weaponRange = 2f;
        [SerializeField] float timeBetweenAttacks = 1f;
       // [SerializeField] float weaponDamage = 5f;
       // [SerializeField] GameObject weaponsPrefab = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] Transform rightHandTransform = null;
       // [SerializeField] AnimatorOverrideController weaponsOverride = null;
        [SerializeField] Weapons defaultWeapon = null;
        //The resources folder name and weapon name must be unique
        //Because if have another folder or weapon asset have same name so it doesn't work
        //[SerializeField] string defaultWeaponName = "NOWeapon";
        Health target;
        //Thre present weapon is the weapon that character gained or pickup
        Weapons presentWeaponsConfig;
        LazyValue<Weapon> presentWeapon; 
        float timeSinceLastAttack = Mathf.Infinity;

        private void Awake() {
            presentWeaponsConfig = defaultWeapon;
            presentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        private Weapon SetupDefaultWeapon()
        {   
            return AttachWeapon(defaultWeapon);
        }

        private void Start() {
            //Use default weapon name to load the resources of that weapon 
            //And it will find the prefab to look at object in script and found it in Resources folder
            //Weapons weapons = Resources.Load<Weapons>(defaultWeaponName);
                // if(presentWeapon == null)
                // {
                //     DeployWeapon(defaultWeapon);
                // }
            //Start game sometimes with default weapon (automatic non-weapon)
            //DeployWeapon(defaultWeapon);
            //AttachWeapon(presentWeaponsConfig);
            presentWeapon.ForceInit();
        }

        private void Update()
        {
            
            timeSinceLastAttack += Time.deltaTime;

            if (target == null)  return;
            if (target.Decedent()) return;
            //Check the target in range of view to move or go attack the enemy
            if (!GetIsInRange(target.transform))
            {
                GetComponent<Mover>().MoveTo(target.transform.position, 1f);
            } 
            else
            {
                GetComponent<Mover>().Cancel();
                //Method Ectracted
                AttackSmashBehaviour(); 
                Hit();
            }
        }

        public void DeployWeapon(Weapons weapons)
        {
            if (weapons == null)
            {
                return;
            }
            presentWeaponsConfig = weapons;
            presentWeapon.value = AttachWeapon(weapons);
        }

        private Weapon AttachWeapon(Weapons weapons)
        {
            // Instantiate(weaponsPrefab, handTransform);
            Animator animator = GetComponent<Animator>();
            //Override the weapons 
            //animator.runtimeAnimatorController = weaponsOverride;
            return weapons.CreateW(rightHandTransform, leftHandTransform, animator);
        }

        public Health GetTarget()
        {
            return target;
        }

        private void AttackSmashBehaviour()
        {
            transform.LookAt(target.transform);
            if(timeSinceLastAttack > timeBetweenAttacks)
            {
                //This will trigger the hit event
                //Method Ectracted
                FunctionSetTriggerAttack(); //the function prevent bugs game
                timeSinceLastAttack = 0;
            }
            //GetComponent<Animator>().SetTrigger("attackSmash");
        }

        private void FunctionSetTriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("pauseAttk");//reset from attack to pause when character no need to attack
            GetComponent<Animator>().SetTrigger("attack"); 
        }

        //Animation-finished event
        void Hit()
        {
                //Make sure there have target to Hit
                if(target == null)
                {
                    //Preventing a null reference exception if the target is not found
                    return;
                }
                float damage = GetComponent<StatisticsBase>().GetStatcsExpand(StatcExpand.Damage);

                if(presentWeapon.value != null)
                {
                    presentWeapon.value.OnHit();
                }
                //If the current weapon has a bullets, then fire the bullets and passing the information that it needs, exactly that target
                if(presentWeaponsConfig.GainedBulletsWay())
                {
                    presentWeaponsConfig.FireBulletsWay(rightHandTransform, leftHandTransform, target, gameObject, damage);
                }
                //Otherwise, if not have bullets way, then just call, get damage and do thing when have weapon
                else
                {
                //The target has to receive damage
                // target.TakeDamagee(gameObject,presentWeapon.GainedDamage());
                    target.TakeDamagee(gameObject, damage);
                }

        }

        void FireTheShoot()
        {
            Hit();
        }

        public bool GetIsInRange(Transform targetTransform)
        {
            //check-in-range
            return Vector3.Distance(transform.position, targetTransform.position) < presentWeaponsConfig.GainedRange();
        }

        public bool CaseCanAttacking(GameObject combatTarget)
        {
            if(combatTarget == null)
            {
                return false;
            }

            if(!GetComponent<Mover>().CanMoveTo(combatTarget.transform.position) && !GetIsInRange(combatTarget.transform ))
            {
                //Cannot move to the targaet again so return false that cannot attack the target
                return false;
            }

            Health CheckingToTarget = combatTarget.GetComponent<Health>();

            return CheckingToTarget != null && !CheckingToTarget.Decedent();
        }

        //determine target
        public void Attack(GameObject combatTarget)
         {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            //Set pause when character in not attack
            //Method Ectracted
            FunctionSetTriggerStoppedAttack();
            target = null;
        }

        private void FunctionSetTriggerStoppedAttack()
        {
            GetComponent<Animator>().SetTrigger("pauseAttk");
            GetComponent<Animator>().ResetTrigger("attack");
        }

        
        public IEnumerable<float> GetAdditiveModifier(StatcExpand statcExpand)
        {
            if(statcExpand == StatcExpand.Damage)
            {
                yield return presentWeaponsConfig.GainedDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifier(StatcExpand statcExpand)
        {
            if(statcExpand == StatcExpand.Damage)
            {
                yield return presentWeaponsConfig.GetPercentageBonus();
            }
        }


        //Implement form ISaveable interface Saving System
        //The gun object saved when we load the game or come to next mission 
        public object CaptureState()
        {
            return presentWeaponsConfig.name;
        }

        public void RestoreState(object state)
        {
            //Save the string for the name with weaponName
            string weaponName = (string)state;           
            //With the name of weapon so it load form the Resources folder
            Weapons weapons = UnityEngine.Resources.Load<Weapons>(weaponName);
            //To give a weapon
            DeployWeapon(weapons);
        }
    }
}