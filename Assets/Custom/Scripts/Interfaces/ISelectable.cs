using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS2D {
    public interface ISelectable {
        string GetID();
        string GetPropertyString();
        void Select();
        void Deselect();
        bool Destroyed();
        GameObject GetObject();
    }
}


