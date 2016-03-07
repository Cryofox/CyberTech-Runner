using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;


namespace RTS_Moba.UI
{
    class ShieldManager
    {
        List<GameObject> shields;

        public ShieldManager()
        {
            shields = new List<GameObject>();

            //Default with 4 shields 
            for (int i = 0; i < 4; i++)
            {
                GameObject go = ResourceManager.Get_Prefab("Shield");
                go = GameObject.Instantiate(go, Vector3.zero, Quaternion.identity) as GameObject;
                go.SetActive(false);
                shields.Add(go);
            }
        }

        public void Clear()
        {
            for (int i = 0; i < shields.Count; i++)
                shields[i].SetActive(false);
        }

        public void ShowShield(Vector3 position, Quaternion lookRotation)
        {
            position.y = 2;
            for (int i = 0; i < shields.Count; i++)
            {
                if (!shields[i].active)
                {
                    shields[i].transform.position = position;
                    shields[i].transform.rotation = lookRotation;
                    shields[i].SetActive(true);
                    return;
                }
            }
            //Not enough shields to show Another, Create another and add to Pool.
            GameObject go = ResourceManager.Get_Prefab("Shield");
            go = GameObject.Instantiate(go, Vector3.zero, Quaternion.identity) as GameObject;
            shields.Add(go);
            go.transform.position = position;
            go.transform.rotation = lookRotation;
        }


    }
}
