using UnityEngine;
using RPG.Core;
using RPG.Attributes;
using RPG.Control;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

        public bool HandleRayCast(PlayerController callingController)
        {
            //In CasCanAtt, it will check if the object in the game is Nulll or not
            //GameObject targetOfGameobject = target.gameObject;
            //If the attack could be return to false so switch to next thing
            if(!callingController.GetComponent<Fighter>().CaseCanAttacking(gameObject)) 
            {
                return false;
            } //Continue in the foreach-loop
                                                                                
            if(Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<Fighter>().Attack(gameObject);
            }

            return true;
        }
    }
}