using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(PlayerController))]
public class Wagon : MonoBehaviour
{
    [SerializeField] WagonConfig _config = null;

    // runtime
    Rigidbody2D _rgBody = null;
    List<WagonPart> _parts = new List<WagonPart>();
    Transform _partParent = null;
    Vector2 _velocity;
    PlayerController _ctrl;
/*    float _smoothTime;
    Vector2? _targetPos = null;*/

    bool _isInvincible = false;
    Bounds _wagonBounds; // aggregate bounds of the entire wagon
    public Vector2 WagonSize { get => _wagonBounds.size; }
    public Vector2 WagonCenter 
    { 
        get
        {
            Vector2 center = Vector2.zero;
            foreach (WagonPart part in _parts)
                center += new Vector2(part.transform.position.x, transform.position.y);
            return center / _parts.Count;
        }
    }

    public void SetTarget(Vector2 pos, float speed)
    {
        // _targetPos = pos;
        // _smoothTime = Vector2.Distance(pos, transform.position) / speed;

        _rgBody.MovePosition(pos);
    }

    public void UnsetTarget()
    {
        // _targetPos = null;
    }

    // add a wagon part to the right (append)
    public void AddPart(WagonPart prefab)
    {
        WagonPart last = _parts.Count > 0 ? _parts[_parts.Count - 1] : null;
        WagonPart new_part = Instantiate(prefab, _partParent);
        new_part.parent = this;

        Vector2 local_pos;

        if (last != null)
        {
            local_pos = last.transform.localPosition;
            float delta_x = last.Width / 2 + _config.HingeDist + new_part.Width / 2;
            local_pos.x += delta_x;

            // make sure wagon coordinate space is at the last wagon part
            Vector2 pos = _partParent.localPosition;
            pos.x -= delta_x;
            _partParent.localPosition = pos;
        }
        else
        {
            local_pos = Vector2.zero;
        }

        new_part.transform.localPosition = local_pos;

        // configure joints
        Rigidbody2D rigidbody = new_part.RigidBody;
        rigidbody.gravityScale = 0;
        rigidbody.angularDrag = _config.HingeAngularDrag;
        rigidbody.mass = _config.PartMass;

        HingeJoint2D joint = new_part.HingeJoint;
        joint.anchor = new Vector2(_config.HingeDist, 0);
        joint.useLimits = true;

        JointAngleLimits2D limits = joint.limits;
        limits.min = -_config.AngleRange / 2;
        limits.max = _config.AngleRange / 2;

        joint.limits = limits;

        if(last != null)
            last.HingeJoint.connectedBody = new_part.RigidBody;
        new_part.HingeJoint.connectedBody = _rgBody;

        _parts.Add(new_part);
    }

    // remove a wagon part from the right
    public bool RemoveEnd()
    {
        if (_parts.Count == 1 || _isInvincible)
            return false;

        TurnInvincible();
        
        WagonPart last = _parts[_parts.Count - 1];
        WagonPart new_last = _parts.Count >= 2 ? _parts[_parts.Count - 2] : null;

        // make the last part do a free fall
        Rigidbody2D rigidbody = last.RigidBody;
        rigidbody.gravityScale = 1;
        rigidbody.velocity = Vector2.zero;
        last.HingeJoint.enabled = false;
        last.gameObject.AddComponent<OutOfViewDestroyer>();

        _parts.RemoveAt(_parts.Count - 1);

        if (new_last != null)
            new_last.HingeJoint.connectedBody = _rgBody;
        
        RecalculateBounds();

        return true;
    }

    public int PartCount()
    {
        return _parts.Count;
    }

    void RecalculateBounds()
    {
        if(_parts.Count >= 1)
            _wagonBounds = _parts[0].Renderer.bounds;
        for(int i=1; i<_parts.Count; ++i)
            _wagonBounds.Encapsulate(_parts[i].Renderer.bounds);

        _ctrl.UpdateWagonBounds();
    }

    void TurnInvincible()
    {
        if (_isInvincible)
            return;

        IEnumerator _routine()
        {
            _isInvincible = true;

            foreach (WagonPart part in _parts)
            {
                Color col = part.Renderer.color;
                col.a *= 0.5f;
                part.Renderer.color = col;
            }

            yield return new WaitForSeconds(_config.InvicibleTimeOnCollision);

            foreach (WagonPart part in _parts)
            {
                Color col = part.Renderer.color;
                col.a *= 2f;
                part.Renderer.color = col;
            }
            _isInvincible = false;
        }

        StartCoroutine(_routine());
    }

    private void Awake()
    {
        _partParent = new GameObject("parts").transform;
        _partParent.parent = transform;
        _partParent.localPosition = new Vector2(-_config.HingeDist, 0);
        _ctrl = GetComponent<PlayerController>();
        _rgBody = gameObject.AddComponent<Rigidbody2D>();

        foreach (WagonPartDesc desc in _config.PartDescs)
        {
            for (int i = 0; i < desc.count; ++i)
                AddPart(desc.prefab);
        }

        RecalculateBounds();
    }

    // Start is called before the first frame update
    void Start()
    {
        float h = Camera.main.orthographicSize;
        Vector3 pos = Camera.main.transform.TransformPoint(new Vector2(0, h * 0.85f));
        pos.z = 10;
        transform.position = pos;

        _rgBody.gravityScale = 0;
        _rgBody.isKinematic = true;
    }

    private void FixedUpdate()
    {
        // add "gravity"
        Vector2 horizontalForce = new Vector2(-9.81f, 0);
        for (int i=0; i<_parts.Count; ++i)
        {
            _parts[i].RigidBody.AddForce(horizontalForce * _parts[i].RigidBody.mass);
        }

/*        if(_targetPos != null)
        {
            _rgBody.MovePosition(Vector2.SmoothDamp(transform.position, _targetPos.Value, ref _velocity, _smoothTime));
            Debug.DrawLine(_targetPos.Value, _targetPos.Value + Vector2.up, Color.red);
        }*/
    }

    // Update is called once per frame
    void Update()
    {
    }
}