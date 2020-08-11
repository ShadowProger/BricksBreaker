using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Manybits
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class Block : MonoBehaviour
    {
        public Field field;
        protected Vector2Int fieldPosition;
        protected BlockType blockType;


        
        public Vector2Int FieldPosition
        {
            get => fieldPosition;

            set => fieldPosition = value;
        }

        public BlockType BlockType
        {
            get => blockType;
        }

        public abstract void OnCreate();
        public abstract void MoveDown(float dist, float time);
        public abstract Rigidbody2D GetRigidbody();
        public virtual void OnBreak() {}
        public virtual XmlNode Serialize(XmlDocument xml) { return null; }
        public virtual bool NeedDelete() { return false; }
        public virtual void Hit() { }
        public virtual void DoAction(string action) { }
    }
}
