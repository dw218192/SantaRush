using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Wagon), typeof(Spawner))]
public class PlayerController : MonoBehaviour
{
    [Range(0, 1)] [Tooltip("minimum delta/screen_height percentage above which the wagon starts to accept input")]
    [SerializeField] float _tolerance = 0.1f;
    [SerializeField] InputAction _moveUp = null;
    [SerializeField] InputAction _deployGift = null;
    [SerializeField] float _speed = 1f;
    [SerializeField] float _deplpyCooldown = 1f;

    Camera _cam = null;
    float _timer = 0f;
    Spawner _giftSpawner = null;
    Wagon _wagon = null;
    Vector2 _prevMousePos = Vector2.zero;

    Vector2[] _wagonMoveBounds = new Vector2[2];

    Wagon Wagon 
    { 
        get {
            if (!_wagon)
                _wagon = GetComponent<Wagon>();
            return _wagon;
        }
        set => _wagon = value; 
    }
    Camera Cam
    {
        get
        {
            if (!_cam)
                _cam = Camera.main;
            return _cam;
        }
        set => _cam = value;
    }

    private void OnEnable()
    {
        _moveUp.Enable();
        _moveUp.performed += MoveUp;

        _deployGift.Enable();
        _deployGift.performed += DeployGift;
    }

    private void OnDisable()
    {
        _moveUp.Disable();
        _moveUp.performed -= MoveUp;

        _deployGift.Disable();
        _deployGift.performed -= DeployGift;
    }

    private void Awake()
    {
        _giftSpawner = GetComponent<Spawner>();
    }

    public void UpdateWagonBounds()
    {
        Vector2 viewSize = new Vector2(Cam.orthographicSize * Cam.aspect, Cam.orthographicSize);
        Vector2 offset = (Vector2)Wagon.transform.position - Wagon.WagonCenter;
        _wagonMoveBounds[0] = (Vector2)Cam.transform.TransformPoint(-viewSize) + Wagon.WagonSize / 2 + offset;
        _wagonMoveBounds[1] = (Vector2)Cam.transform.TransformPoint(viewSize) - Wagon.WagonSize / 2 + offset;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
    }

    void FixedUpdate()
    {

    }

    void DeployGift(InputAction.CallbackContext context)
    {
        if(_timer >= _deplpyCooldown)
        {
            _giftSpawner.Spawn();
            _timer = 0;
        }
    }

    void MoveUp(InputAction.CallbackContext context)
    {
        _prevMousePos = Mouse.current.position.ReadValue();
        StartCoroutine(DragRoutine());
    }

    IEnumerator DragRoutine()
    {
        Debug.Log("starting drag");

        while(!Mathf.Approximately(0, _moveUp.ReadValue<float>()))
        {
            Vector2 pos = Mouse.current.position.ReadValue();

            float percentY = Mathf.Abs(pos.y - _prevMousePos.y) / Screen.height;
            float percentX = Mathf.Abs(pos.x - _prevMousePos.x) / Screen.width;
            float percent = Mathf.Max(percentX, percentY);
            if (percent > _tolerance)
            {
                float speed = _speed * (1 + percent);
                Vector2 worldDelta = _cam.ScreenToWorldPoint(pos) - _cam.ScreenToWorldPoint(_prevMousePos);
                
                // set target for the wagon
                Vector2 target = new Vector2(
                    Mathf.Clamp(_wagon.transform.position.x + worldDelta.x, _wagonMoveBounds[0].x, _wagonMoveBounds[1].x),
                    Mathf.Clamp(_wagon.transform.position.y + worldDelta.y, _wagonMoveBounds[0].y, _wagonMoveBounds[1].y));
                
                _wagon.SetTarget(target, speed);
            }

            _prevMousePos = pos;
            yield return new WaitForFixedUpdate();
        }

        Debug.Log("terminating drag");
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying)
        {
            Gizmos.DrawCube(_wagon.WagonCenter, _wagon.WagonSize);
            Vector2 viewSize = new Vector2(_cam.orthographicSize * _cam.aspect, _cam.orthographicSize);
            Vector2 offset = (Vector2)_wagon.transform.position - _wagon.WagonCenter;
            Vector2 viewMin = (Vector2)_cam.transform.TransformPoint(-viewSize) + _wagon.WagonSize / 2 + offset;
            Vector2 viewMax = (Vector2)_cam.transform.TransformPoint(viewSize) - _wagon.WagonSize / 2 + offset;

            Gizmos.DrawSphere(viewMin, 0.1f);
            Gizmos.DrawSphere(viewMax, 0.1f);
        }
    }
#endif
}
