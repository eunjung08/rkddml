using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eunjung
{
    public class EnemyControl : MonoBehaviour
    {
        GameObject objEnemy;
        Animator animator;
        void Awake()
        {
            objEnemy = Resources.Load<GameObject>("Prefab/Enemy");
        }
        void Start()
        {
        }
        public void CreateEnemy(int num)
        {
            for (int i = 0; i < num; i++)
            {
                GameObject Enemy = Instantiate(objEnemy, new Vector3((i * 1.5f), -2, 0), new Quaternion(0f,180f,0f,0f));
                //GameObject objEnemy = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Enemy.name = "Enemy " + i.ToString();
                Enemy.tag = "Enemy";
                Enemy.AddComponent<Unit>().unitName = "E" + i;
                animator = Enemy.GetComponent<Animator>();
                //objEnemy.transform.position = new Vector3((i * 1.5f), -2, 0);
            }
        }
    }
}
