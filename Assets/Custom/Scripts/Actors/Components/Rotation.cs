using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RTS2D {
    public class Rotation : ActorComponent
    {
        Vector3 m_Target;
        Vector3 m_Direction;
        float m_Angle;

        Attack m_Attack;
        Movement m_Movement;

        protected override void Setup() {
            m_Attack = GetComponent<Attack>();
            m_Movement = GetComponent<Movement>();
        }

        void Update()
        {
            Choose();
            Turn();
        }

        void Choose() {
            if(IsAttacking()) m_Target = m_Attack.targetActor.model.body.transform.position;
            else if(IsSpotted())  m_Target = m_Attack.spottedActor.model.body.transform.position;
            else if(IsMoving()) m_Target = m_Movement.currentPoint;
        }

        bool IsMoving() {
            return m_Movement != null && m_Movement.IsMoving();
        }

        bool IsSpotted() {
            return m_Attack != null && m_Attack.spottedActor != null;
        }

        bool IsAttacking() {
            return m_Attack != null && m_Attack.targetActor != null;
        }

        void Turn() {
            if(GetActor() != null) {
                m_Direction = (m_Target - GetActor().model.body.transform.position).normalized;
                m_Angle = Vector3.SignedAngle(m_Direction, Vector3.up, Vector3.back);
                GetActor().model.body.transform.rotation = Quaternion.Euler(0f, 0f, m_Angle);
            }
        }
    }

}