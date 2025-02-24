using Codice.Client.Common.ProcessTree;
using System.Collections;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json.Bson;
using UnityEngine;

namespace Eunjung
{
    public class BattleSystem : MonoBehaviour
    {
        /// <summary>
        /// 배틀시스템 상태값
        /// </summary>
        public BattleStatus status;
        /// <summary>
        /// 플레이어의 유닛들
        /// </summary>
        private List<Unit> players = new List<Unit>();
        /// <summary>
        /// 적의 유닛들
        /// </summary>
        private List<Unit> enemys = new List<Unit>();
        /// <summary>
        /// 모든 유닛들
        /// </summary>
        private List<Unit> units = new List<Unit>();
        /// <summary>
        /// 유닛들의 순번 저장
        /// </summary>
        private Queue<Unit> turnQueue;
        /// <summary>
        /// 유닛의 행동 저장
        /// </summary>
        private Dictionary<Unit, int> unitActions;
        /// <summary>
        /// UI 전체 컨트롤
        /// </summary>
        public UIControl uIControl;
        /// <summary>
        /// 현재 움직일 턴인 플레이어 유닛
        /// </summary>
        private Unit currentPlayerUnit;
        /// <summary>
        /// 공격시 선택된 적
        /// </summary>
        private Unit selectedTarget;
        /// <summary>
        /// 타겟을 선택하는 도중인지 확인
        /// </summary>
        public bool isTargeting = false;

        PlayerControl playerControl;
        EnemyControl enemyControl;

        Animator animator;
        Renderer[] renderes;
        Color originColor;

        private void Awake()
        {
            uIControl = GameObject.Find("UIControl").GetComponent<UIControl>();
            playerControl = GameObject.Find("PlayerContorl").GetComponent<PlayerControl>();
            enemyControl = GameObject.Find("EnemyContorl").GetComponent<EnemyControl>();
        }
        // Start is called before the first frame update
        void Start()
        {
            // 전투시작
            status = BattleStatus.START;
            Invoke("StartBattle", 2.0f);
        }

        public void StartBattle()
        {
            StartCoroutine(SetBattle());
        }

        IEnumerator SetBattle()
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
            {
                players.Add(obj.GetComponent<Unit>());
                units.Add(obj.GetComponent<Unit>());
            }
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                enemys.Add(obj.GetComponent<Unit>());
                units.Add(obj.GetComponent<Unit>());
            }
            if(players.Count < 1)
            {
                Debug.LogError("Player가 존재 하지 않습니다.");
            }
            if(enemys.Count < 1)
            {
                Debug.LogError("Enemy가 존재하지 않습니다");
            }

            uIControl.SetTextIng("야생의 몬스터들과 마주쳤다.");

