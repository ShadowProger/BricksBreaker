using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manybits
{
    public class Pool<T> : MonoBehaviour where T : MonoBehaviour, IPoolObject
    {
        private Stack<T> items;

        private GameObject prefab;
        private Transform parentPool;



        public Pool(GameObject prefab, int size)
        {
            GameObject poolsGO = GameObject.Find("[POOLS]");

            if (!poolsGO)
                poolsGO = new GameObject("[POOLS]");

            GameObject poolGO = new GameObject("Pool:" + typeof(T).Name);
            poolGO.transform.SetParent(poolsGO.transform);

            parentPool = poolGO.transform;
            this.prefab = prefab;

            items = new Stack<T>(size);
            for (int i = 0; i < size; i++)
            {
                T item = CreateObject(Vector3.zero, Quaternion.identity, parentPool);
                item.gameObject.SetActive(false);
                item.OnDespawn();
                items.Push(item);
            }
        }



        public T Spawn(Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion), Transform parent = null, bool isLocalPosition = false)
        {
            if (items.Count > 0)
            {
                T item = items.Pop();
                item.transform.SetParent(parent, false);
                item.transform.rotation = rotation;
                item.gameObject.SetActive(true);
                if (isLocalPosition)
                    item.transform.localPosition = position;
                else
                    item.transform.position = position;
                item.OnSpawn();
                return item;
            }

            T createdObject = CreateObject(position, rotation, parent);
            createdObject.OnSpawn();
            return createdObject;
        }



        public void Despawn(T item)
        {
            item.gameObject.SetActive(false);
            items.Push(item);
            item.OnDespawn();
            item.transform.SetParent(parentPool, false);
        }



        private T CreateObject(Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion), Transform parent = null)
        {
            GameObject go = Instantiate(prefab, position, rotation, parent);
            if (parent)
                go.transform.localPosition = position;
            else
                go.transform.position = position;
            return go.GetComponent<T>();
        }
    }
}
