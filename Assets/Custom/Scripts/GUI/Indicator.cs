using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RTS2D
{
    public class Indicator : MonoBehaviour
    {
        [SerializeField] Transform m_Body;
        [SerializeField] Vector3 m_Offset;

        void Update() {
            transform.position = m_Body.position + m_Offset;
        }
    }
}
