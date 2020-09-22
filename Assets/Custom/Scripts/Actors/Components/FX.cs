using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RTS2D
{
    public class FX : ActorComponent
    {
        [Header("Particles")]
        [SerializeField] ParticleSystem m_AttackFX;
        [SerializeField] ParticleSystem m_HitFX;
        [SerializeField] ParticleSystem m_ShieldHitFX;
        [SerializeField] ParticleSystem m_ProduceFX;
        [SerializeField] GameObject m_DeadFXPrefab;

        [Header("Lines")]
        [SerializeField] LineRenderer m_PathRenderer;
        [SerializeField] LineRenderer m_AttackRenderer;
        [SerializeField] LineRenderer m_HealRenderer;
        [SerializeField] LineRenderer m_GatherRenderer;

        [Header("SpriteMask")]
        [SerializeField] SpriteMask m_SpriteMask;

        [Header("Bars")]
        [SerializeField] Bar m_HealthBar;
        [SerializeField] Bar m_ShieldBar;
        [SerializeField] Bar m_ProductionBar;

        [Header("Sprites")]
        [SerializeField] SpriteRenderer m_ShieldRenderer;
        [SerializeField] SpriteRenderer m_AttackRangeRenderer;
        [SerializeField] SpriteRenderer m_IndicatorRenderer;

        [Header("Sounds")]
        [SerializeField] AudioClip m_AttackSFX;
        [SerializeField] AudioClip m_HealSFX;
        [SerializeField] AudioClip m_GatherSFX;
        [SerializeField] AudioClip m_MoveSFX;
        [SerializeField] AudioClip m_ProductionCompleteSFX;
        [SerializeField] AudioClip m_HitSFX;
        [SerializeField] AudioClip m_ShieldHitSFX;
        
        AudioSource m_Audio;

        protected override void Setup() {
            m_Audio = GetComponentInChildren<AudioSource>();
        }

        /// selected fx ////////////////////////////////////////////////////

        public void OnSelected() {
            if(m_HealthBar != null) {
                m_HealthBar.Show();
            }
            
            if(m_ShieldBar != null) {
                m_ShieldBar.Show();
            }
            if(m_IndicatorRenderer != null) {
                m_IndicatorRenderer.enabled = true;
            }
        }

        public void OnDeselected() {
            if(m_HealthBar != null) {
                m_HealthBar.Hide();
            }
            
            if(m_ShieldBar != null) {
                m_ShieldBar.Hide();
            }
            if(m_IndicatorRenderer != null) {
                m_IndicatorRenderer.enabled = false;
            }
        }

        /// life fx ////////////////////////////////////////////////////

        public void OnHit() {
            if(m_HitFX != null) {
                if(!m_HitFX.isPlaying)
                    m_HitFX.Play();
            }
            if(m_HitSFX != null) m_Audio.PlayOneShot(m_HitSFX, 2f);
            if(m_HealthBar != null) m_HealthBar.SetValue(GetActor().life.HealthPercent());
        }

        public void OnShieldHit() {
            if(m_ShieldHitFX != null) {
                if(!m_ShieldHitFX.isPlaying) 
                    m_ShieldHitFX.Play();
            }
            if(m_ShieldHitSFX != null) m_Audio.PlayOneShot(m_ShieldHitSFX, 2f);
            if(m_ShieldBar != null) m_ShieldBar.SetValue(GetActor().life.ShieldPercent());
        }

        public void OnRegenerate() {
            if(m_HealthBar != null && GetActor() != null) {
                m_HealthBar.SetValue(GetActor().life.HealthPercent());
            }
            
            if(m_ShieldBar != null && GetActor() != null) {
                m_ShieldBar.SetValue(GetActor().life.ShieldPercent());
            }
        }

        public void OnDead() {
            GetActor().player.Deregister(GetActor());
            Destroy(Instantiate(m_DeadFXPrefab, GetActor().model.body.transform.position, Quaternion.identity), 2f);
            Destroy(gameObject);
        }

        public void OnShieldDestroyed() {
            if(m_ShieldRenderer == null) return;
            m_ShieldRenderer.enabled = false;
        }

        public void OnShieldRestored() {
            if(m_ShieldRenderer == null) return;
            m_ShieldRenderer.enabled = true;
        }

        /// move fx ////////////////////////////////////////////////////

        public void OnPath(Vector3[] a_Path) {
            if(m_PathRenderer == null) return;
            m_PathRenderer.positionCount = a_Path.Length;
            for(int i = 0; i < m_PathRenderer.positionCount; i++) {
                m_PathRenderer.SetPosition(i, a_Path[i]);
            }
        }

        public void OnHoldOnTarget(bool a_Hold) {
            if(m_PathRenderer == null) return;
            m_PathRenderer.startColor = a_Hold ? Color.red : Color.green;
            m_PathRenderer.endColor = a_Hold ? Color.red : Color.green;
        }

        /// perception fx ////////////////////////////////////////////////////

        public void OnScan(float a_Radius) {
            if(m_SpriteMask == null) return;
            float l_Diameter = a_Radius*2f;
            m_SpriteMask.transform.localScale = new Vector3(l_Diameter, l_Diameter, 0f);
        }
        
        /// attack fx ////////////////////////////////////////////////////

        public void OnAttacking() {
            if(m_AttackSFX != null) m_Audio.PlayOneShot(m_AttackSFX, 2f);
            if(m_AttackFX == null) return;
            if(!m_AttackFX.isPlaying)
                m_AttackFX.Play();
        }

        public void OnTarget(Vector3[] a_Line) {
            if(m_AttackRenderer == null) return;
            m_AttackRenderer.enabled = true;
            m_AttackRenderer.positionCount = a_Line.Length;
            m_AttackRenderer.SetPositions(a_Line);
        } 

        public void OnTargetLost() {
            if(m_AttackRenderer == null) return;
            m_AttackRenderer.enabled = false;
        }

        public void OnRangeChanged(float a_Range) {
            if(m_AttackRangeRenderer != null) {
                m_AttackRangeRenderer.transform.localScale = new Vector3(a_Range*2f, a_Range*2f, 1f);
            }
        }

        public void OnPolicyChanged(AttackPolicy a_Policy) {
            if(m_IndicatorRenderer == null) return;
            if(a_Policy == AttackPolicy.Defensiv) {
                m_IndicatorRenderer.color = Color.green;
            }else if(a_Policy == AttackPolicy.Aggressiv) {
                m_IndicatorRenderer.color = Color.red;
            }
        }

        /// heal fx ////////////////////////////////////////////////////

        public void OnHealTarget(Vector3[] a_Line) {
            if(m_HealRenderer == null) return;
            m_HealRenderer.enabled = true;
            m_HealRenderer.positionCount = a_Line.Length;
            m_HealRenderer.SetPositions(a_Line);
            if(m_HealSFX != null && !m_Audio.isPlaying) m_Audio.clip = m_HealSFX; m_Audio.loop = true; m_Audio.Play();
        }

        public void OnHealTargetLost() {
            if(m_HealRenderer == null) return;
            m_HealRenderer.enabled = false;
            m_Audio.Stop();
        }

        /// gather fx ////////////////////////////////////////////////////

        public void OnGatherTarget(Vector3[] a_Line) {
            if(m_GatherRenderer == null) return;
            m_GatherRenderer.enabled = true;
            m_GatherRenderer.positionCount = a_Line.Length;
            m_GatherRenderer.SetPositions(a_Line);
            if(m_GatherSFX != null && !m_Audio.isPlaying) m_Audio.clip = m_GatherSFX; m_Audio.loop = true; m_Audio.Play();
        } 

        public void OnGatherTargetLost() {
            if(m_GatherRenderer == null) return;
            m_GatherRenderer.enabled = false;
            m_Audio.Stop();
        }

        /// production fx ////////////////////////////////////////////////////

        public void OnProduce(float a_Percent) {
            if(m_ProductionBar == null) return;
            m_ProductionBar.SetValue(a_Percent);
        }

        public void OnComplete(Vector3 a_Position) {
            if(m_ProductionCompleteSFX != null) m_Audio.PlayOneShot(m_ProductionCompleteSFX, 2f);
            if(m_ProduceFX == null) return;
            if(!m_ProduceFX.isPlaying) {
                m_ProduceFX.transform.position = a_Position;
                m_ProduceFX.Play();
            }
        }
    }
}
