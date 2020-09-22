using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS2D {
    [System.Serializable]
    public class ScanResult : IComparable<ScanResult> {
        Actor2D m_Actor;
        float m_Distance;
        public ScanResult(Actor2D a_Actor, float a_Distance) {
            m_Actor = a_Actor;
            m_Distance = a_Distance;
        }

        public Actor2D GetActor() {
            return m_Actor;
        }

        public float GetDistance() {
            return m_Distance;
        }

        public int CompareTo(ScanResult obj) {
            return GetDistance().CompareTo(obj.GetDistance());
        }
    }
}
