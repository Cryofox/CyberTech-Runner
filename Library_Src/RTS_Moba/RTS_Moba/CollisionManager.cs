using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RTS_Moba.Characters;
namespace RTS_Moba
{
    class CollisionManager
    {
        #region Singleton Constructor
        //Singleton Constructor
        private static readonly CollisionManager _instance = new CollisionManager();
        private CollisionManager() { }
        public static CollisionManager instance
        {
            get
            {
                return _instance;
            }
        }
        #endregion

        public Character Get_EnemyWithinRange(Character character, float radius)
        {
            List<Character> characters = CharacterManager.instance.characters;
            Character closestCharacter = null;
            float lastDistance = 0;
            for (int i = 0; i < characters.Count; i++)
            {
                if (characters[i].faction != character.faction)
                {
                    float distance = Vector3.Distance(character.Get_Location(), characters[i].Get_Location());
                    //Check if the Distance between the two targets is less than half each combined radius
                    if(distance<= radius)
                        if (closestCharacter == null || distance< lastDistance)
                        {
                            lastDistance = distance;
                            closestCharacter = characters[i];
                        }
                }
            }
            return closestCharacter;
        }

        //Returns whether the target character is in range of character based on supplied radius
        public bool IsWithinRange(Character character, Character targetCharacter, float radius)
        {
            float distance = Vector3.Distance(character.Get_Location(), targetCharacter.Get_Location());
            return (distance<=radius);        
        }





    }
}
