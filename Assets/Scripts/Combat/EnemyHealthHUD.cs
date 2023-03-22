using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat
{
    public class EnemyHealthHUD : MonoBehaviour
    {
        Fighter fighter;

        private void Awake() {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();   
        }

        private void Update() {

            if(fighter.GetTarget() == null)
            {
                GetComponent<Text>().text = "/";
                return;
            }
            Health health = fighter.GetTarget();
            //String Format to clean the perc after .
            GetComponent<Text>().text = String.Format("{0:0}/{1:0}", health.GetHealthFul(), health.GetMaximHealthFul());
        }
    }
}
