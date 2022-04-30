using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * component to move an object
 * Note: all velocity is negative!!!
 * because the world moves backwards in this game
 * instead of the player moving forward!
 */
public class Scroller : MonoBehaviour, ILevelStageHandler
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
    float _extraSpeedIncrease = 0;

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

    public float ActualSpeed
    {
        get => _initSpeed * _multiplier - _extraSpeedIncrease;
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
            Vector2 move = Vector2.right * ActualSpeed * Time.deltaTime;
            transform.Translate(move);
        }
    }

    public void OnGameStageEnter(LevelStageEventData eventData)
    {
        _multiplier = eventData.speedMultiplier;
        _extraSpeedIncrease = eventData.extraSpeedIncrease;

        if (_usePhysics)
            Rb.velocity.Set(ActualSpeed, Rb.velocity.y);
    }
}