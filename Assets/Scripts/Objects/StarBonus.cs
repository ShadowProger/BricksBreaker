using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Manybits
{
    public class StarBonus : Block
    {
        public GameObject particlesPrefab;

        private Rigidbody2D rigidbody;
        private Animator animator;

        private Transform particlesParent;



        void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            blockType = BlockType.StarBonus;
        }



        public void Init(Field field)
        {
            this.field = field;

            particlesParent = field.ParticlesParent;
        }



        public override void MoveDown(float dist, float time)
        {
            StartCoroutine(Move(new Vector2(rigidbody.position.x, rigidbody.position.y - dist), time));
        }



        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Ball"))
            {
                field.AddStars(1);
                field.RemoveBlock(this);
            }
        }



        public override void OnBreak()
        {
            GameObject particles = Instantiate(particlesPrefab, particlesParent, false);
            ParticleSystem particleSystem = particles.GetComponent<ParticleSystem>();

            particles.transform.position = rigidbody.position;

            particleSystem.Play();
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
            block.SetAttribute("type", "StarBonus");

            return block;
        }
    }
}
