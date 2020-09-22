using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS2D {
    public class Movement : ActorComponent
    {
        [SerializeField] List<Vector3> m_WayPoints = new List<Vector3>();
        [SerializeField] MoveState m_State;
        [SerializeField] float m_Force, m_MaxVelocity, m_Threshold;
        [SerializeField] public Vector3 currentPoint;
        [SerializeField] bool m_HoldOnTarget;
        [SerializeField] bool m_Patrol;

        Attack m_Attack;

        enum MoveState {
            Hold,
            Move,
            Attack
        }

        /// overrides /////////////////////////////////////////////////////

        protected override void Setup() {
            m_Attack = GetComponent<Attack>();
        }

        public override string GetPropertyString() {
            return "SPEED: "+m_MaxVelocity.ToString("0");
        }

        void FixedUpdate() 
        {
            if(IsSpotted() && !IsAttacking() && (m_HoldOnTarget || m_State == MoveState.Hold)) {
                m_State = MoveState.Attack;
            }
            else if(m_WayPoints.Count > 0 && (!IsAttacking() || !m_HoldOnTarget)) {
                m_State = MoveState.Move;
                ForgetSpotted();
            }
            else m_State = MoveState.Hold;
            
            if(m_State == MoveState.Attack) {
                Move(m_Attack.spottedActor.model.body.transform.position);
            }
            else if(m_State == MoveState.Move) {
                currentPoint = m_WayPoints[0];
                Move(currentPoint);
                if(Reached()) {
                    if(!m_Patrol) {
                        m_WayPoints.RemoveAt(0);
                    }else {
                        m_WayPoints.Add(m_WayPoints[0]);
                        m_WayPoints.RemoveAt(0);
                    }
                        
                }  
            }
            RenderPath();
        }

        /// Broadcast Receivers ///////////////////////////////////////////////////////////

        public void OnMove(Vector3 a_Point) {
            ClearWayPoints();
            AddWayPoint(a_Point);
            HoldOnTarget(false);
            m_Patrol = false;
        }

        public void OnQueryMove(Vector3 a_Point) {
            AddWayPoint(a_Point);
            HoldOnTarget(false);
            m_Patrol = false;
        }

        public void OnAttack(Vector3 a_Point) {
            ClearWayPoints();
            AddWayPoint(a_Point);
            HoldOnTarget(true);
            m_Patrol = false;
        }

        public void OnQueryAttack(Vector3 a_Point) {
            AddWayPoint(a_Point);
            HoldOnTarget(true);
            m_Patrol = false;
        }

        public void OnHold() {
            ClearWayPoints();
            m_Patrol = false;
        }

        public void OnPatrol() {
            m_Patrol = true;
        }

        /// waypoints ///////////////////////////////////////////////////////////

        public void AddWayPoint(Vector3 a_Point) {
            m_WayPoints.Add(a_Point);
        }

        public void ClearWayPoints() {
            m_WayPoints.Clear();
        }

        /// component reference states /////////////////////////////////////////////////////////////////

        bool IsSpotted() {
            return m_Attack != null && m_Attack.spottedActor != null;
        }

        bool IsAttacking() {
            return m_Attack != null && m_Attack.targetActor != null;
        }

        void ForgetSpotted() {
            if(m_Attack != null) m_Attack.spottedActor = null;
        }

        /// move ////////////////////////////////////////////////////////////////////////////////////////////////////////

        bool Reached() {
            return Vector3.Distance(GetActor().model.body.transform.position, currentPoint) < m_Threshold;
        }

        public bool IsMoving() {
            return m_State != MoveState.Hold;
        }

        public void Move(Vector3 a_Point) {
            Vector3 l_Direction = (a_Point - GetActor().model.body.transform.position).normalized;
            GetActor().model.body.AddForce(l_Direction * m_Force);
            GetActor().model.body.velocity = Vector3.ClampMagnitude(GetActor().model.body.velocity, m_MaxVelocity);
        }

        public void HoldOnTarget(bool a_Hold) {
            BroadcastMessage("OnHoldOnTarget", a_Hold);
            m_HoldOnTarget = a_Hold;
        }

        /// fx ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        void RenderPath() {
            if(GetActor() != null && GetActor().model != null) {
                Vector3[] l_Path = new Vector3[m_WayPoints.Count+1];
                l_Path[0] = GetActor().model.body.transform.position;
                Array.Copy(m_WayPoints.ToArray(), 0, l_Path, 1, m_WayPoints.Count);
                BroadcastMessage("OnPath", l_Path);
            }
        }
    }
}
