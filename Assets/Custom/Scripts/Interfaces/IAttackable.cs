using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS2D {
    public interface IAttackable
    {
        void Damage(Damage a_Damage);
        void Heal(float a_Amount);
    }
}


