using UnityEngine;
using System.Collections;


namespace RTS_Moba
{
    /*
        //Singleton Reference

        //Singleton Constructor
        private static readonly StateManager _instance = new StateManager();
        private StateManager() { }
        public static StateManager instance
        {
            get
            {
                return _instance;
            }
        }

        */
    public class Game_Root : MonoBehaviour {


    private Game game;

	// Use this for initialization
	void Awake ()
    {
        game = new Game();
        ResourceManager.gameHUD = GameObject.Find("Pnl_GameHud");
	}
	
	// Update is called once per frame
	void Update ()
    {

        float timeDelta = Time.deltaTime;
        game.Update(timeDelta);
	}

    void LateUpdate()
    {
        float timeDelta = Time.deltaTime;
         game.LateUpdate(timeDelta);
    }


    }

}