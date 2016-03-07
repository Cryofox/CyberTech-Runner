using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using RTS_Moba.UI;
using RTS_Moba.Pathfind;
using RTS_Moba.Projectiles;
namespace RTS_Moba.Characters
{
    class Soldier : Character
    {
        public enum AnimationState { inCover, running, shooting};
        Vector3 roundedLocation = Vector3.zero;
        //Vector3 roundedTarget = Vector3.zero;
        public Soldier():base()
        {
            faction = Faction.player;
            moveSpeed = 5;
            attackSpeed =  0.5f;
            attackDamage = 10;
            hp = 85;
            hp_Current = hp;
            attackCooldown = attackSpeed;
        }
        
        protected override void Update_Orders(float timeDelta)
        {

            //If we are ordered to move and our location!= Target location then move to it
            if (currentOrder == order_Types.move)
            {
                //Check if Any Enemy is within Firing distance
                EnemyAutoAttack(timeDelta);
                if (Vector3.Distance(targetLocation, location) > 0.1f)
                {
                    animator.SetBool("takeCover", false);
                    //Debug.Log("D=" + Vector3.Distance(targetLocation, location));
                    //Debug.Log("T=" + targetLocation);
                    //Debug.Log("L=" + location);
                    Move(timeDelta, ref targetLocation);
                }
                else
                {
                    animator.SetFloat("speed", 0);
                    //We are in position, take cover if able
                    TakeCover(timeDelta);
                }
            }


        }


        void EnemyAutoAttack(float timeDelta)
        {
            attackCooldown += timeDelta;
            //Check if our Target is even Valid
            if( targetCharacter!= null)
            {
                if (!CollisionManager.instance.IsWithinRange(this, targetCharacter, attackRange))
                    targetCharacter = null;

                if (targetCharacter!=null && targetCharacter.healthPercentage == 0)
                    targetCharacter = null; 
            }


            if ( targetCharacter == null)
                targetCharacter = CollisionManager.instance.Get_EnemyWithinRange(this, attackRange);


            if (targetCharacter != null && attackCooldown>= attackSpeed && projectileAnchor!=null)
            {
                attackCooldown = 0;
                Vector3 target = targetCharacter.Get_Location();
                target.y = 1;
                Vector3 direction = target - location;
                direction = Vector3.Normalize(direction);

                ProjectileManager.instance.SpawnProjectile(projectileAnchor.transform.position, direction, fa);
            }


        }

        void TakeCover(float timeDelta)
        {
            int x = (int)location.x;
            int y = (int)location.z;


            //Orient Based on Cover Location
            if (!Pathfinder.instance.isLocationWalkable(x + 1, y))
            { 
                go.transform.rotation = Quaternion.LookRotation(new Vector3(-1, 0, 0));
               animator.SetBool("takeCover", true);
            }
            else if (!Pathfinder.instance.isLocationWalkable(x - 1, y))
            {
                go.transform.rotation = Quaternion.LookRotation(new Vector3(1, 0, 0));
                animator.SetBool("takeCover", true);
            }
            else if (!Pathfinder.instance.isLocationWalkable(x , y+1))
            {
                go.transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, -1));
                animator.SetBool("takeCover", true);
            }
            else if (!Pathfinder.instance.isLocationWalkable(x, y-1))
            {
                go.transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, 1));
                animator.SetBool("takeCover", true);
            }
            else
                animator.SetBool("takeCover", false);


        }


        //Only this method uses a reference for V3
        void Move(float timeDelta, ref Vector3 finalTarget)
        {
            if (Vector3.Distance(finalTarget, location) <= 0.1f)
                return;

            roundedLocation = new Vector3((int)location.x, 1, (int)location.z);
            finalTarget.y = 1;

            Vector3 target = finalTarget;
            //Add a new Check... As we are not at our target check if our List of Nodes is Eithor (Empty), or does not contain our target
            if (pathToGoal.Count > 0)
            {
                //Check if it contains this node
                if (pathToGoal[pathToGoal.Count - 1] != finalTarget)
                {
                    pathToGoal.Clear();
                    pathToGoal = Pathfinder.instance.Get_Path(location, finalTarget);
                }
                else //It Does Check if we are Within 0.1 of 1st Node
                {
                    if (Vector3.Distance(location, pathToGoal[0]) <= 0.1f)
                    {
                        pathToGoal.RemoveAt(0);
                        //It's not possible this Node is the Final one due to the Entry if check so...
                    }
                }
            }
            else
            {
                pathToGoal = Pathfinder.instance.Get_Path(location, finalTarget);
            }

            target = pathToGoal[0];
            //This prevents the unit from attempting to reach impossible Terrain.
            //Sets the Requested FinalTarget (in global space) to the one calculated from the Path.
            //This is why fTarget Must be pass by reference.
            finalTarget = pathToGoal[pathToGoal.Count - 1];



            Vector3 velocity = Vector3.Normalize(target - location) * moveSpeed * timeDelta;
            float distance = Vector3.Distance(location, target);

            //Overshot location, correct.
            if (velocity.magnitude < distance)
            {
                location += velocity;
                if (velocity.magnitude > 0)
                    go.transform.rotation = Quaternion.LookRotation(velocity);
            }
            else
            {
                location.x = (float)((int)target.x) + 0.50f;
                location.z = (float)((int)target.z) + 0.50f;
                velocity = Vector3.zero;
            }

            //Debug.Log("Velocity=" + velocity.magnitude);
            //Debug.Log("Velocity Act=" + velocity);
            //Debug.Log("Loc Post=" + location);
            //Debug.Log("Tar P ="+ target);

            go.transform.position = location;
            animator.SetFloat("speed", Vector3.Magnitude(velocity));

            //For each Node up untill Max
            LineManager.instance.SetLine(location, pathToGoal[0]);
            for (int i=0;i<pathToGoal.Count-1;i++)
            {
                LineManager.instance.SetLine(pathToGoal[i], pathToGoal[i + 1]);
            }

            CircleManager.instance.PlaceCircle(pathToGoal[pathToGoal.Count-1]);



        }
    


  

    }
}
