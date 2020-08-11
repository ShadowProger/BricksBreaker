using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manybits {
    public class Ball : MonoBehaviour, IPoolObject
    {
        private Field field;
        private bool isDeff = false;
        private bool wasDeff = false;
        private bool isDouble = false;
        public GameObject shield;

        private SpriteRenderer spriteRenderer;
        private CircleCollider2D collider;
        private Rigidbody2D rigidbody;

        private Sprite sprite;
        private Coroutine moveCoroutine = null;
        private readonly Vector3 normalScale = new Vector3(1f, 1f, 1f);
        private readonly Vector3 smallScale = new Vector3(0.6f, 0.6f, 1f);

        // 2 - пауза
        // 1 - все остальное
        private int stopSimulationReason;



        public bool IsProtected
        {
            get => isDeff;
        }

        public Vector2 Velocity
        {
            get => rigidbody.velocity;
            set => rigidbody.velocity = value;
        }

        

        void Awake() {
            spriteRenderer = GetComponent<SpriteRenderer>();
            collider = GetComponent<CircleCollider2D>();
            rigidbody = GetComponent<Rigidbody2D>();
        }



        public void Init(Field field, Sprite sprite)
        {
            this.field = field;
            this.sprite = sprite;

            spriteRenderer.sprite = sprite;
        }


        private void FixedUpdate()
        {
            if (rigidbody.velocity != Vector2.zero)
            {
                if (field.GetBorders().Contains(rigidbody.position) == false)
                {
                    Debug.Log("!!!        ball is beyond the borders");
                    rigidbody.velocity = Vector2.zero;
                    OnArrive();
                }
            }
            rigidbody.angularVelocity = 1360f;
        }
        


        public void SetDeff()
        {
            if (wasDeff == false)
            {
                isDeff = true;
                wasDeff = true;
                shield.SetActive(true);
            }
        }



        public void RemoveDeff()
        {
            if (isDeff == true)
            {
                isDeff = false;
                shield.SetActive(false);
            }
        }



        public float GetRadius()
        {
            return collider.radius;
        }



        public void MoveForward(Vector2 angle, float speed)
        {
            rigidbody.velocity = angle * speed;
        }



        public void SetPosition(Vector2 position)
        {
            rigidbody.MovePosition(position);
        }



        public Vector2 GetPosition()
        {
            return rigidbody.position;
        }



        public void StopPhisicSimulation()
        {
            rigidbody.velocity = Vector2.zero;
            rigidbody.simulated = false;
            stopSimulationReason = 1;
        }



        public void SetPhisicSimulation(bool isSimulated)
        {
            if (isSimulated)
            {
                if (stopSimulationReason == 2)
                {
                    rigidbody.simulated = true;
                    stopSimulationReason = 0;
                }
            }
            else
            {
                if (stopSimulationReason == 0)
                {
                    rigidbody.simulated = false;
                    stopSimulationReason = 2;
                }
            }
            
        }



        public void Stop()
        {
            StopPhisicSimulation();
            GoToNextStartPosition();
        }



        public void GoToNextStartPosition()
        {
            bool isFirstSaved = field.IsFirstBallPositionSaved;

            field.SaveBallPosition(this);

            if (isFirstSaved == false)
            {
                transform.position = field.GetNextStart();
                OnArrive();
            }
            else
            {
                Vector2 position = rigidbody.position;
                position.y = field.ballStart.transform.position.y;
                transform.position = position;

                MoveTo(field.GetNextStart(), 0.2f, OnArrive);
            }
        }



        public void OnSpawn()
        {
            moveCoroutine = null;
            rigidbody.simulated = true;
            stopSimulationReason = 0;
            isDeff = false;
            wasDeff = false;
            isDouble = false;
            transform.localScale = normalScale;
            // Сделать чтобы устанавливался спрайт выбранного в магазине шарика
            //spriteRenderer.sprite = sprite;
            shield.SetActive(false);
        }



        public void OnDespawn()
        {
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
        }


        public delegate void Reaction();
        public void MoveTo(Vector2 position, float time, Reaction reaction)
        {
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
                moveCoroutine = null;
            }
                
            moveCoroutine = StartCoroutine(Move(position, time, reaction));
        }
        


        IEnumerator Move(Vector2 position, float time, Reaction reaction)
        {
            float t = 0f;

            Vector2 pos = transform.position;

            while (((Vector2)transform.position - position).sqrMagnitude > Vector2.kEpsilon)
            {
                if (Field.FieldState != FieldState.Pause)
                {
                    transform.position = Vector2.Lerp(pos, position, t);

                    t += Time.deltaTime / time;

                    yield return null;
                }
                else
                {
                    yield return new WaitWhile(() => { return Field.FieldState == FieldState.Pause; });
                }
            }

            reaction();
        }



        void OnArrive()
        {
            field.ShowBallNextStart();
            field.OnBallArrive(this);
        }



        public void SetDouble(Vector2 position)
        {
            if (isDouble == false)
            {
                Vector2 oldVelocity = rigidbody.velocity;
                Vector2 left = Quaternion.AngleAxis(45f, Vector3.forward) * oldVelocity;
                Vector2 right = Quaternion.AngleAxis(-45f, Vector3.forward) * oldVelocity;
                rigidbody.MovePosition(position);
                rigidbody.velocity = left;
                OnDouble();

                Ball ball = field.CreateBall(position, right.normalized, 0f);
                ball.Velocity = right;
                ball.OnDouble();
                if (isDeff)
                {
                    ball.SetDeff();
                }
            }
        }



        public void OnDouble()
        {
            if (isDouble == false)
            {
                isDouble = true;
                transform.localScale = smallScale;
            }
        }
    }
}
