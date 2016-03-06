using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RTS_Moba.Characters;

using RTS_Moba.Controllers;
using RTS_Moba;
using RTS_Moba.Pathfind;
namespace RTS_Moba.Scenarios
{
    class Extermination : IScenario
    {



        private Terrain terrain;
        private PlayerController playerController;


        //Player Characters
        private Character playerCharacter;

        //Enemy AI
        public Extermination()
        {
            //Setup Terrain
            terrain = new Terrain(100, 100);
            //Link Player Controller
            playerController = new PlayerController(this);

            //Setup Player Characters
            //Create Player's toys
            playerCharacter = new Soldier();
            playerCharacter.Spawn(new Vector3(50, 0.5f, 50));

            //Create Enemy AI
            //Setup Enemy Units
            //Setup_EnemyAI();
        }


        void Setup_EnemyAI()
        {
            Character enemyAI;
            System.Random r = new System.Random();
            for (int i = 0; i < 10; i++)
            {
                enemyAI = new Crawler();

                int x = r.Next(0, 100);
                int y = r.Next(0, 100);
                while (!Pathfinder.instance.isLocationWalkable(x, y))
                {
                    x = r.Next(0, 100);
                    y = r.Next(0, 100);
                }
                enemyAI.Spawn(new Vector3(x, 0.5f, y));
            }
        }

        public Scenario_STATE Get_GameState()
        {
            Scenario_STATE currentState = Scenario_STATE.undecided;

            //Debug
            return currentState;

            List<Character> chars = CharacterManager.instance.characters;
            int playerCount = 0;
            int enemyCount = 0;
            for (int i = 0; i < chars.Count; i++)
            {
                if (chars[i].faction == Faction.ai)
                    enemyCount++;
                else if (chars[i].faction == Faction.player)
                    playerCount++;

                //Lose and Win conditions not met, so skip list processing
                if (enemyCount > 0 && playerCount > 0)
                    break;
            }
            if (playerCount == 0)
                currentState = Scenario_STATE.lose;

            if (enemyCount == 0)
                currentState = Scenario_STATE.win;

            return currentState;
        }

        const float maxTimeInterval = 0.2f;
        //Called Via GameRoot
        public void Update(float time)
        {
            Scenario_STATE currentState = Get_GameState();

            if (currentState == Scenario_STATE.undecided)
            {
                MouseInput.Update();
                //Cap Logic calculations to Max Time Elapsed (1 second) 
                float timeInterval = 0;
                while (time > 0)
                {
                    timeInterval = Math.Min(maxTimeInterval, time);
                    time -= timeInterval;

                    //Apply Game Logic Here
                    UpdateLogic(timeInterval);
                }
            }
            else
            {
                Debug.LogError("You -" + currentState.ToString() + "-");
            }

            //Debug Draw once Per Update (Frame)
            Pathfinder.instance.Debug_Draw();
        }

        //Called Via GameRoot
        public void LateUpdate(float time)
        {
            //Cap Logic calculations to Max Time Elapsed (1 second) 
            float timeInterval = 0;
            while (time > 0)
            {
                timeInterval = Math.Min(maxTimeInterval, time);
                time -= timeInterval;
            }
        }

        public void UpdateLogic(float time)
        {
            //Process Player Input
            CharacterManager.instance.Update(time);
            //Apply Game Logic Here
            playerController.Update(time);
        }


        public void OrderCharacter(order_Types order, Vector3 location)
        {
            playerCharacter.OrderCharacter(order, location);
        }
        public void OrderCharacter(order_Types order, Character character)
        {
            playerCharacter.OrderCharacter(order, character);
        }

        public Vector3 Get_PlayerLocation()
        {
            return playerCharacter.Get_Location();
        }
    }
}
