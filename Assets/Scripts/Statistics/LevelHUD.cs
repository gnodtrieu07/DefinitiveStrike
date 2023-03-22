using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Statistics
{
    public class LevelHUD : MonoBehaviour
    {
        StatisticsBase statisticsBase;

        private void Awake() {
            statisticsBase = GameObject.FindWithTag("Player").GetComponent<StatisticsBase>();   
        }

        private void Update() {
            //String Format to clean the perc after .
            GetComponent<Text>().text = String.Format("{0:0}", statisticsBase.GetLevel());
        }
    }
}
