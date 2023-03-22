using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Movement;
using RPG.Core;
using RPG.Control;
using UnityEngine;
using RPG.Attributes;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control{
    public class PlayerController : MonoBehaviour
    {
        Health healthblood;

        // enum CursorType
        // {
        //     None,
        //     Movement, 
        //     Combat,
        //     UserInterface
        // }

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D textture;
            public Vector2 hotspot;
        }
        
        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavMeshProjectionDistance = 1f;

        [SerializeField] float raycastRadius = 1;
        //[SerializeField] float maxNavPathLength = 40f;

        private void Awake() {
            healthblood = GetComponent<Health>();
        }

         private void Update()
        {
            if(InteractWithUI())
            {
                return;
            }
            if (healthblood.Decedent()) 
            {
                SetCursors(CursorType.None);
                return;
            }

            if(InteractWithComponent())
            {
                return;
            }
            // if (InteractWithCombat()) 
            // {
            //     return;
            // }
            if (InteractWithMovement()) return;
                print("Nothing to do.");
        }


        private bool InteractWithUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetCursors(CursorType.UserInterface);
                return true;
            }
            return false;
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach(IRaycastable raycastable in raycastables)
                {
                    if(raycastable.HandleRayCast(this))
                    {
                        SetCursors(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        RaycastHit[] RaycastAllSorted()
        {
            //Get all hits
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);
            //Sort by distance
            //Build array distance
            float[] distances = new float[hits.Length];
            for(int i = 0; i < hits.Length; i++)
            {
                    distances[i] = hits[i].distance;
            }
            //Sorted the hit
            Array.Sort(distances, hits);
            //Return
            return hits;
        }

        // private bool InteractWithCombat()
        // {
        //     RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
        //     foreach (RaycastHit hit in hits)
        //     {
        //         CombatTarget target = hit.transform.GetComponent<CombatTarget>();
        //         if(target == null) continue;
        //         //In CasCanAtt, it will check if the object in the game is Nulll or not
        //         //GameObject targetOfGameobject = target.gameObject;
        //         //If the attack could be return to false so switch to next thing
        //         if(!GetComponent<Fighter>().CaseCanAttacking(target.gameObject)) {continue;} //Continue in the foreach-loop
                                                                                   
        //         if(Input.GetMouseButtonDown(0))
        //         {
        //             GetComponent<Fighter>().Attack(target.gameObject);
        //         }

        //         SetCursors(CursorType.Combat);
        //         return true;
        //     }
        //     return false;
        //     //throw new NotImplementedException();
        // }


        private bool InteractWithMovement()
        {
            // RaycastHit hit;
            // bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);
            if (hasHit)
            {
                if(!GetComponent<Mover>().CanMoveTo(target))
               {
                    //If we can't move to the target so we cannot interact with movement
                    return false;
                }
                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(target, 1f);
                }
                SetCursors(CursorType.Movement);
                return true;
            }
            return false;
        }

        //Not allow move to the river
        private bool RaycastNavMesh(out Vector3 target)
        {
            
            target = new Vector3();

            //Raycast to terrain
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if(!hasHit)
            {
                return false;
            }
            //Find nearest navmesh point
            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(hit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);
            if(!hasCastToNavMesh)
            {
                return false;
            }
            //Return true it found
            target = navMeshHit.position;
            //Not allow player move to long road
            // NavMeshPath path = new NavMeshPath();
            // bool hasPath = NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas,path);
            // if(!hasPath)
            // {
            //     return false;
            // }

            // if(path.status != NavMeshPathStatus.PathComplete)
            // {
            //     return false;
            // }

            // if(GetPathLength(path) > maxNavPathLength)
            // {
            //     return false;
            // }

            return true;    
        }

        // private float GetPathLength(NavMeshPath path)
        // {       
        //     float total = 0;
        //     if(path.corners.Length <2)
        //     {
        //         return total;
        //     }
        //     for(int i = 0; i < path.corners.Length -1; i++) {
        //         total += Vector3.Distance(path.corners[i], path.corners[i+1]);
        //     }


        //     return 0;
        // }

        private void SetCursors(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.textture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type){
            foreach(CursorMapping mapping in cursorMappings)
            {
                if(mapping.type == type)
                {
                    return mapping;
                }
            }
            return cursorMappings[0];
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}