using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Utils;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

namespace RPG.Statistics
{
    public class StatisticsBase : MonoBehaviour
    {
        [Range(1,99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClassPath characterClassPath;
        [SerializeField] GameObject levelUpParticleEffect = null;
        [SerializeField] bool shouldUseModifier = false;
        [SerializeField] Evolution evolution = null;

        public event Action onLevelUp;
        int level;
        LazyValue<int> presentLevel;
        Experiences experiences;
        private void Awake() {
            experiences = GetComponent<Experiences>();
            presentLevel = new LazyValue<int>(FigureLevel);
        }

        private void Start() {
            presentLevel.ForceInit();
        }

        private void OnEnable()
        {
            if(experiences != null)
            {
                experiences.onExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable() {
            if(experiences != null)
            {
                experiences.onExperienceGained -= UpdateLevel;
            }
        }

        private void UpdateLevel() {

            // if(gameObject.tag == "Player")
            // {
            //     print(GetLevel());    
            // }

            int newLevel = FigureLevel();
            if(newLevel > presentLevel.value)
            {
                presentLevel.value = newLevel;
                LevelUpEffect(); 
                print("Level Raise");
                onLevelUp();
            }
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpParticleEffect, transform); 
        }

        public float GetStatcsExpand(StatcExpand statcExpand)
        {
            return (GetBaseStat(statcExpand) + GetAdditiveModifier(statcExpand)) * (1 + GetPercentageModifier(statcExpand)/100);
        }

        private float GetBaseStat(StatcExpand statcExpand)
        {
            return evolution.GetStatcsExpand(statcExpand, characterClassPath, GetLevel());
        }


        // public float GetExperienceReceive()
        // {
        //     return 10;
        // }

        public int GetLevel()
        {
            // if(presentLevel <1)
            // {
            //     presentLevel = FigureLevel();
            // }
            return presentLevel.value;
        }
        private float GetAdditiveModifier(StatcExpand statcExpand)
        {
            if(!shouldUseModifier)
            {
                return 0;
            }
            float total = 0;
            foreach(IModifier provider in GetComponents<IModifier>()) 
            {
                foreach(float modifiers in provider.GetAdditiveModifier(statcExpand))
                {
                    total += modifiers;
                }
            }
            return total;
        }

        private float GetPercentageModifier(StatcExpand statcExpand)
        {

            if(!shouldUseModifier)
            {
                return 0;
            }

            float total = 0;
            foreach(IModifier provider in GetComponents<IModifier>()) 
            {
                foreach(float modifiers in provider.GetPercentageModifier(statcExpand))
                {
                    total += modifiers;
                }
            }
            return total;
        }

        public int FigureLevel()
        {
            Experiences experiences = GetComponent<Experiences>();
            if(experiences == null) 
            {
                return startingLevel;
            }

            float presentEXP = experiences .GetPoint();
            int maximumLevel = evolution.GetLevels(StatcExpand.ExperiencesToNextLevel, characterClassPath);
            for(int level = 1; level <= maximumLevel; level++)
            {
                float ExpToNextLevel = evolution.GetStatcsExpand(StatcExpand.ExperiencesToNextLevel, characterClassPath, level);
                if(ExpToNextLevel > presentEXP)
                {
                    return level;
                }
            }

            return maximumLevel + 1;
        }
    }
}
