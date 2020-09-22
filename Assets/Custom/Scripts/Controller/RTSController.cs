using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RTS2D {

    enum ControllerState {
        Free,
        Fixed,
        SelectingArea,
        SelectingPoint
    }

    public class RTSController : MonoBehaviour
    {
        Camera m_Camera;
        LineRenderer m_SelectionRenderer;
        SpriteRenderer m_PointRenderer;

        [Header("SFX")]
        [SerializeField] AudioSource m_Audio;
        [SerializeField] AudioClip m_SelectionSFX;
        [SerializeField] List<AudioClip> m_CommandSFX;
        
        [Header("RTSPlayer")]
        [SerializeField] RTSPlayer m_Player;

        [Header("State")]
        [SerializeField] ControllerState m_State;

        [Header("Scroll")]
        [SerializeField] float m_Border;
        [SerializeField] float m_Step;

        [Header("Zoom")]
        [SerializeField] float m_ZoomStep;
        [SerializeField] float m_MinZoom;
        [SerializeField] float m_MaxZoom;

        [Header("Selection")]
        [SerializeField] string[] m_Layers;
        [SerializeField] Vector3 m_MouseScreenPosition;
        [SerializeField] Vector3 m_MouseWorldPosition;
        [SerializeField] Vector3 m_SelectionWorldPointA;
        [SerializeField] Vector3 m_SelectionWorldPointB;
        
        [Header("Building")]
        [SerializeField] GameObject m_BasePrefab;
        [SerializeField] GameObject m_TowerPrefab;
        [SerializeField] float m_BaseCost;
        [SerializeField] float m_TowerCost;

        [Header("BuildingFX")]
        [SerializeField] AudioClip m_BuildSFX;

        List<ISelectable> m_Selection;
        List<ISelectable[]> m_StoredSelections;
        bool m_SelectionChanged;
        delegate void CommandDelegate(Vector3 a_Point);
        CommandDelegate m_CommandDelegate;
        float m_PointRadius;
        bool m_OverlapPoint;
        bool m_CanBuildThere;
        string m_LastInputString;
        float m_TimeSinceLastInput;

        /// overrides //////////////////////////////////////////////////////////////////////

        void Start()
        {
            m_Camera = GetComponent<Camera>();
            m_SelectionRenderer = GetComponent<LineRenderer>();
            m_PointRenderer = GetComponentInChildren<SpriteRenderer>();
            m_Selection = new List<ISelectable>();
            m_SelectionChanged = false;
            m_StoredSelections = new List<ISelectable[]>();
            for(int i = 0; i < 10; i++) {
                m_StoredSelections.Add(null);
            }
        }

        void Update()
        {
            UpdateMousePosition();
            Command();
            UpdateSelection();
            if(m_State == ControllerState.Free) {
                HideSelectionArea();
                HidePointArea();
                UpdateCamera();
            }else if(m_State == ControllerState.SelectingArea) {
                DrawSelectionArea();
            }else if(m_State == ControllerState.SelectingPoint) {
                DrawPointArea();
                UpdateCamera();
            }
        }

        /// mouse position //////////////////////////////////////////////////////////////////////////////

        void UpdateMousePosition() {
            m_MouseScreenPosition = Input.mousePosition;
            Vector3 l_Position = m_Camera.ScreenToWorldPoint(Input.mousePosition);
            m_MouseWorldPosition = new Vector3(l_Position.x, l_Position.y, 0f);
            MouseOverUI();
        }

        bool MouseOverUI() {
            return EventSystem.current.IsPointerOverGameObject();
        }

        /// camera movement //////////////////////////////////////////////////////////////////////////////

        void UpdateCamera() {
            Move();
            Zoom();
        }

        void Move() {
            if(!MouseOverUI()) {
                Vector3 l_Horizontal = Horizontal() * Vector3.right * m_Step,
                    l_Vertical = Vertical() * Vector3.up * m_Step;
                m_Camera.transform.position += (l_Horizontal + l_Vertical) * Time.deltaTime;
            }
        }

        void Zoom() {
            m_Camera.orthographicSize +=  -Input.mouseScrollDelta.y * m_ZoomStep;
            m_Camera.orthographicSize = Mathf.Clamp(m_Camera.orthographicSize, m_MinZoom, m_MaxZoom);
        }

        int Horizontal() {
            if(m_MouseScreenPosition.x > (m_Camera.pixelWidth - m_Border))
                return 1;
            else if(m_MouseScreenPosition.x < m_Border) 
                return -1;
            return 0;
        }
        int Vertical() {
            if(m_MouseScreenPosition.y > (m_Camera.pixelHeight - m_Border))
                return 1;
            else if(m_MouseScreenPosition.y < m_Border) 
                return -1;
            return 0;
        }

        void JumpToSelection() {
            if(m_Selection.Count > 0) {
                try {
                    Vector3 l_SelectionPosition = ((Actor2D) m_Selection[0]).transform.GetChild(0).transform.position;
                    m_Camera.transform.position = new Vector3(l_SelectionPosition.x, l_SelectionPosition.y, m_Camera.transform.position.z);
                }catch {
                    Debug.Log("Ups!");
                }
            }
        }
                

        /// selection //////////////////////////////////////////////////////////////////////////////

        void UpdateSelection() {
            Select();
            StoreSelection();
            RestoreSelection();
        }

        void Select() {
            if(!MouseOverUI() && m_State == ControllerState.Free) {
                if(Input.GetButtonDown("Fire1")) {
                    m_SelectionWorldPointA = m_MouseWorldPosition;
                    m_State = ControllerState.SelectingArea;
                }
            }else if(m_State == ControllerState.SelectingPoint) {
                if(Input.GetButtonDown("Fire1")) {
                    if(m_CanBuildThere) m_CommandDelegate(m_MouseWorldPosition);
                    m_State = ControllerState.Free;
                }
                if(Input.GetButtonDown("Fire2")) {
                    m_State = ControllerState.Free;
                }
            }else if(m_State == ControllerState.SelectingArea) {
                if(Input.GetButtonUp("Fire1")) {
                    m_SelectionWorldPointB = m_MouseWorldPosition;
                    SelectArea(m_SelectionWorldPointA, m_SelectionWorldPointB);
                    m_State = ControllerState.Free;
                }
            }
        }

        void StoreSelection() {
            switch(Input.inputString) {
                case "!":  m_StoredSelections[1] = GetSelection().ToArray(); break;
                case "\"":  m_StoredSelections[2] = GetSelection().ToArray(); break;
                case "§":  m_StoredSelections[3] = GetSelection().ToArray(); break;
                case "$":  m_StoredSelections[4] = GetSelection().ToArray(); break;
                case "%":  m_StoredSelections[5] = GetSelection().ToArray(); break;
                case "&":  m_StoredSelections[6] = GetSelection().ToArray(); break;
                case "/":  m_StoredSelections[7] = GetSelection().ToArray(); break;
                case "(":  m_StoredSelections[8] = GetSelection().ToArray(); break;
                case ")":  m_StoredSelections[9] = GetSelection().ToArray(); break;
                case "=":  m_StoredSelections[0] = GetSelection().ToArray(); break;
            }
        }

        void RestoreSelection() {
            m_TimeSinceLastInput += Time.deltaTime;
            if(Input.inputString != "") {
                switch(Input.inputString) {
                    case "1":  if(m_StoredSelections[1] != null) SetSelection(m_StoredSelections[1]); DoublePressed(); break; 
                    case "2":  if(m_StoredSelections[2] != null) SetSelection(m_StoredSelections[2]); DoublePressed(); break;
                    case "3":  if(m_StoredSelections[3] != null) SetSelection(m_StoredSelections[3]); DoublePressed(); break;
                    case "4":  if(m_StoredSelections[4] != null) SetSelection(m_StoredSelections[4]); DoublePressed(); break;
                    case "5":  if(m_StoredSelections[5] != null) SetSelection(m_StoredSelections[5]); DoublePressed(); break;
                    case "6":  if(m_StoredSelections[6] != null) SetSelection(m_StoredSelections[6]); DoublePressed(); break;
                    case "7":  if(m_StoredSelections[7] != null) SetSelection(m_StoredSelections[7]); DoublePressed(); break;
                    case "8":  if(m_StoredSelections[8] != null) SetSelection(m_StoredSelections[8]); DoublePressed(); break;
                    case "9":  if(m_StoredSelections[9] != null) SetSelection(m_StoredSelections[9]); DoublePressed(); break;
                    case "0":  if(m_StoredSelections[0] != null) SetSelection(m_StoredSelections[0]); DoublePressed(); break;
                }
            }
        }

        void DoublePressed() {
            if(m_LastInputString == Input.inputString && m_TimeSinceLastInput < 1f) JumpToSelection();
            m_TimeSinceLastInput = 0;
            m_LastInputString = Input.inputString;
        }

        public List<ISelectable[]> GetStoredSelections() {
            return m_StoredSelections;
        }

        public List<ISelectable> GetSelection() {
            return m_Selection;
        }

        void ClearSelection() {
            foreach(ISelectable l_Selected in m_Selection) {
                if(l_Selected != null && !l_Selected.Destroyed())
                    l_Selected.Deselect();
            }
            m_Selection.Clear();
        }

        void SelectArea(Vector3 a_WorldPointA, Vector3 a_WorldPointB) {
            Collider2D[] l_Result = Physics2D.OverlapAreaAll(a_WorldPointA, a_WorldPointB, LayerMask.GetMask(m_Layers));
            if(!Input.GetButton("Query")) ClearSelection();
            SetSelection(l_Result);
        }

        void SetSelection(Collider2D[] a_Colliders) {
            foreach(Collider2D l_Collider in a_Colliders) {
                Model l_Model = l_Collider.transform.parent.gameObject.GetComponent<Model>();
                if(l_Model != null) {
                    Actor2D l_Actor = l_Model.GetActor();
                    if(l_Actor is ISelectable && !m_Selection.Contains(l_Actor as ISelectable)) {
                        m_Selection.Add(l_Actor);
                        l_Actor.Select();
                    }
                }
            }
            m_SelectionChanged = true;
            if(m_Selection.Count > 0) m_Audio.PlayOneShot(m_SelectionSFX);
        }

        void SetSelection(ISelectable[] a_Selection) {
            ClearSelection();
            m_Selection.AddRange(a_Selection);
            foreach(ISelectable l_Selection in m_Selection) {
                if(l_Selection != null && !l_Selection.Destroyed()) {
                    l_Selection.Select();
                }
            }
            m_SelectionChanged = true;
            if(m_Selection.Count > 0) m_Audio.PlayOneShot(m_SelectionSFX);
        }

        public bool SelectionChanged() {
            bool l_SelectionChanged = m_SelectionChanged;
            if(m_SelectionChanged) m_SelectionChanged = false;
            return l_SelectionChanged;
        }

        void DrawSelectionArea() {
            m_SelectionRenderer.enabled = true;
            m_SelectionRenderer.positionCount = 4;
            m_SelectionRenderer.SetPositions(GetRect(m_SelectionWorldPointA, m_MouseWorldPosition));
        }

        void HideSelectionArea() {
            m_SelectionRenderer.enabled = false;
        }

        void DrawPointArea() {
            m_PointRenderer.enabled = true;
            m_PointRenderer.transform.position = m_MouseWorldPosition;
            m_PointRenderer.transform.localScale = new Vector3(m_PointRadius*2f, m_PointRadius*2f, 1f);
            if(m_OverlapPoint && Physics2D.OverlapCircle(m_PointRenderer.transform.position, m_PointRadius, LayerMask.GetMask("Ally", "Enemy", "Neutral"))) {
                m_PointRenderer.color = Color.red;
                m_CanBuildThere = false;
            }else {
                m_PointRenderer.color = Color.green;
                m_CanBuildThere = true;
            }
        }

        void HidePointArea() {
            m_PointRenderer.enabled = false;
        }

        Vector3[] GetRect(Vector3 a_PointA, Vector3 a_PointB) {
            Vector3[] l_Rect = new Vector3[4];
            l_Rect[0] = a_PointA;
            l_Rect[1] = new Vector3(a_PointA.x, a_PointB.y);
            l_Rect[2] = a_PointB;
            l_Rect[3] = new Vector3(a_PointB.x, a_PointA.y);
            return l_Rect;
        }
        
        /// command //////////////////////////////////////////////////////////////////////////////

        void Command() {
            if(Input.GetButtonDown("Fire2") && !MouseOverUI() && m_State == ControllerState.Free) {
                if(Input.GetButton("Query")) QueryMoveCommand(m_MouseWorldPosition);
                else MoveCommand(m_MouseWorldPosition);
            }
            if(Input.GetButtonDown("Hold")) {
                OnHoldPressed();
            }
            if(Input.GetButtonDown("Move")) {
                if(Input.GetButton("Query")) QueryMoveCommand(m_MouseWorldPosition);
                else OnMovePressed();
            }
            if(Input.GetButtonDown("Patrol")) {
                OnPatrolPressed();
            }
            if(Input.GetButtonDown("Attack")) {
                if(Input.GetButton("Query")) OnQueryAttackPressed();
                else OnAttackPressed();
            }
            if(Input.GetButtonDown("Defensiv")) {
                OnDefensivPressed();
            }
            if(Input.GetButtonDown("Aggressiv")) {
                OnAggressivPressed();
            }
        }

        /// ui interface

        public void OnMovePressed() {
            m_State = ControllerState.SelectingPoint;
            m_CommandDelegate = MoveCommand;
            m_PointRadius = 0.25f;
            m_OverlapPoint = false;
        }

        public void OnAttackPressed() {
            m_State = ControllerState.SelectingPoint;
            m_CommandDelegate = AttackCommand;
            m_PointRadius = 0.25f;
            m_OverlapPoint = false;
        }

        public void OnQueryAttackPressed() {
            m_State = ControllerState.SelectingPoint;
            m_CommandDelegate = QueryAttackCommand;
            m_PointRadius = 0.25f;
            m_OverlapPoint = false;
        }

        public void OnBuildBasePressed() {
            if(m_Player.CanPayMinerals(m_BaseCost)) {
                m_Player.PayMinerals(m_BaseCost);
                m_State = ControllerState.SelectingPoint;
                m_CommandDelegate = BuildBase;
                m_PointRadius = 10f;
                m_OverlapPoint = true;
            }
        }

        public void OnBuildTowerPressed() {
            if(m_Player.CanPayMinerals(m_TowerCost)) {
                m_Player.PayMinerals(m_TowerCost);
                m_State = ControllerState.SelectingPoint;
                m_CommandDelegate = BuildTower;
                m_PointRadius = 5f;
                m_OverlapPoint = true;
            }   
        }

        /// build commands //////////////////////////////////////////////

        void BuildBase(Vector3 a_Point) {
            GameObject l_Object = Instantiate(m_BasePrefab, m_Player.transform);
            l_Object.transform.position = a_Point;
        }

        void BuildTower(Vector3 a_Point) {
            GameObject l_Object = Instantiate(m_TowerPrefab, m_Player.transform);
            l_Object.transform.position = a_Point;
        }

        /// point commands

        void MoveCommand(Vector3 a_Point) {
            foreach(ISelectable l_Selected in m_Selection) {
                if(IsCommandable(l_Selected)) {
                    (l_Selected as ICommandable).Move(a_Point);
                }  
            }
            m_Audio.PlayOneShot(m_CommandSFX[Random.Range(0, m_CommandSFX.Count)]);
        }

        void QueryMoveCommand(Vector3 a_Point) {
            foreach(ISelectable l_Selected in m_Selection) {
                if(IsCommandable(l_Selected)) {
                    (l_Selected as ICommandable).QueryMove(a_Point);
                }  
            }
        }

        void AttackCommand(Vector3 a_Point) {
            foreach(ISelectable l_Selected in m_Selection) {
                if(IsCommandable(l_Selected)) {
                    (l_Selected as ICommandable).Attack(a_Point);
                }  
            }
            m_Audio.PlayOneShot(m_CommandSFX[Random.Range(0, m_CommandSFX.Count)]);
        }

        void QueryAttackCommand(Vector3 a_Point) {
            foreach(ISelectable l_Selected in m_Selection) {
                if(IsCommandable(l_Selected)) {
                    (l_Selected as ICommandable).QueryAttack(a_Point);
                }  
            }
            m_Audio.PlayOneShot(m_CommandSFX[Random.Range(0, m_CommandSFX.Count)]);
        }

        /// simple command

        public void OnHoldPressed() {
            foreach(ISelectable l_Selected in m_Selection) {
                if(IsCommandable(l_Selected)) {
                    (l_Selected as ICommandable).Hold();
                }  
            }
        }

        void OnPatrolPressed() {
            foreach(ISelectable l_Selected in m_Selection) {
                if(IsCommandable(l_Selected)) {
                    (l_Selected as ICommandable).Patrol();
                }  
            }
        }

        public void OnDefensivPressed() {
            foreach(ISelectable l_Selected in m_Selection) {
                if(IsCommandable(l_Selected)) {
                    (l_Selected as ICommandable).Defensiv();
                }  
            }
        }

        public void OnAggressivPressed() {
            foreach(ISelectable l_Selected in m_Selection) {
                if(IsCommandable(l_Selected)) {
                    (l_Selected as ICommandable).Aggressiv();
                }  
            }
        }

        bool IsCommandable(ISelectable a_Selected) {
            return a_Selected != null && !a_Selected.Destroyed() && a_Selected is ICommandable;
        }
    }
}


