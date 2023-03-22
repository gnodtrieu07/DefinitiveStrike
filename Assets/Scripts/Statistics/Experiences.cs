using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using Unity.VisualScripting;
using UnityEngine;

namespace RPG.Statistics
{
    public class Experiences : MonoBehaviour, ISaveable
    {
        [SerializeField] float experiencesGoal = 0;

        //public delegate void ExperienceGainedDelegate();
        public event Action onExperienceGained;

        public void GainExperience(float experience)
        {
            experiencesGoal += experience;
            onExperienceGained();
        }

        public float GetPoint()
        {
            return experiencesGoal;
        }

        public object CaptureState()
        {
            return experiencesGoal;
        }

        public void RestoreState(object state)
        {
            experiencesGoal = (float)state;
        }

    }
}
