using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject _prefab = null;
    [SerializeField] Transform _spawnSocket = null;

    public GameObject Spawn()
    {
        GameObject ins = Instantiate(_prefab);
        ins.transform.position = _spawnSocket.position;
        return ins;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
