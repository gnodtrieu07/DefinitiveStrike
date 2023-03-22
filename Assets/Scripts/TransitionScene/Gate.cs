using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using RPG.Control;

namespace RPG.TransitionScene
{
    public class Gate : MonoBehaviour
    {

        enum DestinationIdentifier
        {
            A,B,C,D,E
        }
        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform deployPoint;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] float EffectExportTime = 1f;
        [SerializeField] float EffectImportTime = 2f;
        [SerializeField] float EffectWaitingTime = 0.5f;
        private void OnTriggerEnter(Collider other) {
            print("Gate triggered");   
            if(other.tag == "Player")
            {   
                StartCoroutine(TransitionOld());
            }
        }

        private IEnumerator TransitionOld()
        {
            if(sceneToLoad <0)
            {
                Debug.LogError("Loading error");
                yield break;
            }

            DontDestroyOnLoad(gameObject);

            //Call all its public methods in EffectTransition
            //It is EffImport and EffExport
            //It fades on the screen
            EffectTransition efftrans = FindObjectOfType<EffectTransition>(); 
            //Then load through a series of frames
            yield return efftrans.EffectExport(EffectExportTime);           
            //Save current level
            SavingSetup savepoint = FindObjectOfType<SavingSetup>();
            PlayerController playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            //Remove control
            playerController.enabled = false; 
            savepoint.Save();
            //Update the player and wait for a series of next frames
            yield return  SceneManager.LoadSceneAsync(sceneToLoad);   
            PlayerController newPlayerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            //Remove control
            newPlayerController.enabled = false; 

            //Load current level
            savepoint.Load();
            print("Scene Loaded");
            Gate otherGate = GetOtherGate();
            UpdatePlayer(otherGate);
            //After update, in this time we already loaded and move to Scene2 
            savepoint.Save();
            //Has already moved the player and waiting after that for the camera to stabilize
            yield return new WaitForSeconds(EffectWaitingTime);
            efftrans.EffectImport(EffectImportTime);
            
            newPlayerController.enabled = false; 
            //Restore control
            Destroy(gameObject);
        }

        private void UpdatePlayer(Gate otherGate)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().enabled = false;
            player.GetComponent<NavMeshAgent>().Warp(otherGate.deployPoint.position);
            player.transform.rotation = otherGate.deployPoint.rotation;
            player.GetComponent<NavMeshAgent>().enabled = true;
        }

        private Gate GetOtherGate()
        {
            //Each loop will return the list of gates we can do through
            foreach(Gate gate in FindObjectsOfType<Gate>())
            {
                if (gate == this) 
                {
                    continue;
                }
                if (gate.destination != destination)
                {
                    continue;
                }
                //Return to the current gate is being checked
                return gate;
            }

            return null;
        }
    }
}
 