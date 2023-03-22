using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.TransitionScene
{
    public class EffectTransition : MonoBehaviour
    {
        CanvasGroup canvasGroup;
        Coroutine currentActiveEffectExport = null;

        private void Awake() {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void EffectImmediately()
        {
            canvasGroup.alpha = 1;
        }


        // IEnumerator EffectGeneral()
        // {
        //     yield return EffectExport(3f);
        //     print("Effect out");
        //     yield return EffectImport(1f);
        //     print("Effect in");
        // }

        public Coroutine EffectExport(float time)
        {
            return Effect(1, time);
            // while (canvasGroup.alpha != 1) //with alpha is not 1
            // {
            //     canvasGroup.alpha += Time.deltaTime/time; //Allow increase alpha to 1 
            //     yield return null;
            // }
        }

        public Coroutine EffectImport(float time)
        {
            return Effect(0, time);
            // while (canvasGroup.alpha > 0) //with alpha large than 0
            // {
            //     canvasGroup.alpha -= Time.deltaTime/time; //Allow decrease alpha to 0
            //     yield return null;
            // }
        }

        public Coroutine Effect(float target, float time)
        {
            //Cancel running coroutines
            if(currentActiveEffectExport != null)
            {
                StopCoroutine(currentActiveEffectExport);
            }
            //Run fadeout coroutine
            currentActiveEffectExport = StartCoroutine(EffectRoutine(target, time));
            return currentActiveEffectExport;
        }

        public IEnumerator EffectRoutine(float target, float time)
        {
            while (!Mathf.Approximately(canvasGroup.alpha, target)) //with alpha is not 1
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha,target, Time.deltaTime/time); //Allow increase alpha to 1 
                yield return null;
            }
        }


    }
}
