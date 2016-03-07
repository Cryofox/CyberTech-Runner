using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
namespace RTS_Moba.UI
{
    class LineManager
    {

        #region Singleton Constructor
        //Singleton Constructor
        private static readonly LineManager _instance = new LineManager();

        public static LineManager instance
        {
            get
            {
                return _instance;
            }
        }
        #endregion

        List<LineRenderer> lines;
        private LineManager()
        {
            lines = new List<LineRenderer>();
            for (int x = 0; x < 20; x++)
            {
                GameObject go = ResourceManager.Get_Prefab("Line");
                go = GameObject.Instantiate(go, Vector3.zero, Quaternion.identity) as GameObject;
                //go.SetActive(false);
                //lines.Add(go);
                LineRenderer lr = go.GetComponent<LineRenderer>();
                lr.enabled = false;
            }
        }

        public void ClearLines()
        {
            for (int x = 0; x < lines.Count; x++)
                lines[x].enabled = false;
        }

        public void SetLine(Vector3 start, Vector3 end)
        {
            start.y = 1;
            end.y = 1;
            for (int i = 0; i < lines.Count; i++)
            {
                if (!lines[i].enabled)
                {
                    lines[i].SetPosition(0, start);
                    lines[i].SetPosition(1, end);
                    lines[i].enabled = true;
                    //Set the Tiling


                    return;
                }
            }

            GameObject go = ResourceManager.Get_Prefab("Line");
            go = GameObject.Instantiate(go, Vector3.zero, Quaternion.identity) as GameObject;
            //go.SetActive(false);
            //lines.Add(go);
            LineRenderer lr = go.GetComponent<LineRenderer>();
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            lines.Add(lr);

        }

    }
}
