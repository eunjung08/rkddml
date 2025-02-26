using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace Eunjung
{
    public class PlayerControl : MonoBehaviour
    {
        CharacterController characterController;
        public float speed = 10f;
        bool isWalk = false;
        Rigidbody rigid;
        Transform playerResponse;
        GameObject objPlayer;
        GameObject objPlayers;
        GameObject Player;
        Animator animator;
        bool isStop = false;
        BattleSystem battleSystem;
        // Start is called before the first frame update
        private void Awake()
        {
            objPlayer = Resources.Load<GameObject>("Prefabs/Player");
            objPlayers = Resources.Load<GameObject>("Prefabs/Players");
            playerResponse = GameObject.Find("Spawn_p").transform;
            CreatePlayer();
        }
        void Start()
        {
            battleSystem = GameObject.Find("BattleSystem").GetComponent<BattleSystem>();
        }
        void Update()
        {
            if (battleSystem.status != BattleStatus.MAP)
            {
                isStop = true;
            }
            if (!isStop)
            {
                Walk();
                Rotation();
            }
        }
        public void CreatePlayer()
        {
            Player = Instantiate(objPlayer, playerResponse.position, playerResponse.transform.rotation);
            characterController = Player.GetComponent<CharacterController>();
            animator = Player.GetComponent<Animator>();
        }

        void Walk()
        {
            isWalk = false;

            if (Input.GetKey(KeyCode.W))
            {
                characterController.Move(Player.transform.forward * Time.deltaTime * speed);
                isWalk = true;
            }
            if (Input.GetKey(KeyCode.S))
            {
                characterController.Move(-Player.transform.forward * Time.deltaTime * speed);
                isWalk = true;
            }
            if (Input.GetKey(KeyCode.D))
            {
                characterController.Move(Player.transform.right * Time.deltaTime * speed);
                isWalk = true;
            }
            if (Input.GetKey(KeyCode.A))
            {
                characterController.Move(-Player.transform.right * Time.deltaTime * speed);
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
        }

        void Rotation()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Plane plane = new Plane(Vector3.up, Vector3.zero);

            float rayLength;
            if (plane.Raycast(ray, out rayLength))
            {
                Vector3 mousePoint = ray.GetPoint(rayLength);

                Player.transform.LookAt(new Vector3(mousePoint.x, Player.transform.position.y, mousePoint.z));
            }
        }
        public void CreatePlayers(int num)
        {
            for(int i = 0; i < num; i++)
            {
                GameObject Players = Instantiate(objPlayers, new Vector3(-3 + (i * 1.5f), 0, 0), new Quaternion(0f, 180f, 0f, 0f));
                //GameObject objPlayer = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Players.name = "Player " + i.ToString();
                Players.tag = "Players";
                Players.AddComponent<Unit>().unitName = "P" + i;
                animator = Players.GetComponent<Animator>();
                //objPlayer.transform.position = new Vector3(-3 + (i * 1.5f), 0, 0);
            }
        }
    }
}
