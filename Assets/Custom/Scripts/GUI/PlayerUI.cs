using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RTS2D
{
    public class PlayerUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] RTSPlayer m_Player;
        [SerializeField] RTSController m_Controller;
        [SerializeField] Text m_MineralsText;
        [SerializeField] GameObject m_SelectionUI;
        [SerializeField] GameObject m_ProductionUI;

        [Header("Prefabs")]
        [SerializeField] GameObject m_SelectionPrefab;
        [SerializeField] GameObject m_ProductionPrefab;

        List<GameObject> m_SelectionTexts;
        List<GameObject> m_ProductionPanels;

        void Start()
        {
            m_SelectionTexts = new List<GameObject>();
            m_ProductionPanels = new List<GameObject>();
        }

        void ShowSelection() {
            Dictionary<string, int> l_Numbers = new Dictionary<string, int>();
            foreach (ISelectable l_Actor in m_Controller.GetSelection()) {
                if(!l_Numbers.ContainsKey(l_Actor.GetID())) l_Numbers.Add(l_Actor.GetID(), 0);
                l_Numbers[l_Actor.GetID()] += 1;
            }
            foreach(string l_Key in l_Numbers.Keys) {
                GameObject l_Object = Instantiate(m_SelectionPrefab, m_SelectionUI.transform);
                l_Object.GetComponent<Text>().text = l_Key+": "+l_Numbers[l_Key];
                m_SelectionTexts.Add(l_Object);
            }
        }

        void ShowProduction() {
            foreach (ISelectable l_Actor in m_Controller.GetSelection()) {
                if(l_Actor.GetID() == "Base") {
                    IProducing l_Production = l_Actor.GetObject().GetComponent<IProducing>();
                    if(l_Production != null) {
                        GameObject l_Object = Instantiate(m_ProductionPrefab, m_ProductionUI.transform);
                        l_Object.GetComponent<ProductionUI>().SetProduction(l_Production);
                        m_ProductionPanels.Add(l_Object);
                    }
                }
            }
        }

        void ClearSelection() {
            foreach(GameObject l_Object in m_SelectionTexts) {
                Destroy(l_Object);
            }
            m_SelectionTexts.Clear();
        }

        void ClearProduction() {
            foreach(GameObject l_Object in m_ProductionPanels) {
                Destroy(l_Object);
            }
            m_ProductionPanels.Clear();
        }

        void UpdateSelection() {
            if(m_Controller.SelectionChanged()) {
                ClearSelection();
                ClearProduction();
                ShowSelection();
                ShowProduction();
            }
        }

        // Update is called once per frame
        void Update()
        {
            m_MineralsText.text = m_Player.GetMinerals().ToString("0");
            UpdateSelection();
        }
    }
}

