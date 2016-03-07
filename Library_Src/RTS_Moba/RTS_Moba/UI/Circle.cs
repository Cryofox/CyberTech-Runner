using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
namespace RTS_Moba.UI
{
    public class Circle
    {
        float rotation = 0;
        float rotationSpeed = 20;
        GameObject go;

        public Circle()
        {
            go = ResourceManager.Get_Prefab("Circle");
            go = GameObject.Instantiate(go, Vector3.zero, Quaternion.identity) as GameObject;
            go.SetActive(false);
        }


        public void Rotate(float deltaTime)
        {
            rotation += deltaTime * rotationSpeed;
            go.transform.rotation = Quaternion.Euler(0, rotation, 0);
        }

        public void SetActive(bool val)
        {
            go.SetActive(val);
        }

        public bool activeSelf
        {
            get { return go.activeSelf; }
        }

        public void SetPosition(Vector3 position)
        {
            go.transform.position = position;
        }
    }
}
