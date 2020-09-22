using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RTS2D
{
    public class Unit2D : Actor2D, ICommandable, IAttackable
    {
        /// ICommandable ///
        public void Move(Vector3 a_Point) {
            BroadcastMessage("OnMove", a_Point, SendMessageOptions.DontRequireReceiver);
        }

        public void QueryMove(Vector3 a_Point) {
            BroadcastMessage("OnQueryMove", a_Point, SendMessageOptions.DontRequireReceiver);
        }

        public void Attack(Vector3 a_Point) {
            BroadcastMessage("OnAttack", a_Point, SendMessageOptions.DontRequireReceiver);
        }

        public void QueryAttack(Vector3 a_Point) {
            BroadcastMessage("OnQueryAttack", a_Point, SendMessageOptions.DontRequireReceiver);
        }

        public void Patrol() {
            BroadcastMessage("OnPatrol", SendMessageOptions.DontRequireReceiver);
        }

        public void Hold() {
            BroadcastMessage("OnHold", SendMessageOptions.DontRequireReceiver);
        }

        public void Defensiv() {
            BroadcastMessage("OnDefensiv", SendMessageOptions.DontRequireReceiver);
        }

        public void Aggressiv() {
            BroadcastMessage("OnAggressiv", SendMessageOptions.DontRequireReceiver);
        }

        /// IAttackable ///
        public void Damage(Damage a_Damage) {
            BroadcastMessage("OnDamage", a_Damage, SendMessageOptions.DontRequireReceiver);
        }

        public void Heal(float a_Amount) {
            BroadcastMessage("OnHeal", a_Amount, SendMessageOptions.DontRequireReceiver);
        }
    }
}

