using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RTS2D {
    public class KeysUI : MonoBehaviour
    {
        [SerializeField] RTSController m_Controller;
        [SerializeField] GameObject[] m_Keys;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            UpdateKeys();
        }

        void UpdateKeys() {
            for(int i = 0; i < m_Controller.GetStoredSelections().Count; i++) {
                ISelectable[] l_Selection = m_Controller.GetStoredSelections()[i];
                if(l_Selection != null) {
                    m_Keys[i].SetActive(true);
                }else {
                    m_Keys[i].SetActive(false);
                }
            }
        }
    }

}
