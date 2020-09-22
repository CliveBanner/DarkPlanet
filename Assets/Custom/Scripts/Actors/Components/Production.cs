using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RTS2D
{
    public enum ProductName {
        Hexagon,
        Circle,
        Triangle,
        Square
    }

    public class Production : ActorComponent, IProducing {
        [Header("Products")]
        [SerializeField] Product m_ProductHexagon;
        [SerializeField] Product m_ProductCircle;
        [SerializeField] Product m_ProductSquare;
        [SerializeField] Product m_ProductTriangle;

        [Header("Production")]
        [SerializeField] Transform m_Spawn;
        [SerializeField] Vector3 m_RallyPoint;
        [SerializeField] ProductName m_CurrentProductName;

        float m_CurrentTime;
        Transform m_DefaultSpawn;
        bool m_IsProducing;
        bool m_Produce;

        GameObject m_Prefab;
        float m_Duration = 1f;
        float m_Cost = 1f;
        
        /// overrides /////////////////////////////////////////////////////////////////////////////

        protected override void Setup() {
            m_CurrentTime = 0f;
            m_DefaultSpawn = m_Spawn;
            m_IsProducing = false;
            m_Produce = false;
        }

        public override string GetPropertyString() {
            return "PRODUCT: "+m_CurrentProductName.ToString();
        }

        void Update() {
            StartProduction();
            Produce();
        }

        /// broadcast receivers ////////////////////////////////////////////////////////////////////////////////

        public void OnMove(Vector3 a_Point) {
            m_Spawn.position = a_Point;
        }

        public void OnQueryMove(Vector3 a_Point) {
            m_Spawn.position = a_Point;
        }

        public void OnAttackMove(Vector3 a_Point) {
            m_Spawn.position = a_Point;
        }

        public void OnHold() {
            m_Spawn.position = m_DefaultSpawn.position;
        }

        /// IProducing ///////////////////////////////////////////////////////////////////////////////////

        public void ChangeProduct(ProductName a_Product) {
            InterruptProduction();
            m_CurrentProductName = a_Product;
        }

        public void Produce(bool a_Produce) {
            m_Produce = a_Produce;
            if(!m_Produce && m_IsProducing) {
                InterruptProduction();
            }
        }

        public ProductName GetCurrentProduct() {
            return m_CurrentProductName;
        }

        public float GetProgress() {
            return m_CurrentTime / m_Duration;
        }

        public bool IsProducing() {
            return m_Produce;
        }

        /// production /////////////////////////////////////////////////////////////////////////////

        void StartProduction() {
            if(m_Produce && !m_IsProducing) {
                ApplyCurrentProduct();
                if(GetActor().player.CanPayMinerals(m_Cost)) {
                    GetActor().player.PayMinerals(m_Cost);
                    m_IsProducing = true;
                }
            }
        }

        void InterruptProduction() {
            if(m_IsProducing) {
                m_IsProducing = false;
                GetActor().player.AddMinerals(m_Cost);
                m_CurrentTime = 0f;
            }
        }

        void Produce() {
            if(m_IsProducing) {
                m_CurrentTime += Time.deltaTime;
                if(m_CurrentTime >= m_Duration) {
                    m_CurrentTime = 0f;
                    Spawn();
                    m_IsProducing = false;
                }
            }
            else {
                m_CurrentTime = 0f;
            }
            BroadcastMessage("OnProduce", m_CurrentTime / m_Duration);
        }

        void ApplyCurrentProduct() {
            if(m_CurrentProductName == ProductName.Circle) ApplyProduct(m_ProductCircle);
            else if(m_CurrentProductName == ProductName.Square) ApplyProduct(m_ProductSquare);
            else if(m_CurrentProductName == ProductName.Triangle) ApplyProduct(m_ProductTriangle);
            else if(m_CurrentProductName == ProductName.Hexagon) ApplyProduct(m_ProductHexagon);
        }

        void ApplyProduct(Product a_Product) {
            m_Cost = a_Product.Cost;
            m_Duration = a_Product.Duration;
            m_Prefab =  a_Product.Prefab;
        }

        void Spawn() {
            Vector3 l_SpawnPosition = m_Spawn.position + new Vector3(Random.value, Random.value, 0f);
            GameObject l_Object = Instantiate(m_Prefab, GetActor().player.transform);
            l_Object.transform.position = l_SpawnPosition;
            l_Object.transform.rotation =  m_Spawn.rotation;
            BroadcastMessage("OnComplete", l_SpawnPosition);
        }
    }
}
