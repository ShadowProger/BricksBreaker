using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Manybits
{
    public class NoVideoAvailableInterface : Window
    {
        public GameManager gameManager;



        public void OnOKClick()
        {
            Close();
        }



        public override void Open()
        {
            base.Open();
            gameObject.SetActive(true);
        }



        public override void Close()
        {
            base.Close();
            gameObject.SetActive(false);
        }
    }
}
