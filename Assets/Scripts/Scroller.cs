using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroller : MonoBehaviour, ILevelStageHandler
{
    [SerializeField] float _speed = 0;
    [SerializeField] bool _usePhysics = false;
    Rigidbody2D _rb;

    bool _valid;
    float _initSpeed;
    public float speed 
    { 
        get 
        { 
            return _speed; 
        } 
        set 
        { 
            _speed = value; 
            _valid = !Mathf.Approximately(_speed, 0);
            if(_usePhysics)
                _rb.velocity = Vector2.right * _speed;
        } 
    }

    private void Awake()
    {
        if(_usePhysics)
        {
            _rb = gameObject.GetComponent<Rigidbody2D>();
            if(_rb == null)
                _rb = gameObject.AddComponent<Rigidbody2D>();
        }
    }

    void Start()
    {
        _initSpeed = _speed;
        speed = _speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_valid) return;

        if (!_usePhysics)
        {
            Vector2 move = Vector2.right * speed * Time.deltaTime;
            transform.Translate(move);
        }
    }

    public void OnGameStageEnter(LevelStageEventData eventData)
    {
        speed = _initSpeed * eventData.speedMultiplier;
    }
}