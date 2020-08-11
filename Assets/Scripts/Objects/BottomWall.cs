using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manybits
{
    public class BottomWall : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ball"))
            {
                Ball ball = collision.gameObject.GetComponent<Ball>();
                if (ball.IsProtected)
                {
                    ball.RemoveDeff();
                }
                else
                {
                    ball.Stop();
                }
            }
        }
    }
}
