using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using RPG.Statistics;
using RPG.Core;
using System;
using RPG.Utils;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float RegenerateHealPercentage = 70;
        [SerializeField] TakeDamageeEvent takeDamage;
        [SerializeField] UnityEvent onDie;

        [System.Serializable]
        public class TakeDamageeEvent : UnityEvent<float>
        {

        }
        LazyValue<float> healthFul;
        //Default dead is false
        //Because when start game is not dead
        bool Deaded = false;
        
        private void Awake() {
            healthFul = new LazyValue<float>(GetInitialHealth);
        }
        
        private float GetInitialHealth()
        {
            return GetComponent<StatisticsBase>().GetStatcsExpand(StatcExpand.Health);
        }

        private void Start() {
            //Ready to update level and gain more health
            
            // if(healthFul < 0){
            //     //From StaticsBase
            //     //So base stat-tics wil be getting the health, it will be getting it from its evolution that it's linked up to 
            //     healthFul = GetComponent<StatisticsBase>().GetStatcsExpand(StatcExpand.Health);
            // }

            healthFul.ForceInit();
             
        }

        private void OnEnable() {
            GetComponent<StatisticsBase>().onLevelUp += RegenerateHealth;
        }

        private void OnDisable() {
            GetComponent<StatisticsBase>().onLevelUp -= RegenerateHealth;
        }

        //If target is dead, so stopped attacking instanly
        public bool Decedent()
        {
            return Deaded;
        }

        public void TakeDamagee(GameObject instigator,float damage)
        {
            print(gameObject.name + " take the damage: " + damage);
            //Call TakeDamagee in Start()
            //And even if Start hadn't been called in health yet, it would make sure that we had got the correct 
            //default value for health point and everything would work as we expected
            healthFul.value = Mathf.Max(healthFul.value - damage, 0);
            print(healthFul.value);
            //When health = 0, active the Dead function
            if(healthFul.value == 0)
            {
                onDie.Invoke();
                //Method Ectracted
                Dead();
                ReceiveExperience(instigator);
            }
            else
            {
                takeDamage.Invoke(damage);
            }
        }

        
        public void Heal(float healthToRestore)
        {
             healthFul.value = Mathf.Min(healthFul.value + healthToRestore, GetMaximHealthFul());
        }

        public float GetHealthFul()
        {
            return healthFul.value;
        }

        public float GetMaximHealthFul()
        {
            return GetComponent<StatisticsBase>().GetStatcsExpand(StatcExpand.Health);
        }


        private void ReceiveExperience(GameObject instigator)
        {
            //Storing the experience
            Experiences experiences = instigator.GetComponent<Experiences>();
            if(experiences == null)
            {
                return;
            }
            experiences.GainExperience(GetComponent<StatisticsBase>().GetStatcsExpand(StatcExpand.Health));

        }

        //Convert to percentage of health to print on Display HUD
        public float GetPercentage()
        {
            return 100 * GetFraction();
        }

        public float GetFraction()
        {
            return healthFul.value  / GetComponent<StatisticsBase>().GetStatcsExpand(StatcExpand.Health);
        }

        private void Dead()
        {
            //Check whether the character is dead or alive
            //If is dead so cannot do anything more
            if(Deaded)
            {
                return;
            }
            //If the dead is reality
            Deaded = true;
            //So active animation dead
            GetComponent<Animator>().SetTrigger("dead");
            //Cancel current action so that anything is running now know that it should have stopped running
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void RegenerateHealth()
        {
            float regenHealthFul = GetComponent<StatisticsBase>().GetStatcsExpand(StatcExpand.Health) * (RegenerateHealPercentage / 100);
            healthFul.value  = Mathf.Max(healthFul.value, regenHealthFul);
        }

        //Called form interface to save the health at that time
        public object CaptureState()
        {
            return healthFul.value;
        }

        //Called from interface to load the health before
        public void RestoreState(object state)
        {
            healthFul.value = (float) state;
            if(healthFul.value == 0)
            {
                //Method Ectracted
                Dead();
            }
        }
    }
}
