using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS2D {
    public class Perception : ActorComponent
    {
        [HideInInspector] public List<ScanResult> enemies = new List<ScanResult>();
        [HideInInspector] public List<ScanResult> allies = new List<ScanResult>();
        [HideInInspector] public List<ScanResult> neutral = new List<ScanResult>();

        [Header("Properties")]
        [SerializeField] int m_Period;
        
        [SerializeField] float m_Radius;

        [Header("LayerMask")]
        [SerializeField] string[] m_Layers;

        [Header("Result")]
        [SerializeField] int m_EnemyCount;
        [SerializeField] int m_AllyCount;
        [SerializeField] int m_NeutralCount;

        LayerMask m_Mask;
        int m_Step;

        protected override void Setup() {
            m_Step = 0;
            m_Mask = ComputeMask();
        }

        public override string GetPropertyString() {
            return "RNG: "+m_Radius.ToString("0");
        }

        LayerMask ComputeMask() {
            return LayerMask.GetMask(m_Layers);
        }

        void Update()
        {
            m_Step++;
            if(m_Step == m_Period) {
                Scan();
                BroadcastMessage("OnScan", m_Radius);
                m_Step = 0;
            }
        }

        void Scan() {
            Clear();
            Collider2D[] l_Result = Physics2D.OverlapCircleAll(GetActor().model.body.transform.position, m_Radius, m_Mask);
            foreach(Collider2D l_Collider in l_Result) {
                ActorComponent l_Component = l_Collider.transform.parent.gameObject.GetComponent<ActorComponent>();
                if(l_Component != null) {
                    Actor2D l_Actor = l_Component.GetActor();
                    AddActor(l_Actor);
                }
            }
            Sort();
        }

        void AddActor(Actor2D a_Actor) {
            if(a_Actor != null && a_Actor != GetActor()) {
                float l_Distance = GetDistance(a_Actor);
                ScanResult l_Result = new ScanResult(a_Actor, l_Distance);
                if(a_Actor.CompareTag("Ally")){
                    allies.Add(l_Result);
                }else if(a_Actor.CompareTag("Enemy")) {
                    enemies.Add(l_Result);
                }else if(a_Actor.CompareTag("Neutral")) {
                    neutral.Add(l_Result);
                }
            }
        }

        void Clear() {
            allies.Clear();
            enemies.Clear();
            neutral.Clear();
        }

        void Sort() {
            allies.Sort();
            enemies.Sort();
            neutral.Sort();
            m_AllyCount = allies.Count;
            m_EnemyCount = enemies.Count;
            m_NeutralCount = neutral.Count;
        }

        float GetDistance(Actor2D a_Actor) {
            if(a_Actor != null && !a_Actor.Destroyed())
                return Vector3.Distance(a_Actor.model.body.transform.position, GetActor().model.body.transform.position);
            else{
                return Mathf.Infinity;
            }
        }
    }
}
