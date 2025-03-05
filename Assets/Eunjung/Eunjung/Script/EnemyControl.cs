using Codice.CM.Common.Tree;
using System.Collections;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json.Bson;
using UnityEngine;
using UnityEngine.AI;

namespace Eunjung
{
    public class EnemyControl : MonoBehaviour
    {
        GameObject objEnemy;
        GameObject objEnemys;
        Animator animator;
        NavMeshAgent navMeshAgent;
        GameObject Enemy;
        bool isStop;
        Transform player;
        //public float navDistance = 5.0f;
        bool isCheck = false;
        GameManager gameManager;
        void Awake()
        {
            //objEnemy = Resources.Load<GameObject>("Prefabs/Enemy");
            objEnemys = Resources.Load<GameObject>("Prefabs/Enemys");
        }
        void Start()
        {
            //enemyResponse = GameObject.Find("Spawn_e").transform;
            //CreateEnemy();
        }
        //public void CreateEnemy()
        //{
        //    Enemy = Instantiate(objEnemy, enemyResponse.position, enemyResponse.transform.rotation);
        //    navMeshAgent = Enemy.GetComponent<NavMeshAgent>();
        //    player = GameObject.Find("Player(Clone)").transform;
        //    gameManager = FindObjectOfType<GameManager>();
        //}

        void Update()
        {
            //float distanceToPlayer = Vector3.Distance(Enemy.transform.position, player.position);
            //if (distanceToPlayer <= navDistance)
            //{

            //    navMeshAgent.enabled = false;
            //    if (gameManager != null && isCheck == false)
            //    {
            //        isCheck = true;
            //        //gameManager.BattleStart();
            //        Invoke("Time", 2.0f);
            //    }
            //}
        }

        //void Time()
        //{
        //    gameManager.BattleStart();
        //}
        //public void Check()
        //{
        //    float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        //    if (distanceToPlayer <= navDistance)
        //    {
        //        navMeshAgent.enabled = false;

        //        if (gameManager != null)
        //        {
        //            gameManager.BattleStart();
        //        }
        //    }
        //}
        public void CreateEnemys(int num)
        {
            for (int i = 0; i < num; i++)
            {
                GameObject Enemys = Instantiate(objEnemys, new Vector3((i * 1.5f), -2, 0), new Quaternion(0f,180f,0f,0f));
                //GameObject objEnemy = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Enemys.name = "Enemy " + i.ToString();
                Enemys.tag = "Enemys";
                Enemys.AddComponent<Unit>().unitName = "E" + i;
                animator = Enemys.GetComponent<Animator>();
                //objEnemy.transform.position = new Vector3((i * 1.5f), -2, 0);
            }
        }
    }
}
