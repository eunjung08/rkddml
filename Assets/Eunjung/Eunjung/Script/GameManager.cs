using Codice.CM.Common;
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

        GameObject objPlayer;
        GameObject Player;
        CharacterController characterController;
        bool isWalk = false;
        Animator animator;
        public float moveSpeed = 10f;
        private void Awake()
        {
            objPlayer = Resources.Load<GameObject>("Prefabs/Player");
            GameObject objUIControl = new GameObject("UIControl");
            uIControl = objUIControl.AddComponent<UIControl>();

            GameObject objPlayerControl = new GameObject("PlayerContorl");
            playerControl = objPlayerControl.AddComponent<PlayerControl>();

            GameObject objEnemyControl = new GameObject("EnemyContorl");
            enemyControl = objEnemyControl.AddComponent<EnemyControl>();

            GameObject objBattleSystem = new GameObject("BattleSystem");
            battleSystem = objBattleSystem.AddComponent<BattleSystem>();
        }
        // Start is called before the first frame update
        void Start()
        {
            characterController = Player.GetComponent<CharacterController>();
            animator = Player.GetComponent<Animator>();
            //battleSystem.StartBattle();
        }

        // Update is called once per frame
        void Update()
        {
            PlayerMove();
        }

        void PlayerMove()
        {
            isWalk = false;

            if (Input.GetKey(KeyCode.W))
            {
                characterController.Move(Player.transform.forward * Time.deltaTime * moveSpeed);
                isWalk = true;
            }
            if (Input.GetKey(KeyCode.S))
            {
                characterController.Move(-Player.transform.forward * Time.deltaTime * moveSpeed);
                isWalk = true;
            }
            if (Input.GetKey(KeyCode.D))
            {
                characterController.Move(Player.transform.right * Time.deltaTime * moveSpeed);
                isWalk = true;
            }
            if (Input.GetKey(KeyCode.A))
            {
                characterController.Move(-Player.transform.right * Time.deltaTime * moveSpeed);
                isWalk = true;
            }
            if (isWalk)
            {
                animator.SetBool("isWalk", true);
            }
            else
            {
                animator.SetBool("isWalk", false);
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Plane plane = new Plane(Vector3.up, Vector3.zero);

            float rayLength;
            if (plane.Raycast(ray, out rayLength))
            {
                Vector3 mousePoint = ray.GetPoint(rayLength);
                Player.transform.LookAt(new Vector3(mousePoint.x, Player.transform.position.y, mousePoint.z));
            }
        }

        public void BattleStart()
        {
            playerControl.CreatePlayers(4);

            enemyControl.CreateEnemys(4);

            battleSystem.StartsBattle();
        }

        public GameObject CrateMovePlayer()
        {
            Player = Instantiate(objPlayer);
            return Player;
        }

        //public GameObject CrateMoveEnemy()
        //{

        //}
    }
}
