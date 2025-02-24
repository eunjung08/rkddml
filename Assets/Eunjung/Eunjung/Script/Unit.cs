using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eunjung
{
    public class Unit : MonoBehaviour
    {
        /// <summary>
        /// UI�� ǥ�õ� ���� �̸�
        /// </summary>
        public string unitName;
        /// <summary>
        /// �ִ� ü��
        /// </summary>
        public int maxHP=3;
        /// <summary>
        /// ���� ü��
        /// </summary>
        public int currentHP;
        /// <summary>
        /// ���� ���ݷ�
        /// </summary>
        public int attackDmg=1;
        /// <summary>
        /// ���� ��ų ���ݷ�
        /// </summary>
        public int skillDmg=2;
        /// <summary>
        /// �� ������ ������ ��ġ�� �ӵ�
        /// </summary>
        public int speed;
        /// <summary>
        /// �׾����� Ȯ��
        /// </summary>
        public bool isDead = false;
        // Start is called before the first frame update
        void Start()
        {
            currentHP = maxHP;
        }

        // Update is called once per frame
        void Update()
        {
        }
        public bool Damage(int damage)
        {
            currentHP -= damage;
            if(currentHP <= 0)
            {
                currentHP = 0;
                isDead = true;
                return true;
            }
            return false;
        }
    }
}
