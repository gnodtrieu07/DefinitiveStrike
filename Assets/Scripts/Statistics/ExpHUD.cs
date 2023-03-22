using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Statistics
{
    public class ExpHUD : MonoBehaviour
    {
        Experiences experiences;

        private void Awake() {
            experiences = GameObject.FindWithTag("Player").GetComponent<Experiences>();   
        }

        private void Update() {
            //String Format to clean the perc after .
            GetComponent<Text>().text = String.Format("{0:0}", experiences.GetPoint());
        }
    }
}
