using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Manybits
{
    public class ShieldBonus : Block
    {
        private Rigidbody2D rigidbody;
        private Animator animator;

        bool needDelete = false;



        void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            blockType = BlockType.ShieldBonus;
        }



        public void Init(Field field)
        {
            this.field = field;
        }



        public override void MoveDown(float dist, float time)
        {
            StartCoroutine(Move(new Vector2(rigidbody.position.x, rigidbody.position.y - dist), time));
        }



        public override Rigidbody2D GetRigidbody()
        {
            return rigidbody;
        }



        IEnumerator Move(Vector2 newPos, float time)
        {
            float t = 0f;
            Vector2 pos = rigidbody.position;

            while (rigidbody.position != newPos)
            {
                rigidbody.position = Vector2.Lerp(pos, newPos, t);
                t += Time.deltaTime / time;
                yield return null;
            }

            OnArrive();
        }



        void OnArrive()
        {
            fieldPosition.y++;

            if (FieldPosition.y >= field.Height - 1)
            {
                field.RemoveBlock(this);
            }
        }



        public override void OnCreate()
        {
            animator.SetTrigger("Create");
        }



        void OnCreateAnimationEnd()
        {
            field.OnBlockCreateAnimationEnd();
        }



        public override XmlNode Serialize(XmlDocument xml)
        {
            XmlElement block = xml.CreateElement("block");

            block.SetAttribute("x", fieldPosition.x.ToString());
            block.SetAttribute("y", fieldPosition.y.ToString());
            block.SetAttribute("type", "ShieldBonus");

            return block;
        }



        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Ball"))
            {
                Ball ball = collision.gameObject.GetComponent<Ball>();
                ball.SetDeff();
                needDelete = true;
            }
        }



        public override bool NeedDelete()
        {
            return needDelete;
        }
    }
}
