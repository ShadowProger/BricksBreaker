using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;

namespace Manybits
{
    public class Brick : Block
    {
        public SpriteRenderer spriteRenderer;
        public TextMeshPro textMesh;
        public List<Color> colors;
        public List<Color> doubleColors;
        public GameObject particlesPrefab;

        private BoxCollider2D collider;
        private Rigidbody2D rigidbody;
        private Animator animator;
        private int life;
        private bool isDouble;

        private Transform particlesParent;



        void Awake()
        {
            collider = GetComponent<BoxCollider2D>();
            rigidbody = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            blockType = BlockType.Brick;
        }



        public void Init(Field field, int life, bool isDouble = false)
        {
            this.isDouble = isDouble;
            this.field = field;
            this.life = life;
            particlesParent = field.ParticlesParent;
            field.BrickPointCount += life;
            SetColor(life);
            UpdateText();
        }



        void SetColor(int value)
        {
            if (value <= 0)
                return;

            int index = isDouble ? (value - 1) % doubleColors.Count : (value - 1) % colors.Count;

            if (isDouble)
            {
                spriteRenderer.color = doubleColors[index];
            }
            else
            {
                spriteRenderer.color = colors[index];
            }
        }



        public Vector2 GetSize()
        {
            return collider.size;
        }



        public override void MoveDown(float dist, float time)
        {
            StartCoroutine(Move(new Vector2(rigidbody.position.x, rigidbody.position.y - dist), time));
        }



        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ball"))
            {
                Hit();
            }
        }



        private void UpdateText()
        {
            textMesh.text = "" + life;
        }



        public override void Hit()
        {
            if (life > 0)
            {
                life--;
                SetColor(life);
                UpdateText();

                field.BrickPointCount--;
                if (GameManager.gameMode == GameMode.DailyChallenge)
                {
                    field.Score++;
                }

                if (life == 0)
                {
                    field.RemoveBlock(this);
                }
            }
        }



        public override void OnBreak()
        {
            PlayParticles();

            if (life < 0)
            {
                life = 0;
            }

            field.BrickPointCount -= life;
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
            field.CanTurn = true;

            if (FieldPosition.y >= field.Height - 1)
            {
                field.SetGameOver(true);
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
            block.SetAttribute("type", isDouble ? "DoubleBrick" : "Brick");
            block.SetAttribute("life", life.ToString());

            return block;
        }



        private void PlayParticles()
        {
            GameObject particles = Instantiate(particlesPrefab, particlesParent, false);
            ParticleSystem particleSystem = particles.GetComponent<ParticleSystem>();

            particles.transform.position = rigidbody.position;

            ParticleSystem.MainModule main = particleSystem.main;
            main.startColor = spriteRenderer.color;

            particleSystem.Play();
        }



        public override void DoAction(string action)
        {
            switch (action)
            {
                case "damage":
                    GetDamage();
                    break;
            }
        }



        private void GetDamage()
        {
            PlayParticles();
            if (life > 1)
            {
                int lost = Mathf.RoundToInt((float)life / 3);
                life -= lost;
                field.BrickPointCount -= lost;
            }
            UpdateText();
            SetColor(life);
        }
    }
}
