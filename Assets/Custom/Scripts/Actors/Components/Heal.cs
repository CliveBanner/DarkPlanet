using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RTS2D
{
    [RequireComponent(typeof(Perception))]
    public class Heal : ActorComponent
    {
        Perception m_Perception;
        [HideInInspector] public Actor2D targetActor;
        [SerializeField] float m_Range;
        [SerializeField] float m_Rate;
        [SerializeField] bool m_HealAllies;

        protected override void Setup()
        {
            m_Perception = GetComponent<Perception>();
        }

        void Update()
        {
            Choose();
            Track();
            Restore();
            Render();
        }

        public override string GetPropertyString() {
            return "HEAL: "+m_Rate.ToString("0");
        }

        void Choose() {
            List<ScanResult> l_Allies;
            if(m_HealAllies) {
                l_Allies = m_Perception.allies;
            }else {
                l_Allies = m_Perception.enemies;
            }
            if(targetActor == null && l_Allies.Count > 0) {
                foreach(ScanResult l_Result in l_Allies) {
                    if(l_Result.GetActor() is IAttackable && !l_Result.GetActor().life.FullShield()) {
                        targetActor = l_Result.GetActor();
                    }
                }
            } 
        }

        void Track() {
            if(targetActor != null) {
                if(Vector3.Distance(targetActor.model.body.transform.position, GetActor().model.body.transform.position) > m_Range || targetActor.life.FullShield()) {
                    targetActor = null;
                }
            }
        }

        void Restore() {
            if(targetActor != null) 
                (targetActor as IAttackable).Heal(m_Rate * Time.deltaTime);
        }

        void Render() {
            if(targetActor != null) {
                Vector3[] l_Line = new Vector3[] { GetActor().model.body.transform.position, 
                                                   targetActor.model.body.transform.position};
                BroadcastMessage("OnHealTarget", l_Line);
            }else {
                BroadcastMessage("OnHealTargetLost");
            }
        }
    }
}
