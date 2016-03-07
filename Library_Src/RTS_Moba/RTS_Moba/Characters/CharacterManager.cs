using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace RTS_Moba.Characters
{
    public enum order_Types
    {
        move,
        attackMove,
        attackTarget,
        huntMove,
        castAbility_1,
        castAbility_2,
        castAbility_3,
        castAbility_4,
        stop
    }
    class CharacterManager
    {
        #region Singleton Constructor
        //Singleton Constructor
        private static readonly CharacterManager _instance = new CharacterManager();

        public static CharacterManager instance
        {
            get
            {
                return _instance;
            }
        }

        private CharacterManager()
        {
            _characters = new List<Character>();
        }
        #endregion

        private List<Character> _characters;

        public void AddCharacter(Character character)
        {
            for (int i = 0; i < _characters.Count; i++)
                if (_characters[i] == character)
                    return;

            _characters.Add(character);
        }
        public void RemoveCharacter(Character character)
        {
            //for (int i = 0; i < _characters.Count; i++)
              //  if (_characters[i] == character)
                    _characters.Remove(character);
        }
        public void Update(float timeDelta)
        {
            for (int i = 0; i < _characters.Count; i++)
                _characters[i].Update(timeDelta);
        }

        public List<Character> characters
        {
            get
            {
                List<Character> list = new List<Character>();
                list.AddRange(_characters);
                return list;
            }
        }

        public Character Get_CharAt(int x, int y)
        {
            int px = 0;
            int py = 0;
            for (int i = 0; i < _characters.Count; i++)
            {
                px = (int)_characters[i].Get_Location().x;
                py = (int)_characters[i].Get_Location().z;

                if (px == x && py == y)
                    return _characters[i];
            }
            return null;
        }

    }
}
