using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
    private T prefab;
    private T[] pool;
    private bool[] isActive;
    private Transform parent;

    public ObjectPool(T prefab, int size, Transform parent)
    {
        this.prefab = prefab;
        this.parent = parent;
        this.pool = new T[size];
        this.isActive = new bool[size];
        
        InitializePool();
    }

    private void InitializePool()
    {
        for(int i = 0; i < pool.Length; i++)
        {
            pool[i] = GameObject.Instantiate(prefab, parent);
            pool[i].gameObject.SetActive(false);
            isActive[i] = false;
        }
    }

    public T Get()
    {
        for(int i = 0; i < pool.Length; i++)
        {
            if(!isActive[i])
            {
                isActive[i] = true;
                pool[i].gameObject.SetActive(true);
                return pool[i];
            }
        }
        return null; 
    }

    public void Return(T obj)
    {
        for(int i = 0; i < pool.Length; i++)
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
        for(int i = 0; i < pool.Length; i++)
        {
            isActive[i] = false;
            pool[i].gameObject.SetActive(false);
        }
    }
}