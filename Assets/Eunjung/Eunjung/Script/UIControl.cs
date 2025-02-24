using System.Collections;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json.Bson;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Eunjung
{
    public class UIControl : MonoBehaviour
    {
        [SerializeField]
        private Text TextIng;
        public Button BtnAttack;
        public Button BtnSkill;
        public Button BtnEndTurn;
        [SerializeField]
        private Text TextTurnOrder;
        /// <summary>
        /// 페이드 작업시 시간
        /// </summary>
        //private float fadeTime = 1.0f;

        Canvas canvas;
        BattleSystem battleSystem;
        // Start is called before the first frame update
        void Start()
        {
            battleSystem = GameObject.Find("BattleSystem").GetComponent<BattleSystem>();
            CreateCanvas();
        }

        public void CreateCanvas()
        {
            //캔버스 생성
            GameObject objCanvas = new GameObject("Canvas");
            canvas = objCanvas.AddComponent<Canvas>();
            objCanvas.AddComponent<CanvasScaler>();
            objCanvas.AddComponent<GraphicRaycaster>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            //텍스트에 사용될 폰트 지정
            Font myFont = (Font)Resources.GetBuiltinResource(typeof(Font), "LegacyRuntime.ttf");

            //UI 입력을 위한 이벤트 시스템
            GameObject objEventSystem = new GameObject("EventSystem");
            objEventSystem.AddComponent<EventSystem>();
            objEventSystem.AddComponent<StandaloneInputModule>();

            //상태 UI
            GameObject objTextIng = new GameObject("TextIng");
            objTextIng.transform.parent = canvas.transform;
            TextIng = objTextIng.AddComponent<Text>();
            TextIng.font = myFont;
            TextIng.text = "";
            TextIng.fontSize = 20;
            TextIng.alignment = TextAnchor.MiddleCenter;
            RectTransform RtTextIng = objTextIng.GetComponent<RectTransform>();
            RtTextIng.anchorMin = new Vector2(0, 1);
            RtTextIng.anchorMax = new Vector2(1, 1);
            RtTextIng.anchoredPosition = new Vector2(0, -(RtTextIng.sizeDelta.y / 2));
            RtTextIng.offsetMin = new Vector2(0, RtTextIng.offsetMin.y);
            RtTextIng.offsetMax = new Vector2(0, RtTextIng.offsetMax.y);
            RtTextIng.pivot = new Vector2(0.5f, 0.5f);

            //턴 UI
            GameObject objTextTurnOrder = new GameObject("TextTurnOrder");
            objTextTurnOrder.transform.parent = canvas.transform;
            TextTurnOrder = objTextTurnOrder.AddComponent<Text>();
            TextTurnOrder.font = myFont;
            TextTurnOrder.text = "현재 상태";
            TextTurnOrder.fontSize = 20;
            TextTurnOrder.alignment = TextAnchor.MiddleCenter;
            RectTransform RtTextTurnOrder = objTextTurnOrder.GetComponent<RectTransform>();
            RtTextTurnOrder.anchorMin = new Vector2(0, 1);
            RtTextTurnOrder.anchorMax = new Vector2(1, 1);
            RtTextTurnOrder.anchoredPosition = new Vector2(0, -(RtTextTurnOrder.sizeDelta.y));
            RtTextTurnOrder.offsetMin = new Vector2(0, RtTextTurnOrder.offsetMin.y);
            RtTextTurnOrder.offsetMax = new Vector2(0, RtTextTurnOrder.offsetMax.y);
            RtTextTurnOrder.pivot = new Vector2(0.5f, 0.5f);

            GameObject objBtnAttack = new GameObject("BtnAttack");
            objBtnAttack.transform.parent = canvas.transform;
            objBtnAttack.AddComponent<Image>();
            BtnAttack = objBtnAttack.AddComponent<Button>();
            RectTransform RtBtnAttack = objBtnAttack.GetComponent<RectTransform>();
            RtBtnAttack.sizeDelta = new Vector2(200, 20);
            RtBtnAttack.anchoredPosition = new Vector2(0, 140);
            RtBtnAttack.pivot = new Vector2(0.5f, 0.5f);
            BtnAttack.onClick.AddListener(OnBtnAttack);
            GameObject objBtnAttackText = new GameObject("BtnAtackText");
            objBtnAttackText.transform.SetParent(objBtnAttack.transform);
            Text BtnAttackText = objBtnAttackText.AddComponent<Text>();
            BtnAttackText.text = "BtnAttackText";
            BtnAttackText.font = myFont;
            BtnAttackText.fontSize = 18;
            BtnAttackText.color = Color.black;
            BtnAttackText.alignment = TextAnchor.MiddleCenter;
            RectTransform rtBtnAttattackText = objBtnAttackText.GetComponent<RectTransform>();
            rtBtnAttattackText.anchorMin = new Vector2(0, 0);
            rtBtnAttattackText.anchorMax = new Vector2(1, 1);
            rtBtnAttattackText.anchoredPosition = Vector2.zero;
            rtBtnAttattackText.sizeDelta = new Vector2(0, 0);

            GameObject objBtnSkill = new GameObject("BtnSkill");
            objBtnSkill.transform.parent = canvas.transform;
            objBtnSkill.AddComponent<Image>();
            BtnSkill = objBtnSkill.AddComponent<Button>();
            RectTransform RtBtnSkill = objBtnSkill.GetComponent<RectTransform>();
            RtBtnSkill.sizeDelta = new Vector2(200, 20);
            RtBtnSkill.anchoredPosition = new Vector2(0, 100);
            RtBtnSkill.pivot = new Vector2(0.5f, 0.5f);
            BtnSkill.onClick.AddListener(OnBtnSkill);
            GameObject objBtnSkillText = new GameObject("BtnSkillText");
            objBtnSkillText.transform.SetParent(objBtnSkill.transform);
            Text BtnSkillText = objBtnSkillText.AddComponent<Text>();
            BtnSkillText.text = "BtnSkillText";
            BtnSkillText.font = myFont;
            BtnSkillText.fontSize = 18;
            BtnSkillText.color = Color.black;
            BtnSkillText.alignment = TextAnchor.MiddleCenter;
            RectTransform rtBtnAttSkillText = objBtnSkillText.GetComponent<RectTransform>();
            rtBtnAttSkillText.anchorMin = new Vector2(0, 0);
            rtBtnAttSkillText.anchorMax = new Vector2(1, 1);    
            rtBtnAttSkillText.anchoredPosition = Vector2.zero;
            rtBtnAttSkillText.sizeDelta = new Vector2(0, 0);

            GameObject objBtnEndTurn = new GameObject("BtnEndTurn");
            objBtnEndTurn.transform.parent = canvas.transform;
            objBtnEndTurn.AddComponent<Image>();
            BtnEndTurn = objBtnEndTurn.AddComponent<Button>();
            RectTransform RtBtnEndTurn = objBtnEndTurn.GetComponent<RectTransform>();
            RtBtnEndTurn.sizeDelta = new Vector2(200, 20);
            RtBtnEndTurn.anchoredPosition = new Vector2(0, 60);
            RtBtnEndTurn.pivot = new Vector2(0.5f, 0.5f);
            BtnEndTurn.onClick.AddListener(OnBtnEndTurn);
            GameObject objBtnEndTurnText = new GameObject("BtnEndTurnText");
            objBtnEndTurnText.transform.SetParent(objBtnEndTurn.transform);
            Text BtnEndTurnText = objBtnEndTurnText.AddComponent<Text>();
            BtnEndTurnText.text = "BtnEndTurnText";
            BtnEndTurnText.font = myFont;
            BtnEndTurnText.fontSize = 18;
            BtnEndTurnText.color = Color.black;
            BtnEndTurnText.alignment = TextAnchor.MiddleCenter;
            RectTransform rtBtnAttEndTurnText = objBtnEndTurnText.GetComponent<RectTransform>();
            rtBtnAttEndTurnText.anchorMin = new Vector2(0, 0);
            rtBtnAttEndTurnText.anchorMax = new Vector2(1, 1);
            rtBtnAttEndTurnText.anchoredPosition = Vector2.zero;
            rtBtnAttEndTurnText.sizeDelta = new Vector2(0, 0);
        }

        public void PlayerTurn(string unitName)
        {
            TextIng.text = unitName + "의 턴. 행동을 선택하세요.";
            //유니티 버튼의 활성화와 비활성화를 관리하는 변수
            BtnAttack.interactable = true;
            BtnSkill.interactable = true;
            BtnEndTurn.interactable = true;
        }
        public void EnemyTurn(string unitName)
        {
            TextIng.text = unitName + "의 턴. 행동을 선택하세요.";
            //유니티 버튼의 활성화와 비활성화를 관리하는 변수
            BtnAttack.interactable = false;
            BtnSkill.interactable = false;
            BtnEndTurn.interactable = false;
        }

        public void SetTextIng(string message)
        {
            TextIng.text = message;
        }

        public void SetTextTurnOrder(string message)
        {
            TextTurnOrder.text = message;
        }

        public void OnBtnAttack()
        {
            Debug.Log("공격 발동");
            if(battleSystem.status != BattleStatus.PLAYER_TURN)
            {
                return;
            }
            TextIng.text = "대상을 선택해주세요";
            //유니티 버튼의 활성화와 비활성화를 관리하는 변수
            BtnAttack.interactable = false;
            BtnSkill.interactable = false;
            BtnEndTurn.interactable = false;

           battleSystem.isTargeting = true;
        }

        public void OnBtnSkill()
        {
            Debug.Log("스킬 발동");
            if (battleSystem.status != BattleStatus.PLAYER_TURN)
            {
                return;
            }
            TextIng.text = "대상을 선택해주세요";
            //유니티 버튼의 활성화와 비활성화를 관리하는 변수
            BtnAttack.interactable = false;
            BtnSkill.interactable = false;
            BtnEndTurn.interactable = false;

            battleSystem.isTargeting = true;
        }

        public void OnBtnEndTurn()
        {
            Debug.Log("턴종료");
            battleSystem.EndTurn();
        }
    }
}