            yield return new WaitForSeconds(1.0f);
            SetTurnQueue();
            SetTurnUI();
            ProcessTurn();
        }

        void SetTurnQueue()
        {
            turnQueue= new Queue<Unit>();
            unitActions = new Dictionary<Unit, int>();

            foreach (Unit unit in units)
            {
                AddUnitToQueue(unit);
            }
        }

        public void EndTurn()
        {
            ProcessTurn();
        }

        /// <summary>
        /// 각 유닛의 속도 값을 상대 유닛의 속도로 나누어 행동한다.
        /// (속도가 많이 차이나는 유닛이 한턴에 여러번 공격 가능)
        /// </summary>
        /// <param name="unit"></param>
        void AddUnitToQueue(Unit unit)
        {
            //내 속도가 최고 속도값으로 나눠서
            int actions = Mathf.Max(1, unit.speed / GetFastestSpeed());
            unitActions[unit] = actions;
            for (int i = 0; i< actions; i++)
            {
                turnQueue.Enqueue(unit);
            }
        }

        /// <summary>
        /// 가장 바른 속도를 가진 Unit의 속도를 받아온다.
        /// </summary>
        /// <returns></returns>
        int GetFastestSpeed()
        {
            int maxSpeed = 1;
            foreach (Unit unit in units)
            {
                if(unit.speed>maxSpeed)
                    maxSpeed = unit.speed;
            }
            return maxSpeed;
        }

        void ProcessTurn()
        {
            if(turnQueue.Count == 0)
            {
                SetTurnQueue();
            }
           SetTurnUI();
            currentPlayerUnit = turnQueue.Dequeue();

            //플레이어들에서 람다식을 통해 현재 유닛이 있는지 확인한다.
            if(players.Exists(p => p.GetComponent<Unit>() == currentPlayerUnit))
            {
                status = BattleStatus.PLAYER_TURN;
                PlayerTurn(currentPlayerUnit);
            }
            else
            {
                status = BattleStatus.ENEMY_TURN;
                StartCoroutine(EnemyTurn(currentPlayerUnit));
            }
        }

        void PlayerTurn(Unit unit)
        {
            uIControl.PlayerTurn(unit.unitName);
        }

        IEnumerator EnemyTurn(Unit unit)
        {
            uIControl.SetTextIng(unit.unitName + "의 턴!");

            yield return new WaitForSeconds(1.0f);

            Unit target = players[Random.Range(0, players.Count)].GetComponent<Unit>();
            bool isDead = target.Damage(unit.attackDmg);

            animator = unit.GetComponent<Animator>();
            renderes = target.GetComponentsInChildren<Renderer>();
            originColor = renderes[0].material.color;
            animator.SetTrigger("isAttack");
            uIControl.SetTextIng(unit.unitName + "가 " + target.unitName + "를 공격!");
            foreach (Renderer render in renderes)
            {
                render.material.color = Color.red;
            }
            yield return new WaitForSeconds(0.5f);
            foreach (Renderer render in renderes)
            {
                render.material.color = originColor;
            }
            yield return new WaitForSeconds(1f);
            if (isDead)
            {
                uIControl.SetTextIng(target.unitName + "가 사망!");
                Destroy(target.gameObject);
            }
            Invoke("ProcessTurn", 1.0f);
        }

        void SetTurnUI()
        {
            List<string> turnOrder = new List<string>();
            foreach(Unit unit in turnQueue)
            {
                turnOrder.Add(unit.unitName);
            }
            uIControl.SetTextTurnOrder("현재 턴 : " + string.Join(" -> ", turnOrder));
        }

        /// <summary>
        /// 플레이어가 공격할 타겟 선정
        /// </summary>
        /// <param name="target"></param>
        void SelectTarget(Unit target)
        {
            selectedTarget = target;
            StartCoroutine(PlayerAttack()); 
        }

        /// <summary>
        /// 플레이어가 공격
        /// </summary>
        /// <returns></returns>
        IEnumerator PlayerAttack()
        {
            animator = currentPlayerUnit.GetComponent<Animator>();
            renderes = selectedTarget.GetComponentsInChildren<Renderer>();
            originColor = renderes[0].material.color;
            bool isDead = selectedTarget.Damage(currentPlayerUnit.attackDmg);
            animator.SetTrigger("isAttack");
            uIControl.SetTextIng(currentPlayerUnit.unitName + "가 " + selectedTarget.unitName + "를 공격!");
            foreach (Renderer render in renderes)
            {
                render.material.color = Color.red;
            }
            yield return new WaitForSeconds(0.5f);
            foreach (Renderer render in renderes)
            {
                render.material.color = originColor;
            }

            yield return new WaitForSeconds(1f);

            if (isDead)
            {
                uIControl.SetTextIng(selectedTarget.unitName + "가 사망!");
                Destroy(selectedTarget.gameObject);
                
            }

            Invoke("ProcessTurn", 1.0f);
        }

        void Update()
        {
            if (isTargeting)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if(Physics.Raycast(ray, out hit))
                    {
                        // Raycast가 충돌한 오브젝트 출력
                        // Debug.Log("클릭한 오브젝트: " + hit.collider.gameObject.name);
                        SelectTarget(hit.collider.GetComponent<Unit>());
                    }
                }
            }
        }
    }
}
