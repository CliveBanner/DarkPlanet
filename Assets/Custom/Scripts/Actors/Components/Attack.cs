using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS2D 
{
    public enum AttackPolicy {
        Defensiv,
        Aggressiv
    }

    [RequireComponent(typeof(Perception))]
    public class Attack : ActorComponent
    {
        [HideInInspector] public Actor2D targetActor;
        [HideInInspector] public Actor2D spottedActor;

        [Header("Properties")]
        [SerializeField] float m_Range;
        [SerializeField] float m_Damage;
        [SerializeField] float m_FireRate;
        [SerializeField] bool m_AttackAllies;
        [SerializeField] AttackPolicy m_Policy;

        float m_CurrentTime;
        Perception m_Perception;

        /// overrides /////////////////////////////////////////////////////

        protected override void Setup(){
            m_CurrentTime = 0f;
            m_Perception = GetComponent<Perception>();
            if(m_Perception == null) Debug.LogError("Attack without Perception!");
            SetRange(m_Range);
            SetPolicy(m_Policy);
        }

        public override string GetPropertyString() {
            return "DMG: "+m_Damage.ToString("0") + ", RATE: "+m_FireRate.ToString("0");
        }

        void Update() {
            Choose();
            Track();
            Damage();
            Render();
        }

        /// broadcast receivers /////////////////////////////////////////////////////

        public void OnDamage(Damage a_Damage) {
            if(m_Policy == AttackPolicy.Aggressiv) spottedActor = a_Damage.GetSource();
        }

        public void OnDefensiv() {
            SetPolicy(AttackPolicy.Defensiv);
        }

        public void OnAggressiv() {
            SetPolicy(AttackPolicy.Aggressiv);
        }

        /// wrappers ////////////////////////////////////////////////////////////////

        void SetRange(float a_Range) {
            m_Range = a_Range;
            BroadcastMessage("OnRangeChanged", m_Range);
        }

        void SetPolicy(AttackPolicy a_Policy) {
            m_Policy = a_Policy;
            BroadcastMessage("OnPolicyChanged", m_Policy);
        }

        /// helpers /////////////////////////////////////////////////////////////////

        void Choose() {
            List<ScanResult> l_Enemies;
            if(m_AttackAllies) l_Enemies = m_Perception.allies;
            else l_Enemies = m_Perception.enemies;
            if(targetActor == null) {
                if(l_Enemies.Count > 0) {
                    foreach(ScanResult l_Result in l_Enemies) {
                        if(l_Result.GetActor() is IAttackable) {
                            if(InRange(l_Result.GetActor())) {
                                targetActor = l_Result.GetActor();
                                break;
                            }else if(m_Policy == AttackPolicy.Aggressiv) {
                                spottedActor = l_Result.GetActor();
                                break;
                            }else {
                                targetActor = null;
                                spottedActor = null;
                            }
                        }
                    }
                }
            }
        }

        void Track() {
            if(!InRange(targetActor)) {
                targetActor = null;
            }
        }

        bool Exists(Actor2D a_Actor) {
            return a_Actor != null && !a_Actor.Destroyed();
        }

        bool InRange(Actor2D a_Other) {
            if(Exists(a_Other))
                return Vector3.Distance(a_Other.model.body.transform.position, GetActor().model.body.transform.position) < m_Range;
            else
                return false;
        }

        void Damage() {
            if(targetActor != null) {
                m_CurrentTime += Time.deltaTime;
                if(m_CurrentTime >= (1f/m_FireRate)) {
                    m_CurrentTime = 0f;
                    BroadcastMessage("OnAttacking");
                    (targetActor as IAttackable).Damage(new Damage(m_Damage, GetActor()));
                }
            }else {
                m_CurrentTime = 0f;
            }
        }

        /// fx ///////////////////////////////////////////////////////////////////////////////////

        void Render() {
            if(targetActor != null) {
                Vector3[] l_Line = new Vector3[] {GetActor().model.body.transform.position, 
                                                  targetActor.model.body.transform.position};
                BroadcastMessage("OnTarget", l_Line);
            }else {
                BroadcastMessage("OnTargetLost");
            }
        }
    }
}

