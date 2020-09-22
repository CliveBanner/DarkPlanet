using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RTS2D
{
    public class Damage 
    {
        float m_Amount;
        Actor2D m_Source;
        public Damage(float a_Amount, Actor2D a_Source) {
            m_Amount = a_Amount;
            m_Source = a_Source;
        }

        public float GetAmount() {
            return m_Amount;
        }

        public Actor2D GetSource() {
            return m_Source;
        }
    }
}
