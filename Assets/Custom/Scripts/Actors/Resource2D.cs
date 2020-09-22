using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RTS2D
{
    public class Resource2D : Actor2D, IGatherable
    {
        public void Gather(float a_Amount) {
            BroadcastMessage("OnGather", a_Amount);
        }
    }
}
