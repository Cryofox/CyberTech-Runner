using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace RTS_Moba.Controllers
{
    class MouseInput
    {
        public static bool leftMouse_Down = false;
        public static bool leftMouse = false;
        public static bool leftMouse_Up = false;

        public static bool rightMouse_Down = false;
        public static bool rightMouse = false;
        public static bool rightMouse_Up = false;

        public static void Update()
        {
            leftMouse = Input.GetMouseButton(0);
            leftMouse_Down = Input.GetMouseButtonDown(0);
            leftMouse_Up = Input.GetMouseButtonUp(0);

            rightMouse = Input.GetMouseButton(1);
            rightMouse_Down = Input.GetMouseButtonDown(1);
            rightMouse_Up = Input.GetMouseButtonUp(1);

        }

        static void Update_LeftMouse()
        {

        }
        static void Update_RightMouse()
        {

        }




    }
}
