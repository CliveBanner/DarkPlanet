using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS2D
{
    public class Bar : MonoBehaviour
    {
        [SerializeField] Color m_FillColor, m_BackgroundColor;
        [SerializeField] SpriteRenderer m_FillRenderer, m_BackgroundRenderer;
        [SerializeField] Transform m_Transform;
        [SerializeField] Vector3 m_Offset;
        [SerializeField] bool m_AlwaysShow;
        Vector3 m_Scale, m_Position;

        void Start()
        {
            m_BackgroundRenderer.color = m_BackgroundColor;
            m_FillRenderer.color = m_FillColor;
            m_Position = m_Transform.position + m_Offset;
            m_Scale = m_FillRenderer.transform.localScale;
            Hide();
        }
        public void Hide() {
            if(m_AlwaysShow) return;
            m_BackgroundRenderer.enabled = false;
            m_FillRenderer.enabled = false;
        }

        public void Show() {
            m_BackgroundRenderer.enabled = true;
            m_FillRenderer.enabled = true;
        }

        public void SetValue(float a_Value) {
            m_FillRenderer.transform.localScale = new Vector3(m_Scale.x * a_Value, m_Scale.y, m_Scale.z);
            m_FillRenderer.transform.localPosition = new Vector3(-0.5f + (a_Value/2f), 0f, 0f);
        }

        void Update() {
            gameObject.transform.position = m_Transform.position + m_Offset;
            gameObject.transform.rotation = Quaternion.identity;
        }
    }
}


