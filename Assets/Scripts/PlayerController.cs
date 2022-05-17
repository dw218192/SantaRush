using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Wagon))]
public class PlayerController : MonoBehaviour, IResolutionScaleHandler
{
    Camera _cam = null;
    float _timer = 0f;
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
    WagonConfig Config
    {
        get => Wagon.Config;
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
        Config.MoveUp.Enable();
        Config.MoveUp.performed += MoveUp;

        Config.DeployGift.Enable();
        Config.DeployGift.performed += DeployGift;
    }

    private void OnDisable()
    {
        Config.MoveUp.Disable();
        Config.MoveUp.performed -= MoveUp;

        Config.DeployGift.Disable();
        Config.DeployGift.performed -= DeployGift;
    }

    private void Awake()
    {
    }

    public void UpdateWagonBounds()
    {
        Vector2 viewMin = GameConsts.worldCameraMin;
        Vector2 viewMax = GameConsts.worldCameraMax;

        // take HUD into account
        viewMax.y -= HUD.Instance.Height;

        // Vector2 viewSize = new Vector2(Cam.orthographicSize * Cam.aspect, Cam.orthographicSize);
        Vector2 offset = (Vector2)Wagon.transform.position - Wagon.WagonCenter;

        _wagonMoveBounds[0] = viewMin + Wagon.WagonSize / 2 + offset;
        _wagonMoveBounds[1] = viewMax - Wagon.WagonSize / 2 + offset;
    }

    // Start is called before the first frame update
    void Start()
    {
        _cam = Camera.main;
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
        if(_timer >= Config.DeplpyCooldown)
        {
            Wagon.SpawnGift();
            _timer = 0;
        }
    }

    Vector2 GetPointerPos()
    {
        if (GameConsts.IsOnMobile())
            return Touchscreen.current.position.ReadValue();
        else
            return Mouse.current.position.ReadValue();
    }

    void MoveUp(InputAction.CallbackContext context)
    {
        _prevMousePos = GetPointerPos();
        StartCoroutine(DragRoutine());
    }

    IEnumerator DragRoutine()
    {
        Debug.Log("starting drag");

        while(!Mathf.Approximately(0, Config.MoveUp.ReadValue<float>()))
        {
            Vector2 pos = GetPointerPos();

            float percentY = Mathf.Abs(pos.y - _prevMousePos.y) / Screen.height;
            float percentX = Mathf.Abs(pos.x - _prevMousePos.x) / Screen.width;
            float percent = Mathf.Max(percentX, percentY);
            if (percent > Config.Tolerance)
            {
                float speed = Config.Speed * (1 + percent);
                Vector2 worldDelta = Cam.ScreenToWorldPoint(pos) - Cam.ScreenToWorldPoint(_prevMousePos);
                
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
        }
    }

    public void OnResolutionScale(ResolutionScaleEventData eventData)
    {
        UpdateWagonBounds();
    }
#endif
}
