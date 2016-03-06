using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RTS_Moba.Characters;
using RTS_Moba.UI;

using RTS_Moba.Controllers;
using RTS_Moba.Scenarios;
namespace RTS_Moba
{
    public enum CamState { panning, stationary }
    public enum InputState { normal, attack }


    class PlayerController
    {
        CamState camState;
        InputState inputState;
        GameObject moveArrow;
        Material arrowMat;
        GameObject camFocus;
        IScenario game;


        public PlayerController(IScenario game)
        {
            camState = CamState.stationary;
            inputState = InputState.normal;
            this.game = game;
            currentCursor = cursorTypes.pan_down;
            moveArrow = GameObject.Find("RTS_Arrow");
            camFocus = GameObject.Find("CamFocus");
            arrowMat = moveArrow.transform.FindChild("ArrowMesh").GetComponent<Renderer>().material;
        }


        public void Update(float timeDelta)
        {
            camState = CamState.stationary;
            Update_Keyboard(timeDelta);
            Update_Mouse(timeDelta);
        }

        public void Update_Keyboard(float timeDelta)
        {
            PanCamera_Keyboard(timeDelta);
            if (Input.GetKey(KeyCode.Space))
            {
                camFocus.transform.position = game.Get_PlayerLocation();
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                inputState = InputState.attack;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                game.OrderCharacter(order_Types.stop, null);
            }
        }


        private cursorTypes currentCursor;
        public void Update_Mouse(float timeDelta)
        {
             Rect rect = new Rect(0, 0, Screen.width, Screen.height);

             if (!rect.Contains(Input.mousePosition))
               return;
            //Change Mouse Cursor Based on State
            ChangeCursor(cursorTypes.select);
            PanCamera_Mouse(timeDelta);

            //RightClick to Move
            if (MouseInput.rightMouse_Down)
            {
                // 1 << 9 = Ignore all but 9
                // ~(1<<9) = Ignore only 9, ~ = Complement.
                int mask = (1 << 8); //Ignore all Layers except the CollisionPlane Layer

                //RayCast the Hit with the plane
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit, mask))
                {
                    moveArrow.transform.position = hit.point;
                    moveArrow.GetComponent<Animation>().Rewind("Play");
                    moveArrow.GetComponent<Animation>().Play("Play", PlayMode.StopAll);
                   
                    //Order the Character!
                    if (inputState == InputState.attack)
                    {
                        arrowMat.SetColor("_EmisColor", Colors.RED);
                        game.OrderCharacter(order_Types.attackMove, hit.point);
                        inputState = InputState.normal;
                    }
                    else
                    {
                        arrowMat.SetColor("_EmisColor", Colors.BLUE);
                        game.OrderCharacter(order_Types.move, hit.point);
                    }
                }
            }
           if (MouseInput.leftMouse_Down && inputState == InputState.attack)
                inputState = InputState.normal;

        }

        public void LateUpdate(float timeDelta)
        {


        }

        private float camSpeed = 10;
        void PanCamera_Mouse(float timeDelta)
        {
            Vector3 direction = Vector3.zero;
            //Check Bottom
            if (Input.mousePosition.y < Screen.height * 0.2f)
            { 
                direction -= camFocus.transform.forward;
                ChangeCursor(cursorTypes.pan_down);
            }
            //Check Top
            if (Input.mousePosition.y > Screen.height - Screen.height * 0.2f)
            {
                direction += camFocus.transform.forward;
                ChangeCursor(cursorTypes.pan_up);
            }
            //Check Right
            if (Input.mousePosition.x < Screen.width * 0.05f)
            {
                direction -= camFocus.transform.right;
                ChangeCursor(cursorTypes.pan_left);
            }
            //Check Left
            if (Input.mousePosition.x > Screen.width - Screen.width * 0.05f)
            {
                direction += camFocus.transform.right;
                ChangeCursor(cursorTypes.pan_right);
            }
            direction *= camSpeed * timeDelta;
            camFocus.transform.position += direction;
            if (direction != Vector3.zero)
            {
                camState = CamState.panning;
            }
        }
        void PanCamera_Keyboard(float timeDelta)
        {
            Vector3 direction = Vector3.zero;
            //Check Bottom
            if (Input.GetKey(KeyCode.DownArrow))
            {
                direction -= camFocus.transform.forward;
            }
            //Check Top
            if (Input.GetKey(KeyCode.UpArrow))
            {
                direction += camFocus.transform.forward;
            }
            //Check Right
            if (Input.GetKey(KeyCode.RightArrow))
            {
                direction += camFocus.transform.right;
            }
            //Check Left
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                direction -= camFocus.transform.right;
            }
            direction *= camSpeed * timeDelta;
            camFocus.transform.position += direction;
            if (direction != Vector3.zero)
            {
                camState = CamState.panning;
            }
        }


        void ChangeCursor(cursorTypes cursor)
        {
            if(currentCursor!= cursor)
                Cursor.SetCursor(ResourceManager.Get_MouseCursor(cursor), Vector2.zero, CursorMode.ForceSoftware);
            currentCursor = cursor;
        }
    }
}
