using UnityEngine;

namespace Utilities
{
    public class ObjectPool<T> where T : MonoBehaviour
    {
        private T prefab;
        private T[] pool;
        private bool[] isActive;
        private Transform parent;
        private int size;

        public ObjectPool(T prefab, int size, Transform parent)
        {
            this.prefab = prefab;
            this.size = size;
            this.parent = parent;
            this.pool = new T[size];
            this.isActive = new bool[size];
        
            InitializePool();
        }
        private void InitializePool()
        {
            for(int i = 0; i < size; i++)
            {
                CreateNewObject(i);
            }
        }
        private void CreateNewObject(int index)
        {
            T newObject = GameObject.Instantiate(prefab, parent);
            newObject.gameObject.SetActive(false);
            pool[index] = newObject;
            isActive[index] = false;
        }

        public T Get()
        {
            for(int i = 0; i < size; i++)
            {
                if(!isActive[i])
                {
                    isActive[i] = true;
                    return pool[i];
                }
            }
            return null;
        }

        public void Return(T obj)
        {
            for(int i = 0; i < size; i++)
            {
                if(pool[i] == obj)
                {
                    isActive[i] = false;
                    obj.gameObject.SetActive(false);
                    return;
                }
            }
        }
        public void ReturnAll()
        {
            for(int i = 0; i < size; i++)
            {
                if(pool[i] != null)
                {
                    pool[i].gameObject.SetActive(false);
                    isActive[i] = false;
                }
            }
        }
    }
}