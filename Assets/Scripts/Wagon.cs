using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

[RequireComponent(typeof(PlayerController), typeof(HurtboxGroup))]
public class Wagon : MonoBehaviour, IBuffStateHandler
{
    [SerializeField] WagonConfig _config = null;
    [SerializeField] Transform _giftSpawnSocket;

    // runtime
    Rigidbody2D _rgBody = null;
    List<WagonPart> _parts = new List<WagonPart>();
    Transform _partParent = null;

    PlayerController _ctrl;

    Inventory _giftInventory = new Inventory();

    /*  
        float _smoothTime;
        Vector2? _targetPos = null;
        Vector2 _velocity;
    */
    Coroutine _collisionInvisibleRoutine = null;
    Coroutine _superStatusRoutine = null;

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

    public WagonConfig Config { get => _config; }

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

        // configure hurtbox
        new_part.gameObject.layer = gameObject.layer;

        // apply effects
        if (_superStatusRoutine != null)
            DoSuperStatusEffect(new_part, true);
        else if (_collisionInvisibleRoutine != null)
            DoInvisibleCollisionEffect(new_part, true);

        _parts.Add(new_part);
    }

    // remove a wagon part from the right
    public bool RemoveEnd()
    {
        if (_parts.Count == 1)
            return false;

        WagonPart last = _parts[_parts.Count - 1];
        WagonPart new_last = _parts.Count >= 2 ? _parts[_parts.Count - 2] : null;

        // make the last part do a free fall
        Rigidbody2D rigidbody = last.RigidBody;
        rigidbody.gravityScale = 1;
        rigidbody.velocity = Vector2.zero;
        last.HingeJoint.enabled = false;
        last.gameObject.AddComponent<OutOfViewDestroyer>();

        // disable hurtbox on last part
        last.Hurtbox.enabled = false;

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

    IEnumerator TurnInvincibleRoutine(float time, Action<WagonPart, bool> doEffect, Action callback)
    {
        foreach (WagonPart part in _parts)
            doEffect(part, false);

        yield return new WaitForSeconds(time);

        foreach (WagonPart part in _parts)
            doEffect(part, true);

        callback?.Invoke();
    }

    private void Awake()
    {
        _partParent = new GameObject("parts").transform;
        _partParent.parent = transform;
        _partParent.localPosition = new Vector2(-_config.HingeDist, 0);
        _ctrl = GetComponent<PlayerController>();
        _rgBody = gameObject.AddComponent<Rigidbody2D>();

        foreach (var desc in _config.PartDescs)
        {
            for (int i = 0; i < desc.count; ++i)
                AddPart(desc.prefab);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        RecalculateBounds();

        float h = Camera.main.orthographicSize;
        h -= HUD.Instance.Height;

        Vector3 pos = Camera.main.transform.TransformPoint(new Vector2(0, h * 0.85f));
        pos.z = 10;
        transform.position = pos;

        _rgBody.gravityScale = 0;
        _rgBody.isKinematic = true;
    
        foreach(GiftDesc desc in _config.InitGiftDescs)
        {
            _giftInventory.Add(desc.type, desc.count);
        }
    }

    public void SpawnGift()
    {

    }

    private void FixedUpdate()
    {
        // add "gravity"
        Vector2 horizontalForce = new Vector2(-9.81f, 0);
        for (int i=0; i<_parts.Count; ++i)
        {
            _parts[i].RigidBody.AddForce(horizontalForce * _parts[i].RigidBody.mass);
        }

/*      
 *      if(_targetPos != null)
        {
            _rgBody.MovePosition(Vector2.SmoothDamp(transform.position, _targetPos.Value, ref _velocity, _smoothTime));
            Debug.DrawLine(_targetPos.Value, _targetPos.Value + Vector2.up, Color.red);
        }*/
    }

    void TakeDamage()
    {
        IEnumerator _deathRoutine()
        {
            if(_config.DeathEffectPrefab != null)
            {
                Color c = _parts[0].Renderer.color;
                c.a = 0;
                _parts[0].Renderer.color = c;


                _ctrl.CtrlEnabled = false;

                GameObject effectIns = Instantiate(_config.DeathEffectPrefab.gameObject, _parts[0].transform.position, Quaternion.identity);
                Animator animator = effectIns.GetComponent<Animator>();

                while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1 || animator.IsInTransition(0))
                {
                    yield return null;
                }
            }
            GameConsts.eventManager.InvokeEvent(typeof(IWagonCollisionHandler), new WagonCollisionEventData(PartCount()));
        }

        if (_collisionInvisibleRoutine == null)
        {
            _collisionInvisibleRoutine = StartCoroutine(TurnInvincibleRoutine(_config.InvicibleTimeOnCollision, 
                DoInvisibleCollisionEffect, () => { _collisionInvisibleRoutine = null; }));
            RemoveEnd();
            if (PartCount() > 1)
            {
                GameConsts.eventManager.InvokeEvent(typeof(IWagonCollisionHandler), new WagonCollisionEventData(PartCount()));
            }
            else
            {
                StartCoroutine(_deathRoutine());
            }
        }
    }

    [HurtboxHandler]
    public void OnWagonCollide(Hitbox inflictor)
    {
        if (inflictor.IsInLayer(GameConsts.k_GiftLayerName))
        {
            GiftInstance gift = inflictor.GetComponent<GiftInstance>();

            Debug.Assert(gift != null, this);
            
            GameConsts.gameManager.AddScore(gift.GiftType.GetScore());
        }
        else if (inflictor.IsInLayer(GameConsts.k_NPCLayerName))
        {
            // kill NPC and gets its gift reward during super status
            if(_superStatusRoutine != null)
            {
                NPCPart npc = inflictor.GetComponent<NPCPart>();

                Debug.Assert(npc != null, this);

                // if not a bomb only NPC
                if(!npc.Owner.AlwaysBomb)
                    GameConsts.gameManager.AddScore(npc.Owner.NpcType.GiftType.GetScore());
                
                npc.Owner.Die(true);
            }
            else
            {
                TakeDamage();
            }
        }
        else
        {
            if (_superStatusRoutine == null)
                TakeDamage();
        }

        DEBUG_CheckInvariant();
    }

    void DoInvisibleCollisionEffect(WagonPart part, bool undo)
    {
        Color c = part.Renderer.color;
        
        if (undo)
            c.a *= 2;
        else
            c.a *= 0.5f;
    
        part.Renderer.color = c;
    }

    void DoSuperStatusEffect(WagonPart part, bool undo)
    {
        if (undo)
            part.Renderer.color = Color.white;
        else
            part.Renderer.color = Color.red;
    }

    public void OnBuffStateChange(BuffStateEventData eventData)
    {
        if(eventData.buffEnabled)
        {
            switch (eventData.desc.Type)
            {
                case PlayerBuffType.HP_REWARD:
                    {
                        uint numParts = (uint)_parts.Count;
                        uint maxNumParts = 0;

                        WagonPart newPartPrefab = null;
                        foreach (var desc in _config.PartDescs)
                        {
                            newPartPrefab = desc.prefab;
                            maxNumParts += desc.count;
                            if (maxNumParts >= numParts)
                                break;
                        }

                        if (numParts == maxNumParts)
                            return;
                        else if (numParts < maxNumParts)
                        {
                            AddPart(newPartPrefab);
                        }
                        else
                            Debug.Break();
                        break;
                    }
                case PlayerBuffType.SUPER_STATUS:
                    {
                        if (_superStatusRoutine != null)
                            StopCoroutine(_superStatusRoutine);

                        if (_collisionInvisibleRoutine != null)
                        {
                            StopCoroutine(_collisionInvisibleRoutine);
                            _collisionInvisibleRoutine = null;
                        }

                        _superStatusRoutine = StartCoroutine(TurnInvincibleRoutine(eventData.desc.Duration, DoSuperStatusEffect, ()=> { _superStatusRoutine = null; }));
                        break;
                    }
                default:
                    Debug.LogError("unknown buff type received!");
                    Debug.Break();
                    break;
            }
        }
        else
        {
            switch (eventData.desc.Type)
            {
                case PlayerBuffType.HP_REWARD:
                    break;
                case PlayerBuffType.SUPER_STATUS:
                    break;
                default:
                    Debug.LogError("unknown buff type received!");
                    Debug.Break();
                    break;
            }
        }

        DEBUG_CheckInvariant();
    }

    [Conditional("DEBUG")]
    void DEBUG_CheckInvariant()
    {
        bool b1 = _superStatusRoutine != null;
        bool b2 = _collisionInvisibleRoutine != null;

        Debug.Assert(!(b1 && b2), this);
        if (b1 && b2)
            Debug.Break();
    }
}