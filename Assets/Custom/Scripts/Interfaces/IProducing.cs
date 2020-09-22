using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RTS2D
{
    public interface IProducing 
    {
        void ChangeProduct(ProductName a_Product);
        void Produce(bool a_Produce);
        ProductName GetCurrentProduct();
        float GetProgress();
        bool IsProducing();
    }
}
