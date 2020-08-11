using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manybits
{
    public interface IPoolObject
    {
        void OnSpawn();
        void OnDespawn();
    }
}
