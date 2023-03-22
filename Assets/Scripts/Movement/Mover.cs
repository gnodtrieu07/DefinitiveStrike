using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 
using RPG.Movement;
using RPG.Core;
using RPG.Combat;
using RPG.Saving;
using RPG.Attributes;

namespace RPG.Movement{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] Transform target;
        [SerializeField] float activeMaxPace = 4f;
        [SerializeField] float maxNavPathLength = 40f;
        
        Health healthblood;
        NavMeshAgent navMeshAgent;

        private void Awake() {
            navMeshAgent = GetComponent<NavMeshAgent>();     
            healthblood = GetComponent<Health>();
        }
        void Start() 
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            // if(Input.GetMouseButtonDown(0))
            // {
            //     MoveToCursor();
            // }

             //Disabled the navigation of the dead character
            //An update will disable as soon as when prim or enemy die

            navMeshAgent.enabled = !healthblood.Decedent();  
            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination, float paceSection)
        {
            GetComponent<ActionScheduler>().StartAction(this);   
            MoveTo(destination, paceSection);
        }

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas,path);
            if(!hasPath)
            {
                return false;
            }

            if(path.status != NavMeshPathStatus.PathComplete)
            {
                return false;
            }

            if(GetPathLength(path) > maxNavPathLength)
            {
                return false;
            }

            return true;
        }


        //mouse-click point move to destiantion
        public void MoveTo(Vector3 destination, float paceSection)
        {
            navMeshAgent.destination = destination;
            navMeshAgent.speed = activeMaxPace * Mathf.Clamp01(paceSection);
            navMeshAgent.isStopped = false;
        }
    
        //stop-default
        public void Cancel()
        {
            navMeshAgent.isStopped = true;
        }

        //animator-update follow speedmove
        private void UpdateAnimator()
        {
            Vector3 velocity = navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("forwardSpeed", speed);
        }

        private float GetPathLength(NavMeshPath path)
        {       
            float total = 0;
            if(path.corners.Length <2)
            {
                return total;
            }
            for(int i = 0; i < path.corners.Length -1; i++) {
                total += Vector3.Distance(path.corners[i], path.corners[i+1]);
            }


            return 0;
        }

        [System.Serializable]
        struct MoverSavedData
        {
            public SerializableVector3 position;
            public SerializableVector3 rotation;
        }


        //Save function called form interface SerializableVector3
        public object CaptureState()
        {
            MoverSavedData database = new MoverSavedData();
            database.position = new SerializableVector3(transform.position);
            database.rotation = new SerializableVector3(transform.eulerAngles);
            return database;
        }
        //Load function called form interface SerializableVector3
        public void RestoreState(object state)
        {
            //Save the data object that created in CaptureState
            MoverSavedData database = (MoverSavedData) state;
            navMeshAgent.enabled = false;
            //When we setting up our transformed position, we have to get that our of the data object
            transform.position = database.position.ToVector();
            transform.eulerAngles = database.rotation.ToVector();
            navMeshAgent.enabled = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

    }
}