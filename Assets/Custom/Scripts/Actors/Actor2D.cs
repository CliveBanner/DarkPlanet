using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS2D {
    public class Actor2D : MonoBehaviour, ISelectable
    {
        [HideInInspector] public Model model;
        [HideInInspector] public Life life;
        [HideInInspector] public RTSPlayer player;

        [Header("Properties")]
        [SerializeField] string m_ID;

        List<ActorComponent> m_Components;

        // Start is called before the first frame update
        void Start() {
            player = GetComponentInParent<RTSPlayer>();
            player.Register(this);
            model = GetComponent<Model>();
            life = GetComponent<Life>();
            m_Components = new List<ActorComponent>();
            m_Components.AddRange(GetComponents<ActorComponent>());
        }

        /// ISelectable ///
        public string GetID() {
            return m_ID;
        }

        public string GetPropertyString() {
            string l_Properties = ">"+GetID() + "\n";
            foreach (ActorComponent l_Component in m_Components) {
                if(l_Component.enabled && l_Component.GetPropertyString() != "") {
                    l_Properties += l_Component.GetPropertyString() +"\n";
                }
            }
            return l_Properties;
        }

        public void Select() {
            BroadcastMessage("OnSelected");
        }

        public void Deselect() {
            BroadcastMessage("OnDeselected");
        }

        public bool Destroyed() {
            return life.Dead();
        }

        public GameObject GetObject() {
            return gameObject;
        }
    }
}
