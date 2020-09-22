using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RTS2D
{
    public class Generator : ActorComponent
    {
        [SerializeField] float m_Energy;

        protected override void Setup() {
            // GetActor().player.AddEnergy(m_Energy);
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}

