using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace RTS_Moba.Debugging
{
    class Debugger
    {
        public const bool isDebugMode = false;


        public static void DrawRectangle(Rect rectangle, float duration = 0) { DrawRectangle(rectangle, Color.white, duration); }


        public static void DrawRectangle(Rect rectangle, Color color, float duration = 0)
        {
            Vector3 cornerStart = Vector3.zero;
            Vector3 cornerEnd = Vector3.zero;


            cornerStart.y = cornerEnd.y  =  2;

            //Bottom Edge
            cornerStart.x = rectangle.left;
            cornerEnd.x = rectangle.right;
            cornerStart.z = cornerEnd.z = rectangle.bottom;

            DrawLine(cornerStart, cornerEnd, color,duration);

            //Top Edge
            cornerStart.z = cornerEnd.z = rectangle.top;
            DrawLine(cornerStart, cornerEnd, color, duration);

            //Left Edge
            cornerStart.x = cornerEnd.x = rectangle.left;
            cornerStart.z = rectangle.top;
            cornerStart.z = rectangle.bottom;
            DrawLine(cornerStart, cornerEnd, color, duration);

            //Left Edge
            cornerStart.x = cornerEnd.x = rectangle.right;
            DrawLine(cornerStart, cornerEnd, color, duration);

        }

        public static void DrawLine(Vector3 start, Vector3 end, float duration = 0) { DrawLine(start, end, Color.white, duration); }
        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0)
        {
            Debug.DrawLine(start, end, color,duration);
        }

    }
}
