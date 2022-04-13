using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroller : MonoBehaviourEx, ILevelStageHandler
{
    [SerializeField] float _initSpeed = 0;
    [SerializeField] bool _usePhysics = false;
    Rigidbody2D _rb;

    Rigidbody2D Rb
    {
        get
        {
            if (_usePhysics)
            {
                _rb = gameObject.GetComponent<Rigidbody2D>();
                if (_rb == null)
                    _rb = gameObject.AddComponent<Rigidbody2D>();
            }
            return _rb;
        }
    }

    bool _valid = true;
    float _multiplier = 1;

    public float BaseSpeed
    {
        get
        {
            return _initSpeed;
        }
        set
        {
            _initSpeed = value;

            _valid = !Mathf.Approximately(_initSpeed, 0);
            _multiplier = GameConsts.gameManager.TileSpeedMultiplier;

            if (_valid)
            {
                if (_usePhysics)
                    Rb.velocity.Set(_initSpeed * _multiplier, Rb.velocity.y);
            }
        }
    }

    void Start()
    {
        BaseSpeed = _initSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_valid) return;

        if (!_usePhysics)
        {
            Vector2 move = Vector2.right * _initSpeed * _multiplier * Time.deltaTime;
            transform.Translate(move);
        }
    }

    public void OnGameStageEnter(LevelStageEventData eventData)
    {
        _multiplier = eventData.speedMultiplier;

        if (_usePhysics)
            Rb.velocity.Set(_initSpeed * _multiplier, Rb.velocity.y);
    }
}