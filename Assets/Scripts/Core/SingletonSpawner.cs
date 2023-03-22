using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class SingletonSpawner : MonoBehaviour
    {
        [SerializeField] GameObject instantsigentObjectPrefab;

        //Global variables
        //When we put this boolean in a layer of class or in a future scene
        //or an expression of a new object constantly born will remember the value that it has been placed before
        static bool hasSpawner = false;

        private void Awake() {
            if(hasSpawner) 
            {
                //if already spawner then return nothing
                return;
            }

            SpawnInstantObject();

            //If not
            hasSpawner = true;
        }

        private void SpawnInstantObject()
        {
            GameObject instantsigentObject = Instantiate(instantsigentObjectPrefab);
            DontDestroyOnLoad(instantsigentObject);
        }
    }
}
