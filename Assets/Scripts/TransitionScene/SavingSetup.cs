using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RPG.Saving;

namespace RPG.TransitionScene
{
    public class SavingSetup : MonoBehaviour
    {
        const String defaultSaveFile = "save";
        [SerializeField] float EffImportTime = 0.7f;

        private void Awake() {
            StartCoroutine(LoadLastScene());
        }
        private IEnumerator LoadLastScene()
        {
            //When start after endgame, the scene auto transit to the last screen(or mission) you save
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
            EffectTransition efftrans = FindObjectOfType<EffectTransition>();
            //It only happen when start at first and then it instanly fade out
            efftrans.EffectImmediately(); 
            yield return efftrans.EffectImport(EffImportTime);
        }

        public void Update() {
            if(Input.GetKeyDown(KeyCode.L))
            {
                Load();
                print("Active L");
            }
            if(Input.GetKeyDown(KeyCode.S))
            {
                Save();
                print("Active S");
            }
            if(Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }
        }

        public void Load()
        {
            //Call to saving system load
            GetComponent<SavingSystem>().Load(defaultSaveFile);
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }

        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(defaultSaveFile);
        }
    }
}
