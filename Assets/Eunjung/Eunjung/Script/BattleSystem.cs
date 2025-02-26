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
        /// ��Ʋ�ý��� ���°�
        /// </summary>
        public BattleStatus status;
        /// <summary>
        /// �÷��̾��� ���ֵ�
        /// </summary>
        public List<Unit> players = new List<Unit>();
        /// <summary>
        /// ���� ���ֵ�
        /// </summary>
        public List<Unit> enemys = new List<Unit>();
        /// <summary>
        /// ��� ���ֵ�
        /// </summary>
        public List<Unit> units = new List<Unit>();
        /// <summary>
        /// ���ֵ��� ���� ����
        /// </summary>
        private List<Unit> turnQueue;
        /// <summary>
        /// ������ �ൿ ����
        /// </summary>
        private Dictionary<Unit, int> unitActions;
        /// <summary>
        /// UI ��ü ��Ʈ��
        /// </summary>
        public UIControl uIControl;
        /// <summary>
        /// ���� ������ ���� �÷��̾� ����
        /// </summary>
        private Unit currentPlayerUnit;
        /// <summary>
        /// ���ݽ� ���õ� ��
        /// </summary>
        private Unit selectedTarget;
        /// <summary>
        /// Ÿ���� �����ϴ� �������� Ȯ��
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
            // ��������
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
                Debug.LogError("Player�� ���� ���� �ʽ��ϴ�.");
            }
            if(enemys.Count < 1)
            {
                Debug.LogError("Enemy�� �������� �ʽ��ϴ�");
            }

            uIControl.CreateCanvas();
            uIControl.CreateHPUI();
            uIControl.SetTextIng("�߻��� ���͵�� �����ƴ�.");

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
            //���� �ʴ� ���ֵ��� ã�� ���� �ӵ� �������� ����
            turnQueue = turnQueue.Where(unit => ! unit.isDead).OrderByDescending(unit => unit.speed).ToList();
        }

        public void EndTurn()
        {
            ProcessTurn();
        }

        /// <summary>
        /// �� ������ �ӵ� ���� ��� ������ �ӵ��� ������ �ൿ�Ѵ�.
        /// (�ӵ��� ���� ���̳��� ������ ���Ͽ� ������ ���� ����)
        /// </summary>
        /// <param name="unit"></param>
        //void AddUnitToQueue(Unit unit)
        //{
        //    //�� �ӵ��� �ְ� �ӵ������� ������
        //    int actions = Mathf.Max(1, unit.speed / GetFastestSpeed());
        //    unitActions[unit] = actions;
        //    for (int i = 0; i< actions; i++)
        //    {
        //        turnQueue.Enqueue(unit);
        //    }
        //    RemoveDeadUnitsFromQueue();
        //}

        /// <summary>
        /// ���� �ٸ� �ӵ��� ���� Unit�� �ӵ��� �޾ƿ´�.
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

            //�÷��̾�鿡�� ���ٽ��� ���� ���� ������ �ִ��� Ȯ���Ѵ�.
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
            uIControl.SetTextIng(unit.unitName + "�� ��!");

            yield return new WaitForSeconds(1.0f);

            Unit target = players[Random.Range(0, players.Count)].GetComponent<Unit>();
            int count = 0;
            while (target.isDead)
            {
                target = players[Random.Range(0, players.Count)].GetComponent<Unit>();
                count++;
                if(count > 100)
                {
                    Debug.LogError("100�� �̻� �ݺ��� ��ŭ ������ ����");
                    break;
                }
            }
            bool isDead = target.Damage(unit.attackDmg);

            animator = unit.GetComponent<Animator>();
            //renderes = target.GetComponentsInChildren<Renderer>();
            //originColor = renderes[0].material.color;
            animator.SetTrigger("isAttack");
            uIControl.SetTextIng(unit.unitName + "�� " + target.unitName + "�� ����!");
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
                uIControl.SetTextIng(target.unitName + "�� ���!");
                target.isDead = true;
                target.GetComponent<CapsuleCollider>().enabled = false;
                target.GetComponent<Animator>().SetTrigger("Death");
                turnQueue.RemoveAll(unit => unit.isDead);
                //RemoveDeadUnitsFromQueue();

                //Invoke("ProcessTurn", 1.0f);
            }
            uIControl.SetPlayerHPUI(target);
            //�¸� üũ
            if (!players.Exists(x => !x.isDead))
            {
                uIControl.SetTextIng("���� �¸��Ͽ����ϴ�.");
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
            uIControl.SetTextTurnOrder("���� �� : " + string.Join(" -> ", turnOrder));
        }

        /// <summary>
        /// �÷��̾ ������ Ÿ�� ����
        /// </summary>
        /// <param name="target"></param>
        void SelectTarget(Unit target)
        {
            selectedTarget = target;
            StartCoroutine(PlayerAttack()); 
        }

        /// <summary>
        /// �÷��̾ ����
        /// </summary>
        /// <returns></returns>
        IEnumerator PlayerAttack()
        {
            animator = currentPlayerUnit.GetComponent<Animator>();
            //renderes = selectedTarget.GetComponentsInChildren<Renderer>();
            //originColor = renderes[0].material.color;
            bool isDead = selectedTarget.Damage(currentPlayerUnit.attackDmg);
            animator.SetTrigger("isAttack");
            uIControl.SetTextIng(currentPlayerUnit.unitName + "�� " + selectedTarget.unitName + "�� ����!");
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
                uIControl.SetTextIng(selectedTarget.unitName + "�� ���!");
                selectedTarget.GetComponent<CapsuleCollider>().enabled = false;
                selectedTarget.GetComponent<Animator>().SetTrigger("Death");
                turnQueue.RemoveAll(unit => unit.isDead);
                //RemoveDeadUnitsFromQueue();
                //Invoke("ProcessTurn", 1.0f);
            }
            uIControl.SetEnemyHPUI(selectedTarget);
            //�¸� üũ
            if (!enemys.Exists(x => !x.isDead))
            {
                uIControl.SetTextIng("�÷��̾ �¸��Ͽ����ϴ�.");
            }
            else
            {
                ProcessTurn();
            }
        }
        /// <summary>
        /// �׾����� �Ǵ�
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
                        // Raycast�� �浹�� ������Ʈ ���
                        // Debug.Log("Ŭ���� ������Ʈ: " + hit.collider.gameObject.name);
                        if (hit.transform.CompareTag("Enemys"))
                            //if(hit.transform.tag==("Enmeys")) :������ CompareTag �ӵ��� �� ����
                        {
                            SelectTarget(hit.collider.GetComponent<Unit>());
                        }
                    }
                }
            }
        }
    }
}
