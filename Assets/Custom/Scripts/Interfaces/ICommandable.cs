using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS2D {
    public interface ICommandable {
        void Move(Vector3 a_Point);
        void QueryMove(Vector3 a_Point);
        void Attack(Vector3 a_Point);
        void QueryAttack(Vector3 a_Point);
        void Patrol();
        void Hold();
        void Defensiv();
        void Aggressiv();
    }
}
