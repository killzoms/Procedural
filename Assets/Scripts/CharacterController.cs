using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Procedural.Gamelogic
{
    class CharacterController : MonoBehaviour
    {

        private void Start()
        {

        }

        public float horizontalSpeed;
        public float verticalSpeed;


        private void Update()
        {
            float h = horizontalSpeed * Input.GetAxis("Mouse X");
            float v = verticalSpeed * -Input.GetAxis("Mouse Y");

            Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            transform.position += moveDirection;

            transform.eulerAngles = new Vector3(v + transform.eulerAngles.x, h + transform.eulerAngles.y, 0);
        }
    }
}
