using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UserInterface.DamageText
{
    public class DamageText : MonoBehaviour
    {

        [SerializeField] Text damageText = null;
        public void DesstroyText()
        {
            Destroy(gameObject);
        }

        public void SetValue(float amount)
        {
             damageText.text = String.Format("{0:0}", amount);
        }

    }
}

