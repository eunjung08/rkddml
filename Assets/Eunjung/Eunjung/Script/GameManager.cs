using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eunjung
{
    public class GameManager : MonoBehaviour
    {
        public BattleStatus status;
        UIControl uIControl;
        PlayerControl playerControl;
        EnemyControl enemyControl;
        BattleSystem battleSystem;
        public int playerNum = 4;
        public int enemyNum = 4;
        // Start is called before the first frame update
        void Start()
        {
            GameObject objUIControl = new GameObject("UIControl");
            uIControl = objUIControl.AddComponent<UIControl>();

            GameObject objPlayerControl = new GameObject("PlayerContorl");
            playerControl = objPlayerControl.AddComponent<PlayerControl>();

            GameObject objEnemyControl = new GameObject("EnemyContorl");
            enemyControl = objEnemyControl.AddComponent<EnemyControl>();

            GameObject objBattleSystem = new GameObject("BattleSystem");
            battleSystem = objBattleSystem.AddComponent<BattleSystem>();

            //battleSystem.StartBattle();
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void BattleStart()
        {
            playerControl.CreatePlayers(4);

            enemyControl.CreateEnemys(4);

            battleSystem.StartsBattle();
        }
    }
}
