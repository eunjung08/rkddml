using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eunjung
{
    public class Enemy : MonoBehaviour
    {
        public GameManager gameManager;

        private void Awake()
        {
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                GameObject.Find("GameManager").GetComponent<GameManager>().Test();
            }
        }
    }
}
