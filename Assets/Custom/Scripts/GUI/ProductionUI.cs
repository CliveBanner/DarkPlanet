using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RTS2D
{
    public class ProductionUI : MonoBehaviour
    {
        [SerializeField] Text m_ProductText;
        [SerializeField] Image m_ProgressImage;
        [SerializeField] Text m_ProduceText;
        [SerializeField] Image m_ProduceImage;
        [SerializeField] Button m_ButtonHexagon;
        [SerializeField] Button m_ButtonCircle;
        [SerializeField] Button m_ButtonSquare;
        [SerializeField] Button m_ButtonTriangle;
        IProducing m_Production;
        Vector3 m_ProgressImageOrigin;
        bool m_Produce;

        void Start() {
            m_Produce = m_Production.IsProducing();
            UpdateButtons();
        }

        public void SetProduction(IProducing a_Production) {
            m_Production = a_Production;
            m_ProgressImageOrigin = m_ProgressImage.transform.position;
        }

        public void OnProduceCirclePressed() {
            m_Production.ChangeProduct(ProductName.Circle);
            m_ButtonHexagon.interactable = true;
            m_ButtonCircle.interactable = false;
            m_ButtonSquare.interactable = true;
            m_ButtonTriangle.interactable = true;
        }

        public void OnProduceSquarePressed() {
            m_Production.ChangeProduct(ProductName.Square);
            m_ButtonHexagon.interactable = true;
            m_ButtonCircle.interactable = true;
            m_ButtonSquare.interactable = false;
            m_ButtonTriangle.interactable = true;
        }

        public void OnProduceTrianglePressed() {
            m_Production.ChangeProduct(ProductName.Triangle);
            m_ButtonHexagon.interactable = true;
            m_ButtonCircle.interactable = true;
            m_ButtonSquare.interactable = true;
            m_ButtonTriangle.interactable = false;
        }

        public void OnProduceHexagonPressed() {
            m_Production.ChangeProduct(ProductName.Hexagon);
            m_ButtonHexagon.interactable = false;
            m_ButtonCircle.interactable = true;
            m_ButtonSquare.interactable = true;
            m_ButtonTriangle.interactable = true;
        }

        public void OnToggleProductionPressed() {
            m_Produce = !m_Produce;
        }

        void UpdateProgress(float a_Progress) {
            m_ProgressImage.transform.localScale = new Vector3(a_Progress, 1f, 1f);
        }

        void UpdateProducing() {
            if(m_Produce) {
                m_ProduceText.text = "Stop";
                m_ProduceImage.color = Color.red;
            }else {
                m_ProduceText.text = "Start";
                m_ProduceImage.color = Color.green;
            }
        }

        void UpdateButtons() {
            switch (m_Production.GetCurrentProduct()) {
                case ProductName.Hexagon: {
                    m_ButtonHexagon.interactable = false;
                    m_ButtonCircle.interactable = true;
                    m_ButtonSquare.interactable = true;
                    m_ButtonTriangle.interactable = true;
                    break;
                }
                case ProductName.Circle: {
                    m_ButtonHexagon.interactable = true;
                    m_ButtonCircle.interactable = false;
                    m_ButtonSquare.interactable = true;
                    m_ButtonTriangle.interactable = true;
                    break;
                }
                case ProductName.Square: {
                    m_ButtonHexagon.interactable = true;
                    m_ButtonCircle.interactable = true;
                    m_ButtonSquare.interactable = false;
                    m_ButtonTriangle.interactable = true;
                    break;
                }
                case ProductName.Triangle: {
                    m_ButtonHexagon.interactable = true;
                    m_ButtonCircle.interactable = true;
                    m_ButtonSquare.interactable = true;
                    m_ButtonTriangle.interactable = false;
                    break;
                }
                default: break;
            }
        }
        
        void Update() {
            if(m_Production != null) {
                m_ProductText.text = Enum.GetName(typeof(ProductName), m_Production.GetCurrentProduct());
                UpdateProgress(m_Production.GetProgress());
                UpdateProducing();
                m_Production.Produce(m_Produce);
            }   
        }
    }
}
