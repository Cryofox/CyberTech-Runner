using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RTS_Moba.Characters;
using UnityEngineInternal;
namespace RTS_Moba.UI
{
    class FloatingText
    {


        List<GameObject> textInstances;
        Character target;
        public FloatingText(Character target)
        {
            this.target = target;
            textInstances = new List<GameObject>();
        }
        GameObject Get_NewText()
        {
            return NGUITools.AddChild(ResourceManager.gameHUD, ResourceManager.Get_UI("HUD_Text"));
        }
        public void AddText(string text,Color color)
        {
            HUDText hdText;
            //Recycle any Existing Floating Texts
            for (int i = 0; i < textInstances.Count; i++)
            {
                hdText = textInstances[i].GetComponent<HUDText>();
                //If any textAsset that is not Visible exists use it!
                if (!hdText.isVisible)
                {
                    hdText.Add(text, color, 0f);
                    return;
                }
            }

            //Create a new FloatingText Instance
            GameObject go= Get_NewText();
            hdText = go.GetComponent<HUDText>();
            textInstances.Add(go);

            //Next we Assign the Target
            go.GetComponent<UIFollowTarget>().target = target.go.transform.FindChild("UI_Anchor");
            hdText = go.GetComponent<HUDText>();
            hdText.Add(text, color, 0f);
        }
    }
}
