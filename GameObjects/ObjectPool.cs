using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
	GameObject prefab;
	List<GameObject> pool;
	public bool canGrow;

	public ObjectPool(GameObject prefab, bool canGrow, int size)
	{
		this.prefab = prefab;
		this.canGrow = canGrow;
		pool = new List<GameObject>();

		for (int i = 0; i < size; i++)
		{

			GameObject temp = GameObject.Instantiate(prefab);
			temp.SetActive(false);
			pool.Add(temp);
		}
	}

	public GameObject GetObject()
	{
		for (int i = 0; i < pool.Count; i++)
		{
			if (!pool[i].activeSelf)
			{
				pool[i].SetActive(true);
				return pool[i];
			}
		}
		if (canGrow)
		{
			GameObject temp = GameObject.Instantiate(prefab);
			//temp.SetActive(false);
			pool.Add(temp);
			return temp;
		}
		else
		{
			return null;
		}
	}
}

