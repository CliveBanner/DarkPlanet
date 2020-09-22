using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS2D {
    public abstract class ActorComponent : MonoBehaviour
    {
        Actor2D m_Actor;

        void Start() {
            m_Actor = GetComponent<Actor2D>();
            if(m_Actor == null) {
                m_Actor = GetComponentInParent<Actor2D>();
            }
            if(m_Actor == null) {
                Debug.LogError("Not attached to an 'Actor2D'!");
            }
            Setup();
        }
        
        protected abstract void Setup();

        public Actor2D GetActor() {
            return m_Actor;
        }

        public virtual string GetPropertyString() {
            return "";
        }
    }
}


