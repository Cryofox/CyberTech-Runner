using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RTS_Moba;
using RTS_Moba.Characters;
namespace RTS_Moba.Scenarios
{
    public enum Scenario_STATE { win, lose, undecided }

    public interface IScenario
    {
        Scenario_STATE Get_GameState();

        //Called Via GameRoot
        void Update(float time);

        //Called Via GameRoot
        void LateUpdate(float time);

        void UpdateLogic(float time);


        void OrderCharacter(order_Types order, Vector3 location);
        void OrderCharacter(order_Types order, Character character);
        Vector3 Get_PlayerLocation();
    }
}
