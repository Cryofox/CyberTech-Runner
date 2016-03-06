using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RTS_Moba.Characters;
using RTS_Moba.Controllers;
using RTS_Moba.Scenarios;
using RTS_Moba.Pathfind;
namespace RTS_Moba
{
    public enum GAME_STATE { win, lose, undecided}
    class Game
    {
        Extermination gameScene;
        public Game()
        {
            //Load Game Scenario
            gameScene = new Extermination();
        }


        //Called Via GameRoot
        public void Update(float time)
        {
            gameScene.Update(time);
        }
        //Called Via GameRoot
        public void LateUpdate(float time)
        {
            gameScene.LateUpdate(time);
        }


    }
}
