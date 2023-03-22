using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Cinema
{
    public class FakePlayableDirection : MonoBehaviour
    {
        public event Action<float> onFinish;

        private void Start() {
            Invoke("OnFinish", 3f);
        }

        public void OnFinish()
        {
            onFinish(5.1f);
        }
    }

}