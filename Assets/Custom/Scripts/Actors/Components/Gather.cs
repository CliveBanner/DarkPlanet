using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RTS2D
{
    [RequireComponent(typeof(Perception))]
    public class Gather : ActorComponent
    {
        [HideInInspector] public Actor2D targetActor;
        [SerializeField] float m_Range; 
        [SerializeField] float m_Rate;
        Perception m_Perception;
        protected override void Setup()
        {
            m_Perception = GetComponent<Perception>();
        }

        void Update()
        {
            Choose();
            Track();
            Mine();
            Render();
        }

        void Choose() {
            if(targetActor == null && m_Perception.neutral.Count > 0) {
                foreach(ScanResult l_Result in m_Perception.neutral) {
                    if(l_Result.GetActor() is IGatherable && !l_Result.GetActor().life.Dead()) {
                        targetActor = l_Result.GetActor();
                    }
                }
            } 
        }

        void Track() {
            if(targetActor != null) {
                if(Vector3.Distance(targetActor.model.body.transform.position, GetActor().model.body.transform.position) > m_Range || targetActor.life.Dead()) {
                    targetActor = null;
                }
            }
        }

        void Mine() {
            if(targetActor != null) {
                (targetActor as IGatherable).Gather(m_Rate * Time.deltaTime);
                GetActor().player.AddMinerals(m_Rate * Time.deltaTime);
            } 
        }

        void Render() {
            if(targetActor != null) {
                Vector3[] l_Line = new Vector3[] { GetActor().model.body.transform.position, 
                                                   targetActor.model.body.transform.position};
                BroadcastMessage("OnGatherTarget", l_Line);
            }else {
                BroadcastMessage("OnGatherTargetLost");
            }
        }
    }
}

