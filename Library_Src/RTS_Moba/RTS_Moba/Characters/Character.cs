using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RTS_Moba.UI;
using RTS_Moba.Pathfind;
namespace RTS_Moba.Characters
{
    public enum Faction { player, ai}
    public enum AlertLevel {green, yellow, orange,red}
    public class Character
    {
        public Faction faction;
        public GameObject go;
        public GameObject projectileAnchor;
        protected Animator animator;

        protected Vector3 location;

        protected int hp_Current = 100;
        #region Stats
        protected int hp =100;
        protected int mp =100;

        protected int attackDamage = 5;

        //1 Attack every X Seconds
        protected float attackSpeed = 2;

        //The Range at which a character needs to be at before being able to AutoAttack
        protected int attackRange   = 10;
        //The Range at which enemies become visible
        protected int sightRange    = 15;
        protected int moveSpeed     = 5;

        #endregion
        public Vector3 Get_Location()
        {
            return location;
        }



        //UI Elements
        HealthBar healthBar;
        FloatingText fText;
        //

        protected order_Types currentOrder = order_Types.stop;
        protected Vector3 targetLocation = Vector3.zero;
        protected Character targetCharacter = null;

        public float healthPercentage
        {
            get { return  (float)(hp_Current)/(float)(hp); }
        }




        
        public Character()
        {
            CharacterManager.instance.AddCharacter(this);
            faction = Faction.ai;
        }

        //The Main Update Routine for any Character.
        //1.Reduce Cooldowns
        //2.Perform any AI Logic
        //3.Perform any Order Logic
        //4.Update UI Bars
        public void Update(float timeDelta)
        {
            //Regardless Action always decrement Cooldowns;
            attackCooldown += timeDelta;

            //Any Logic that needs to Occur for specific units
            Update_AILogic(timeDelta);

            //Debug.Log("Current Order:" + currentOrder.ToString());
            Update_Orders(timeDelta);
            
            //Update HealthBar
            healthBar.Update();
        }

        protected virtual void Update_AILogic(float timeDelta)
        { }


        //
        protected virtual void Update_Orders(float timeDelta)
        {
            if (currentOrder == order_Types.move && targetLocation != location)
            {
                Move(timeDelta, targetLocation);
            }
            else if (currentOrder == order_Types.attackMove && targetLocation != location)
            {
                //Check if Target is Dead.
                if (targetCharacter != null)
                {
                    if (targetCharacter.healthPercentage == 0)
                        targetCharacter = null;
                }

                //If we have no target, check for one and move to our desired location
                if (targetCharacter == null)
                {
                    //Check if Any enemies are in range
                    targetCharacter = CollisionManager.instance.Get_EnemyWithinRange(this, attackRange);

                    //Move to Location
                    Move(timeDelta, targetLocation);
                }
                else
                {
                    //Target is not null.
                    if (CollisionManager.instance.IsWithinRange(this, targetCharacter, attackRange))
                    {
                        //If we are in AttackRange of the Unit Attack!
                        Attack(timeDelta);
                    }
                    else
                    {
                        //Not in range of unit so forget it.
                        targetCharacter = null;
                        //Not in range, move to target ignoring enemy
                        Move(timeDelta, targetLocation);
                    }
                }
            }
            else if (currentOrder == order_Types.huntMove)
            {
                //Check if Target is Dead.
                if (targetCharacter != null)
                {
                    if (targetCharacter.healthPercentage == 0)
                        targetCharacter = null;
                }

                //If we have no target, check for one and move to our desired location
                if (targetCharacter == null)
                {
                    //Check if Any enemies are in range
                    targetCharacter = CollisionManager.instance.Get_EnemyWithinRange(this, sightRange);

                    //Move to Location
                    Move(timeDelta, targetLocation);
                }
                else
                {
                    //Target is not null, we must hunt for it!
                    if (CollisionManager.instance.IsWithinRange(this, targetCharacter, attackRange))
                    {
                        //If we are in AttackRange of the Unit Attack!
                        Attack(timeDelta);
                    }
                    else
                    {
                        //Get The current location of Target
                        targetLocation = targetCharacter.location;

                        //Not in range, move to target ignoring enemy
                        Move(timeDelta, targetLocation);
                    }
                }
            }
            else if (currentOrder == order_Types.attackTarget && targetCharacter != null)
            {
                //Check if Target is Dead.
                if (targetCharacter.healthPercentage == 0)
                    targetCharacter = null;

                //Target is not null.
                if (CollisionManager.instance.IsWithinRange(this, targetCharacter, attackRange))
                {
                    //If we are in AttackRange of the Unit Attack!
                    Attack(timeDelta);
                }
                else
                {
                    //Get The current location of Target
                    targetLocation = targetCharacter.location;
                    //Not in range, move closer to Enemy
                    Move(timeDelta, targetLocation);
                }
            }
            else if (currentOrder == order_Types.stop)
            {
                //Lol Do Nothing. :)
            }

        }






