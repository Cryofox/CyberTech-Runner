using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
namespace RTS_Moba.UI
{
    class CircleManager
    {

        #region Singleton Constructor
        //Singleton Constructor
        private static readonly CircleManager _instance = new CircleManager();

        public static CircleManager instance
        {
            get
            {
                return _instance;
            }
        }
        #endregion

        List<Circle> circles;
        private CircleManager()
        {
            circles = new List<Circle>();
            for (int x = 0; x < 20; x++)
            {
                circles.Add(new Circle());
            }
        }


        public void Update(float deltaTime)
        {
   
            for (int x = 0; x < circles.Count; x++)
                circles[x].Rotate(deltaTime);
        }
        public void Clear()
        {
            for (int x = 0; x < circles.Count; x++)
                circles[x].SetActive(false);
        }

        public void PlaceCircle(Vector3 position)
        {
            position.y = 1;
            for (int i = 0; i < circles.Count; i++)
            {
                if (!circles[i].activeSelf)
                {
                    circles[i].SetPosition(position);
                    circles[i].SetActive(true);
                    return;
                }
            }

            Circle circle = new Circle();
            circle.SetPosition(position);
            circle.SetActive(true);
            circles.Add(circle);

        }

    }
}
