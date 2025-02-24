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
        /// ��Ʋ�ý��� ���°�
        /// </summary>
        public BattleStatus status;
        /// <summary>
        /// �÷��̾��� ���ֵ�
        /// </summary>
        private List<Unit> players = new List<Unit>();
        /// <summary>
        /// ���� ���ֵ�
        /// </summary>
        private List<Unit> enemys = new List<Unit>();
        /// <summary>
        /// ��� ���ֵ�
        /// </summary>
        private List<Unit> units = new List<Unit>();
        /// <summary>
        /// ���ֵ��� ���� ����
        /// </summary>
        private Queue<Unit> turnQueue;
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
            // ��������
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
                Debug.LogError("Player�� ���� ���� �ʽ��ϴ�.");
            }
            if(enemys.Count < 1)
            {
                Debug.LogError("Enemy�� �������� �ʽ��ϴ�");
            }

            uIControl.SetTextIng("�߻��� ���͵�� �����ƴ�.");

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
        /// �� ������ �ӵ� ���� ��� ������ �ӵ��� ������ �ൿ�Ѵ�.
        /// (�ӵ��� ���� ���̳��� ������ ���Ͽ� ������ ���� ����)
        /// </summary>
        /// <param name="unit"></param>
        void AddUnitToQueue(Unit unit)
        {
            //�� �ӵ��� �ְ� �ӵ������� ������
            int actions = Mathf.Max(1, unit.speed / GetFastestSpeed());
            unitActions[unit] = actions;
            for (int i = 0; i< actions; i++)
            {
                turnQueue.Enqueue(unit);
            }
        }

        /// <summary>
        /// ���� �ٸ� �ӵ��� ���� Unit�� �ӵ��� �޾ƿ´�.
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
            bool isDead = target.Damage(unit.attackDmg);

            animator = unit.GetComponent<Animator>();
            renderes = target.GetComponentsInChildren<Renderer>();
            originColor = renderes[0].material.color;
            animator.SetTrigger("isAttack");
            uIControl.SetTextIng(unit.unitName + "�� " + target.unitName + "�� ����!");
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
                uIControl.SetTextIng(target.unitName + "�� ���!");
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
            renderes = selectedTarget.GetComponentsInChildren<Renderer>();
            originColor = renderes[0].material.color;
            bool isDead = selectedTarget.Damage(currentPlayerUnit.attackDmg);
            animator.SetTrigger("isAttack");
            uIControl.SetTextIng(currentPlayerUnit.unitName + "�� " + selectedTarget.unitName + "�� ����!");
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
                uIControl.SetTextIng(selectedTarget.unitName + "�� ���!");
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
                        // Raycast�� �浹�� ������Ʈ ���
                        // Debug.Log("Ŭ���� ������Ʈ: " + hit.collider.gameObject.name);
                        SelectTarget(hit.collider.GetComponent<Unit>());
                    }
                }
            }
        }
    }
}