        protected float attackCooldown = 0;
        void Attack(float timeDelta)
        {
            if (attackCooldown >= attackSpeed)
            {
                attackCooldown = 0;
                targetCharacter.TakeDamage(attackDamage);
            }
        }

        protected List<Vector3> pathToGoal = new List<Vector3>();




        protected virtual void Move(float timeDelta, Vector3 goalLocation)
        {

            /*
            //Grid Align the GLocation
            Vector3 modifiedGL = new Vector3((int)goalLocation.x,1, (int)goalLocation.z);

            //Check if our Path2Goal contains the modified GL
            if (pathToGoal.Count==0 || (pathToGoal[pathToGoal.Count - 1].x != modifiedGL.x || pathToGoal[pathToGoal.Count - 1].z != goalLocation.z))
            {
                //Path does not contain goal, request new Path to Target.
                //Create a new P2G
                pathToGoal = Pathfinder.instance.Get_Path(location, modifiedGL);
            }


            //If the half the cooldown is less than half the attack speed pretend we're animating our attack.
            //And so pause before allowing to move again.
            if (attackCooldown < 0.2f)
                return;


            //Check if we are within X of p2g and it's not the last element if so remove it
            if (pathToGoal.Count > 1 && Vector3.Distance(location, pathToGoal[0]) <= 0.2f)
            {
                //We are so dump the value. (ASsuming we have more than 1 left)
                pathToGoal.RemoveAt(0);
            }
            //IF we have a value in p2g and
            if(pathToGoal.Count >0 && Vector3.Distance(location, pathToGoal[0]) >= 0.2f)
                MoveTo(timeDelta, pathToGoal[0]);




            */

        }

        void MoveTo(float timeDelta, Vector3 goalLocation)
        {
            Vector3 direction = goalLocation - location;
            direction = Vector3.Normalize(direction);
            //Rotate the Gobject
            go.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

            direction *= timeDelta * moveSpeed;

            float goalDistance = Vector3.Distance(location, goalLocation);
            float travelDistance = timeDelta * moveSpeed;

            if ( travelDistance > goalDistance)
            {
                location = goalLocation;
            }
            else
            {
                location += direction;
            }
            go.transform.position = location;
        }





        public void Spawn(Vector3 spawnLocation)
        {
            //Create Reference & Spawn Object
            go = GameObject.Instantiate(ResourceManager.Get_Character(this.GetType().Name), spawnLocation, Quaternion.identity) as GameObject;
            location = spawnLocation;

            //Create the UI Element and provide reference for update.
            healthBar = new HealthBar(this);
            fText = new FloatingText(this);
            animator = go.GetComponent<Animator>();

            projectileAnchor = FindProjectileAnchor();
           // Debug.LogError("Projectile Anchor =" + projectileAnchor);
            go.name = this.GetType().Name + faction;
        }

        GameObject FindProjectileAnchor()
        {
            Transform transform = RecursiveSearch("Projectile_Anchor", go.transform);
            if (transform == null)
                return null;
            else
                return transform.gameObject;
        }

        //Recursive Search...
        Transform RecursiveSearch(string name, Transform transform)
        {
            if (transform.name == name)
                return transform;

            Transform t = null;
            foreach (Transform child in transform)
            {
                t = RecursiveSearch(name, child);
                if (t != null && t.name == name)
                    return t;

            }

            return null;
        }



        //Location Based Order
        //Cast Ability at "x,y,z"
        //Move to "x,y,z"
        //AttackMove to "x,y,z"
        public void OrderCharacter(order_Types order, Vector3 location)
        {
            location.y = 1;
            location.x = (int)location.x;
            location.z = (int)location.z;

            location.x += 0.5f;
            location.z += 0.5f;

            currentOrder = order;
            targetLocation = location;
        }

        //Target Based Order
        //Cast Ability on "___"
        //Attack "___"
        //Interact with "___"
        public void OrderCharacter(order_Types order, Character character)
        {
            currentOrder = order;
            targetCharacter = character;
        }






        public void TakeDamage(int value)
        {
            hp_Current -= value;
            if (hp_Current <= 0)
            { 
                hp_Current = 0;
                Destroy();
            }
            fText.AddText("-" + value, Colors.RED);
        }
        public void HealDamage(int value)
        {
            hp_Current += value;
            if (hp_Current > hp )
                hp_Current = hp;

            fText.AddText("+" + value, Colors.GREEN);
        }


        void Destroy()
        {
            CharacterManager.instance.RemoveCharacter(this);
            go.SetActive(false);
            healthBar.Hide();
        }
    }
}
