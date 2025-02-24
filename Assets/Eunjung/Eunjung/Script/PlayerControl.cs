using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Eunjung
{
    public class PlayerControl : MonoBehaviour
    {
        Rigidbody rigid;
        public Transform playerResponse;
        GameObject objPlayer;
        Animator animator;
        // Start is called before the first frame update
        private void Awake()
        {
            objPlayer = Resources.Load<GameObject>("Prefabs/Player");
        }
        void Start()
        {
        }
        void Update()
        {
        }
        public void CreatePlayers(int num)
        {
            for(int i = 0; i < num; i++)
            {
                GameObject Player = Instantiate(objPlayer, new Vector3(-3 + (i * 1.5f), 0, 0), new Quaternion(0f, 180f, 0f, 0f));
                //GameObject objPlayer = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Player.name = "Player " + i.ToString();
                Player.tag = "Player";
                Player.AddComponent<Unit>().unitName = "P" + i;
                animator = Player.GetComponent<Animator>();
                //objPlayer.transform.position = new Vector3(-3 + (i * 1.5f), 0, 0);
            }
        }
    }
}
