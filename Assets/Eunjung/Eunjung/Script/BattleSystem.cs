using Codice.Client.Common.ProcessTree;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json.Bson;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
        public List<Unit> players = new List<Unit>();
        /// <summary>
        /// 적의 유닛들
        /// </summary>
        public List<Unit> enemys = new List<Unit>();
        /// <summary>
        /// 모든 유닛들
        /// </summary>
        public List<Unit> units = new List<Unit>();
        /// <summary>
        /// 유닛들의 순번 저장
        /// </summary>
        private List<Unit> turnQueue;
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
        //Renderer[] renderes;
        //Color originColor;

        private void Awake()
        {
            status = BattleStatus.MAP;
            Debug.Log(status);
            uIControl = GameObject.Find("UIControl").GetComponent<UIControl>();
            playerControl = GameObject.Find("PlayerContorl").GetComponent<PlayerControl>();
            enemyControl = GameObject.Find("EnemyContorl").GetComponent<EnemyControl>();
        }
        // Start is called before the first frame update
        void Start()
        {
        }
        public void StartsBattle()
        {            
            // 전투시작
            status = BattleStatus.START;
            StartCoroutine(SetBattle());
        }

        IEnumerator SetBattle()
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Players"))
            {
                players.Add(obj.GetComponent<Unit>());
                units.Add(obj.GetComponent<Unit>());
            }
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemys"))
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

            uIControl.CreateCanvas();
            uIControl.CreateHPUI();
            uIControl.SetTextIng("야생의 몬스터들과 마주쳤다.");

            yield return new WaitForSeconds(1.0f);
            SetTurnQueue();
            SetTurnUI();
            ProcessTurn();
        }

        void SetTurnQueue()
        {
            turnQueue= new List<Unit>(units);
            SortTurnQueueBySpeed();
            //unitActions = new Dictionary<Unit, int>();

            //foreach (Unit unit in units)
            //{
            //    AddUnitToQueue(unit);
            //}
        }
        
        void SortTurnQueueBySpeed()
        {
            //죽지 않는 유닛들을 찾고 빠른 속도 유닛으로 정렬
            turnQueue = turnQueue.Where(unit => ! unit.isDead).OrderByDescending(unit => unit.speed).ToList();
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
        //void AddUnitToQueue(Unit unit)
        //{
        //    //내 속도가 최고 속도값으로 나눠서
        //    int actions = Mathf.Max(1, unit.speed / GetFastestSpeed());
        //    unitActions[unit] = actions;
        //    for (int i = 0; i< actions; i++)
        //    {
        //        turnQueue.Enqueue(unit);
        //    }
        //    RemoveDeadUnitsFromQueue();
        //}

        /// <summary>
        /// 가장 바른 속도를 가진 Unit의 속도를 받아온다.
        /// </summary>
        /// <returns></returns>
        //int GetFastestSpeed()
        //{
        //    int maxSpeed = 1;
        //    foreach (Unit unit in units)
        //    {
        //        if(unit.speed>maxSpeed)
        //            maxSpeed = unit.speed;
        //    }
        //    return maxSpeed;
        //}

        void ProcessTurn()
        {
            SortTurnQueueBySpeed();
            if(turnQueue.Count == 0)
            {
                SetTurnQueue();
            }
            SetTurnUI();
            //currentPlayerUnit = turnQueue.Dequeue();
            currentPlayerUnit = turnQueue[0];
            turnQueue.RemoveAt(0);

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
            int count = 0;
            while (target.isDead)
            {
                target = players[Random.Range(0, players.Count)].GetComponent<Unit>();
                count++;
                if(count > 100)
                {
                    Debug.LogError("100번 이상 반복할 만큼 문제가 생김");
                    break;
                }
            }
            bool isDead = target.Damage(unit.attackDmg);

            animator = unit.GetComponent<Animator>();
            //renderes = target.GetComponentsInChildren<Renderer>();
            //originColor = renderes[0].material.color;
            animator.SetTrigger("isAttack");
            uIControl.SetTextIng(unit.unitName + "가 " + target.unitName + "를 공격!");
            //foreach (Renderer render in renderes)
            //{
            //    render.material.color = Color.red;
            //}
            //yield return new WaitForSeconds(0.5f);
            //foreach (Renderer render in renderes)
            //{
            //    render.material.color = originColor;
            //}
            yield return new WaitForSeconds(1f);
            if (isDead)
            {
                uIControl.SetTextIng(target.unitName + "가 사망!");
                target.isDead = true;
                target.GetComponent<CapsuleCollider>().enabled = false;
                target.GetComponent<Animator>().SetTrigger("Death");
                turnQueue.RemoveAll(unit => unit.isDead);
                //RemoveDeadUnitsFromQueue();

                //Invoke("ProcessTurn", 1.0f);
            }
            uIControl.SetPlayerHPUI(target);
            //승리 체크
            if (!players.Exists(x => !x.isDead))
            {
                uIControl.SetTextIng("적이 승리하였습니다.");
            }
            else
            {
                ProcessTurn();
            }
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
            //renderes = selectedTarget.GetComponentsInChildren<Renderer>();
            //originColor = renderes[0].material.color;
            bool isDead = selectedTarget.Damage(currentPlayerUnit.attackDmg);
            animator.SetTrigger("isAttack");
            uIControl.SetTextIng(currentPlayerUnit.unitName + "가 " + selectedTarget.unitName + "를 공격!");
            //foreach (Renderer render in renderes)
            //{
            //    render.material.color = Color.red;
            //}
            //yield return new WaitForSeconds(0.5f);
            //foreach (Renderer render in renderes)
            //{
            //    render.material.color = originColor;
            //}

            yield return new WaitForSeconds(1f);
            if (isDead)
            {
                uIControl.SetTextIng(selectedTarget.unitName + "가 사망!");
                selectedTarget.GetComponent<CapsuleCollider>().enabled = false;
                selectedTarget.GetComponent<Animator>().SetTrigger("Death");
                turnQueue.RemoveAll(unit => unit.isDead);
                //RemoveDeadUnitsFromQueue();
                //Invoke("ProcessTurn", 1.0f);
            }
            uIControl.SetEnemyHPUI(selectedTarget);
            //승리 체크
            if (!enemys.Exists(x => !x.isDead))
            {
                uIControl.SetTextIng("플레이어가 승리하였습니다.");
            }
            else
            {
                ProcessTurn();
            }
        }
        /// <summary>
        /// 죽었는지 판단
        /// </summary>
        //void RemoveDeadUnitsFromQueue()
        //{
        //    Queue<Unit> updatedQueue = new Queue<Unit>();
        //    foreach (Unit unit in turnQueue)
        //    {
        //        if (!unit.isDead)
        //        {
        //            updatedQueue.Enqueue(unit);
        //        }
        //    }

        //    turnQueue = updatedQueue;
        //}
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
                        if (hit.transform.CompareTag("Enemys"))
                            //if(hit.transform.tag==("Enmeys")) :같지만 CompareTag 속도가 더 빠름
                        {
                            SelectTarget(hit.collider.GetComponent<Unit>());
                        }
                    }
                }
            }
        }
    }
}
