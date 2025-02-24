using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eunjung
{
    public class Unit : MonoBehaviour
    {
        /// <summary>
        /// UI에 표시될 유닛 이름
        /// </summary>
        public string unitName;
        /// <summary>
        /// 최대 체력
        /// </summary>
        public int maxHP=3;
        /// <summary>
        /// 현재 체력
        /// </summary>
        public int currentHP;
        /// <summary>
        /// 현재 공격력
        /// </summary>
        public int attackDmg=1;
        /// <summary>
        /// 현재 스킬 공격력
        /// </summary>
        public int skillDmg=2;
        /// <summary>
        /// 턴 순서에 영향을 미치는 속도
        /// </summary>
        public int speed;
        /// <summary>
        /// 죽었는지 확인
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
