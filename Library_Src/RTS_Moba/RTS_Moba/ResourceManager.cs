using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using UnityEngine;
namespace RTS_Moba
{
    public enum cursorTypes
    {
        pan_up,
        pan_down,
        pan_left,
        pan_right,

        attack1,
        attack2,

        move1,
        move2,
        harvest1,
        harvest2,
        select
    }

    class ResourceManager
    {
        public static GameObject gameHUD;

#region Get GameObjects
        public static GameObject Get_Character(string name)
        {
            return Get_Prefab("Characters/"+ name);
        }

        public static GameObject Get_Tile()
        {
            return Get_Prefab("Tiles/Tile");
        }
        public static GameObject Get_Wall()
        {
            return Get_Prefab("Tiles/Wall");
        }
        public static GameObject Get_UI(string name)
        {
            return Get_Prefab("UI/"+name);
        }
        public static GameObject Get_Prefab(string name)
        {
            GameObject go = Resources.Load<GameObject>("Prefabs/" + name);
            if (go == null)
            {
                go = Resources.Load<GameObject>("Prefabs/PlaceHolder");
            }
            return go;
        }
#endregion
        public static Texture2D Get_Texture(string name)
        {
            return Resources.Load<Texture2D>("Textures/" + name);
        }
        public static Texture2D Get_MouseCursor(cursorTypes ct)
        {
            return Get_Texture("Cursors/" + ct.ToString());
        }



    }
}
