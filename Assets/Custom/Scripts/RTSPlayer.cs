using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RTS2D
{
    public class RTSPlayer : MonoBehaviour
    {
        [SerializeField] List<Actor2D> m_Actors = new List<Actor2D>();
        [SerializeField] float m_Minerals;

        void Start() {}

        /// register ////////////////////////////////////////////

        public void Register(Actor2D a_Actor) {
            m_Actors.Add(a_Actor);
        }

        public void Deregister(Actor2D a_Actor) {
            m_Actors.Remove(a_Actor);
        }

        /// minerals ////////////////////////////////////////////
        public float GetMinerals() {
            return m_Minerals;
        }

        public void AddMinerals(float a_Amount) {
            m_Minerals += a_Amount;
        }

        public bool CanPayMinerals(float a_Amount) {
            return a_Amount <= m_Minerals;
        }

        public void PayMinerals(float a_Amount) {
            m_Minerals -= a_Amount;
        }

        /// getter ////////////////////////////////////////////

        public ISelectable[] GetAllBases() {
            List<ISelectable> l_AllBases = new List<ISelectable>();
            foreach (Actor2D l_Actor in m_Actors) {
                if(l_Actor.GetID() == "Base") l_AllBases.Add(l_Actor as ISelectable);
            }
            return l_AllBases.ToArray();
        }
    }
}

