using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS2D {
    public class Model : ActorComponent
    {
        [HideInInspector] public Rigidbody2D body;
        [HideInInspector] public Animator animator;
        [SerializeField] List<SpriteRenderer> renderers;

        // Start is called before the first frame update
        protected override void Setup()
        {
            body = GetComponentInChildren<Rigidbody2D>();
            animator = GetComponent<Animator>();
        }
    }
}

