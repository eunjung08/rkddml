using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eunjung
{
    public class FollowCamera : MonoBehaviour
    {
        Transform target;
        Transform battlePostion;
        public float smoothSpeed = 0.125f;
        public Vector3 offset;
        BattleSystem battleSystem;

        void Start()
        {
        }

        public void Set()
        {
            target = GameObject.Find("Player(Clone)").transform;
            battleSystem = GameObject.Find("BattleSystem").GetComponent<BattleSystem>();
        }
        void LateUpdate()
        {
            //battlePostion = GameObject.Find("CameraPostion").transform;
            //if (battleSystem.status == BattleStatus.MAP)
            //{
                Vector3 desiredPos = target.position + offset;
                Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
                this.transform.position = smoothedPos;
                this.transform.LookAt(target);
            //}
            //else
            //{
            //    Vector3 desiredPos = battlePostion.position;
            //    Quaternion quaternion = battlePostion.rotation;
            //    this.transform.position = desiredPos;
            //    this.transform.rotation = quaternion;
            //}
        }
    }
}