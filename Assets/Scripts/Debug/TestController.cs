using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestController : MonoBehaviour
{
    [SerializeField] InputAction _moveUp = null;
    Camera _cam = null;
    Rigidbody2D _rigidBody;
    Vector2 _prevMousePos = Vector2.zero;
    WagonPart[] _parts;
    void OnEnable()
    {
        _moveUp.Enable();
        _moveUp.performed += Move;
    }
    void Move(InputAction.CallbackContext context)
    {
        IEnumerator DragRoutine()
        {
            while (!Mathf.Approximately(0, _moveUp.ReadValue<float>()))
            {
                Vector2 pos = Mouse.current.position.ReadValue();

                Vector2 worldDelta = _cam.ScreenToWorldPoint(pos) - _cam.ScreenToWorldPoint(_prevMousePos);
                _rigidBody.MovePosition((Vector2)transform.position + worldDelta);

                _prevMousePos = pos;
                yield return new WaitForFixedUpdate();
            }
        }

        _prevMousePos = Mouse.current.position.ReadValue();
        StartCoroutine(DragRoutine());
    }
    void Start()
    {
        _cam = Camera.main;
        _rigidBody = GetComponent<Rigidbody2D>();

        _parts = GetComponentsInChildren<WagonPart>();
        foreach(var part in _parts)
        {
            part.OnPartHurt.AddListener(OnCollide);
        }
    }

    bool _b = false;
    void OnCollide(Hitbox box)
    { 
        IEnumerator Unset()
        {
            if (_b) yield break;

            _b = true;
            yield return new WaitForSeconds(3);
            foreach (var part in _parts)
            {
                part.Renderer.color = Color.white;
            }
            _b = false;
        }
        foreach(var part in _parts)
        {
            part.Renderer.color = Color.red;
        }

        StartCoroutine(Unset());
    }
}
