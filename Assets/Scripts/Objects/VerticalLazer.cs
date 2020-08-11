using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Manybits
{
    public class VerticalLazer : Block
    {
        private Rigidbody2D rigidbody;
        private Animator animator;
        public LineRenderer verticalLine;

        bool needDelete = false;



        void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            blockType = BlockType.Lazer;
        }



        public void Init(Field field)
        {
            this.field = field;
            UpdateLinePosition();
            verticalLine.gameObject.SetActive(false);
        }



        public override Rigidbody2D GetRigidbody()
        {
            return rigidbody;
        }



        public override void MoveDown(float dist, float time)
        {
            StartCoroutine(Move(new Vector2(rigidbody.position.x, rigidbody.position.y - dist), time));
        }



        void OnArrive()
        {
            fieldPosition.y++;
            UpdateLinePosition();

            if (FieldPosition.y >= field.Height - 1)
            {
                field.RemoveBlock(this);
            }
        }



        public override void OnCreate()
        {
            animator.SetTrigger("Create");
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



        void OnCreateAnimationEnd()
        {
            field.OnBlockCreateAnimationEnd();
        }



        public override XmlNode Serialize(XmlDocument xml)
        {
            XmlElement block = xml.CreateElement("block");

            block.SetAttribute("x", fieldPosition.x.ToString());
            block.SetAttribute("y", fieldPosition.y.ToString());
            block.SetAttribute("type", "VerticalLazer");

            return block;
        }



        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Ball"))
            {
                Ball ball = collision.gameObject.GetComponent<Ball>();
                animator.SetTrigger("Lazer");
                field.HitVertical(fieldPosition.x);
                needDelete = true;
            }
        }



        public override bool NeedDelete()
        {
            return needDelete;
        }



        private void UpdateLinePosition()
        {
            Rect borders = field.GetBorders();
            verticalLine.SetPosition(0, new Vector3(transform.position.x, borders.yMin, 1f));
            verticalLine.SetPosition(1, new Vector3(transform.position.x, borders.yMax, 1f));
        }
    }
}
