using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Statistics
{
    [CreateAssetMenu(fileName = "Evolution", menuName ="Statistics/New Evolution", order = 0)]
    public class Evolution : ScriptableObject
    {
        [SerializeField] EvolutionCharacterClass[] characterClass = null;

        Dictionary<CharacterClassPath, Dictionary<StatcExpand, float[]>> lookupTable = null;

        public float GetStatcsExpand(StatcExpand statcExpand,CharacterClassPath characterClassPath, int level)
        {
            ConstructorSearch();
            float[] levels = lookupTable[characterClassPath][statcExpand];

            if(levels.Length < level)
            {
                return 0;
            }

            return levels[level -1];
            //Call this the evolution class in the full loop
            // foreach(EvolutionCharacterClass evolutionClass in characterClass)
            // {
            //     if(evolutionClass.characterClassPath != characterClassPath) //If method in front = back - class that we passed in to get health
            //     {
            //         //return evolutionClass.health[level - 1];
            //         continue;
            //     }

                //That's going to go over what it's going to go over the statcs in the evolution class
                //It in fact and we're going to get this
            //     foreach(EvolutionStatcs evolutionStatcs in evolutionClass.statcs)
            //     {
            //         if(evolutionStatcs.statcExpand != statcExpand)
            //         {
            //             continue;
            //         }
            //         if(evolutionStatcs.levels.Length < level) 
            //         {
            //             continue;
            //         }
            //         return evolutionStatcs.levels[level - 1];
            //     }
            // }
            // return 0;
        }

        public int GetLevels(StatcExpand statcExpand, CharacterClassPath characterClassPath)
        {
            ConstructorSearch();
            //Build and get the look up character class stacts and the length of levels array
            float[] levels = lookupTable[characterClassPath][statcExpand];
            return levels.Length;
        }

        private void ConstructorSearch()
        {
            if(lookupTable != null)
            {
                return;
            }
            //Build a lookup table first from scratch
            lookupTable = new Dictionary<CharacterClassPath, Dictionary<StatcExpand, float[]>>();
            //We have gone over each character chass that we got
            foreach(EvolutionCharacterClass evolutionClass in characterClass)
            {
                //And added in a statcs lookup table
                var statcsLookupTable = new Dictionary<StatcExpand, float[]>();
                //The statcs lookup table is in turn built buy going over all the stacts we got within the evolution class
                foreach(EvolutionStatcs evolutionStatcs in evolutionClass.statcs)
                {
                    //And using the key, the evolution statcs as the key and the levels is then the variable that we want to create
                    statcsLookupTable[evolutionStatcs.statcExpand] = evolutionStatcs.levels;
                }

                lookupTable[evolutionClass.characterClassPath] = statcsLookupTable;
            }
        }

        [System.Serializable]
        public class EvolutionCharacterClass
        {
            public CharacterClassPath characterClassPath;
            public EvolutionStatcs[] statcs; //==stats
            //public float[] health;
        }

        [System.Serializable]
        public class EvolutionStatcs
        {
            public StatcExpand statcExpand;
            public float[] levels;
        }
    }
}
