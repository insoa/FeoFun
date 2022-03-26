using System;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectPool {
	
	namespace ObjectPool {
		[Serializable]
		public class Pool {
			public string Tag;
			public GameObject Prefab;
			public int Size;
		}

		public sealed class ObjectPooler : MonoBehaviour {
		
			public List<Pool> Pools;
			public Dictionary<string, Queue<GameObject>> poolDictionary;

			#region Singleton
			public static ObjectPooler Instance;
		
			private void Awake() {
				Instance = this;
			}
			#endregion

			private void Start() {
				poolDictionary = new Dictionary<string, Queue<GameObject>>();

				foreach (var pool in Pools) {
					Queue<GameObject> objectPool = new Queue<GameObject>();

					for (int i = 0; i < pool.Size; i++) {
						GameObject obj = Instantiate(pool.Prefab);
						obj.SetActive(false);
						objectPool.Enqueue(obj);
					}
					poolDictionary.Add(pool.Tag, objectPool);
				}
			}

			public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation) {
				if (!poolDictionary.ContainsKey(tag)) {
					return null;
				}

				GameObject objectToSpawn = poolDictionary[tag].Dequeue();
			
				objectToSpawn.SetActive(true);
				objectToSpawn.transform.position = position;
				objectToSpawn.transform.rotation = rotation;

				poolDictionary[tag].Enqueue(objectToSpawn);

				return objectToSpawn;
			}
		}
	}
}