using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Manybits
{
    public class TouchController : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public CameraManager cameraManager;
        public Field field;

        private Vector2 angle;

        private bool isTouchDown = false;
        private bool canShot = false;
        Vector2 touchDownPos;
        private const float minAngleY = 0.1f;

        Vector2 startPos;
        Vector2 startHelpLine;
        Vector2 endHelpLine;

        const int pointerDefaultID = -10;
        int pointerID = pointerDefaultID;


        void Start()
        {
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            collider.size = new Vector2(cameraManager.ScreenWidth, cameraManager.ScreenHeight);
        }



        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerId != pointerID)
            {
                return;
            }

            if (Field.FieldState == FieldState.PlayerTurn && isTouchDown == true)
            {
                Vector2 dragPos = eventData.position;
                angle = (dragPos - touchDownPos).normalized;

                endHelpLine = Camera.main.ScreenToWorldPoint(eventData.position);
                field.SetHelpLine(startHelpLine, endHelpLine);
                field.SetHelpLineVisible(true);

                if (angle.y >= 0f)
                {
                    if (angle.y < minAngleY)
                    {
                        angle.y = minAngleY;
                    }
                    canShot = true;

                    int layerMask = LayerMask.GetMask("Border");

                    RaycastHit2D hit = Physics2D.Raycast(startPos, angle, Mathf.Infinity, layerMask);

                    Vector2 endPos = startPos + angle * 100;

                    if (hit.transform != null)
                    {
                        endPos = hit.point;
                    }

                    field.SetAimLine(startPos, endPos);
                    field.SetAimLineVisible(true);
                }
                else
                {
                    canShot = false;
                    field.SetAimLineVisible(false);
                }
            }
        }



        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log($"[Down] pointerID: {eventData.pointerId}");

            if (pointerID == pointerDefaultID) 
            {
                pointerID = eventData.pointerId;
            }
            else
            {
                return;
            }

            if (Field.FieldState == FieldState.BeforeStart)
            {
                field.SetState(FieldState.PlayerTurn);
            }

            if (Field.FieldState == FieldState.PlayerTurn)
            {
                isTouchDown = true;
                startPos = field.BallPosition;
                touchDownPos = eventData.position;
                startHelpLine = Camera.main.ScreenToWorldPoint(eventData.position);
            }
        }



        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log($"[Up] pointerID: {eventData.pointerId}");

            if (eventData.pointerId != pointerID)
            {
                return;
            }
            else
            {
                pointerID = pointerDefaultID;
            }

            if (Field.FieldState == FieldState.PlayerTurn)
            {
                isTouchDown = false;
                field.SetHelpLineVisible(false);

                if (canShot)
                {
                    canShot = false;
                    field.SetAimLineVisible(false);
                    field.ThrowBalls(angle);
                }
            }
        }
    }
}
