using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS2D
{
    [CreateAssetMenu(fileName = "Product", menuName = "ScriptableObjects/Product", order = 1)]
    public class Product : ScriptableObject {
        public GameObject Prefab;
        public float Duration;
        public float Cost;
    }
    }

