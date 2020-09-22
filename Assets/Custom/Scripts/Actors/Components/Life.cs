using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RTS2D {
    public class Life : ActorComponent
    {
        [Header("Properties")]
        [SerializeField] float m_MaxHealth;
        [SerializeField] float m_HealthRegeneration;
        [SerializeField] float m_MaxShield;
        [SerializeField] float m_ShieldRegeneration;
        [SerializeField] float m_Armor;
        float m_Health;
        float m_Shield;
        bool m_Dead;
        
        protected override void Setup() {
            m_Health = m_MaxHealth;
            m_Shield = m_MaxShield;
            m_Dead = false;
        }

        public override string GetPropertyString() {
            return "HP: "+m_MaxHealth.ToString("0") + ", SHIELD: "+m_MaxShield.ToString("0");
        }

        public void OnGather(float a_Amount) {
            Damage(a_Amount);
        }

        public void OnDamage(Damage a_Damage) {
            Damage(a_Damage.GetAmount());
        }

        void Damage(float a_Amount) {
            if(m_Shield >= a_Amount) {
                m_Shield -= a_Amount;
                BroadcastMessage("OnShieldHit");
            }else {
                float l_Amount = a_Amount - m_Shield;
                m_Shield = 0f;
                m_Health -= l_Amount * (1f-m_Armor);
                BroadcastMessage("OnHit");
                if(m_Health <= 0) {
                    m_Dead = true;
                    BroadcastMessage("OnDead");
                }
            }
            ClampHealth();
        }

        public bool Dead() {
            return m_Dead;
        }

        public void OnHeal(float a_Amount) {
            Heal(a_Amount);
        }

        void Heal(float a_Amount) {
            m_Shield += a_Amount;
            ClampShield();
        }

        public bool FullShield() {
            return m_Shield >= m_MaxShield;
        }

        public bool FullHealth() {
            return m_Health >= m_MaxHealth;
        }

        public float HealthPercent() {
            return m_Health / m_MaxHealth;
        }

        public float ShieldPercent() {
            return m_Shield / m_MaxShield;
        }

        void Regenerate() {
            if(m_Health < m_MaxHealth) {
                m_Health += m_HealthRegeneration * Time.deltaTime;
                ClampHealth();
            }
            if(m_Shield < m_MaxShield) {
                m_Shield += m_ShieldRegeneration * Time.deltaTime;
                ClampShield();
            }
            BroadcastMessage("OnRegenerate");
        }

        void ClampHealth() {
            m_Health = Mathf.Clamp(m_Health, 0f, m_MaxHealth);
        }

        void ClampShield() {
            m_Shield = Mathf.Clamp(m_Shield, 0f, m_MaxShield);
        }

        void Update() {
            Regenerate();
            if(m_Shield < 15f) {
                BroadcastMessage("OnShieldDestroyed");
            }else {
                BroadcastMessage("OnShieldRestored");
            }
        }
    }
}

