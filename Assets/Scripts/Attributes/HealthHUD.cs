using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class HealthHUD : MonoBehaviour
    {
        Health health;

        private void Awake() {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();   
        }

        private void Update() {
            //String Format to clean the perc after .
            GetComponent<Text>().text = String.Format("{0:0}/{1:0}", health.GetHealthFul(), health.GetMaximHealthFul());
        }
    }
}
