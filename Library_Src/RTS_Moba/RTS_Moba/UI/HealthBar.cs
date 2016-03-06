using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RTS_Moba.Characters;
using UnityEngineInternal;

namespace RTS_Moba.UI
{
    class HealthBar
    {
        Character character;
        UIProgressBar uiBar;
        GameObject go;

        bool isVisible = true;
        public HealthBar(Character target)
        {
            character = target;
            //First Get our GO:
            go =NGUITools.AddChild(ResourceManager.gameHUD, ResourceManager.Get_UI("HUD_HealthBar")) ;
            //Next we Assign the Target
            go.GetComponent<UIFollowTarget>().target = target.go.transform.FindChild("UI_Anchor");
            uiBar = go.transform.FindChild("HealthBar").GetComponent<UIProgressBar>();
            if (target.faction == Faction.ai)
                uiBar.GetComponent<UISprite>().color = Colors.RED ;
            else
                uiBar.GetComponent<UISprite>().color = Colors.BLUE;
        }

        //Updates the HealthBar
        public void Update()
        {
            if(isVisible)
            { 
                float percentage = character.healthPercentage;
                uiBar.value = percentage;
            }
        }

        public void Hide()
        {
            isVisible = false;
           go.SetActive(false);
        }

    }
}
