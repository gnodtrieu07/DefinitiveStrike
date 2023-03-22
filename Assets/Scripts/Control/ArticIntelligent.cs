using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Movement;
using RPG.Core;
using UnityEngine;
using System;
using RPG.Attributes;
using RPG.Utils;

namespace RPG.Control
{
    public class ArticIntelligent : MonoBehaviour
    {
        [SerializeField] float visionDistance = 5f;
        [SerializeField] float theMistrust = 4f;
        [SerializeField] float argoCooldownTime = 3f;
        [SerializeField] WatchWay watchWay;
        [SerializeField] float waypointSuspend = 3f;
        [SerializeField] float waypointSequence = 2f;
        [Range(0,1)]
        [SerializeField] float waypointPace = 0.4f;
        [SerializeField] float shouldDistance = 5f;
        Fighter inFighter;
        GameObject player; 
        Health healthblood;
        Mover moving;
        float timeLastSawObject = Mathf.Infinity;
        float timeBeforeNextObject = Mathf.Infinity;
        int currentWaypointIndex = 0;
        float timeSinceAggrevated = Mathf.Infinity;
        LazyValue<Vector3> guardianProtectionPosition;

        private void Awake() {
            inFighter = GetComponent<Fighter>();
            player = GameObject.FindWithTag("Player");
            healthblood = GetComponent<Health>();
            moving = GetComponent<Mover>();

            guardianProtectionPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        private void Start() {
            guardianProtectionPosition.ForceInit(); 
        }


        private void Update()
        {
            //If deaded, so don't do anything
            if (healthblood.Decedent())
            {
                return;
            }
            //The distance from enemy to player make them can be recognize and attacking the player
            //Method Ectracted
            //If this condition is true so start the attack behaviour
            if (IsAggrevated() && inFighter.CaseCanAttacking(player)) //If the player's distance is smaller than the chasing distance
            {
                // if(gameObject.tag == "Player") 
                // {
                //     print("Chasing Player");
                // }
                // print(gameObject.name + "Following"); 
                // GetComponent<Fighter>().Attack(player);
                //timeLastSawObject = 0;
                AttackBehaviourEnemy();
            }
            //If the last time from the last time you see players is less than suspected time, they will commit suspicion
            else if (timeLastSawObject < theMistrust)
            {
                //The function of suspicion
                MistrustBehaviourEnemy();
            }
            //If not, then return to the default position
            else
            {
                ProtectWatchwayBehaviourEnemy();
            }
            UpdateTimeinLastBefore();
        }

         public void Aggrevate()
         {
            timeSinceAggrevated = 0;

         } 

        private void UpdateTimeinLastBefore()
        {
            timeLastSawObject += Time.deltaTime;
            timeBeforeNextObject += Time.deltaTime;
            timeSinceAggrevated += Time.deltaTime;
        }

        private void ProtectWatchwayBehaviourEnemy()
        {
            Vector3 followingWay = guardianProtectionPosition.value; 
            
            if(watchWay != null)
            {
                //At the reference point you want to aim, then you need to turn around
                if (InThatWaypoint())
                {
                    timeBeforeNextObject = 0;
                    //It is necessary to raise the reference point towards it to become the next reference point
                    TurnAroundWaypoint();
                }
                //Otherwise it will only set the next position equal to the current reference point
                followingWay = GetCurrentWaypoint();
            }

            //If time from the BeforeNxWP > SuspendWP so that is time when we can start moving
            if(timeBeforeNextObject > waypointSuspend)
            {
                //And need to set the next position to start moving action
                moving.StartMoveAction(followingWay, waypointPace);
            }

        }

        private bool InThatWaypoint()
        {
            //GetCurrentWaypoint will give us the distance to the reference point 
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint()); 
            return distanceToWaypoint < waypointSequence;
        }

        private void TurnAroundWaypoint()
        {
            currentWaypointIndex = watchWay.ConnectNextIndex(currentWaypointIndex);
        }

        private Vector3 GetCurrentWaypoint()
        {
           return watchWay.GetPositionWPoint(currentWaypointIndex);
        }

        private void MistrustBehaviourEnemy()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehaviourEnemy()
        {
            timeLastSawObject = 0;
            inFighter.Attack(player);

            AggrevateNearbyEnemies();
        }

        //AI calling the teammates to fight
        private void AggrevateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shouldDistance, Vector3.up, 0);
            //Loop over all the hits
            foreach(RaycastHit  hit in hits)
            {
                ArticIntelligent AI = hit.collider.GetComponent<ArticIntelligent>();
                if(AI == null)
                {
                    continue;
                }
                AI.Aggrevate();
            }
            //Find any enemy component
        }

        //Attack range of player
        private bool IsAggrevated()
        {
            //This way make us enemy can find more thing like the position of the Player 
            //If the distance to players is smaller than the monitoring distance, then return True for the player's attack range
            float elements = Vector3.Distance(player.transform.position, transform.position);//The distance between the player's position and the current distance 
            //check aggrevated
            //When enemy have been attack, he can move and fight to the player not standing like the old
            return elements < visionDistance || timeSinceAggrevated < argoCooldownTime; 
        }

        //The function called by Unity
        //This Gizmos will appear the radius distance that the enemy can see and start attacking
        //If not seen anyone in range of Gizmos, enemy will stopped move
        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, visionDistance); 
        }
    }

}