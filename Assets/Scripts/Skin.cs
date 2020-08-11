using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manybits
{
    public class Skin : MonoBehaviour
    {
        public int id;
        public string ballName;
        public SkinStatus status;
        public Sprite sprite;
        public int price;
    }

    public enum SkinStatus { Bought, Buy, Ads }
}
