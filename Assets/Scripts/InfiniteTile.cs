using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[Serializable]
class TileDesc : ILotteryItem
{
    public GameObject prefab;
    public int weight;
    public int GetWeight()
    {
        return weight;
    }
};

struct TileInstance
{
    public GameObject ins;
    public float width;
};

public class InfiniteTile : MonoBehaviour
{
    [SerializeField] TileDesc[] _tileTypes = null;
    [SerializeField] float _speed = 0;
    [SerializeField] int _sortOrder = 0;
    [SerializeField] bool _isGenerative = false;
    [SerializeField] bool _mergeColliders = false;

    bool _valid = true;
    Camera _cam = null;
    Scroller _scroller = null;
    float _viewWidth = 0;
    LinkedList<TileInstance> _tileInstances = new LinkedList<TileInstance>();
    int _idx = 0;

    // only used in random tile generation
    Lottery _lottery = null;

    TileDesc GetNextTileType()
    {
        if(!_isGenerative)
        {
            if (_idx + 1 == _tileTypes.Length)
                _idx = 0;
            else
                ++_idx;
            return _tileTypes[_idx];
        }
        else
        {
            return (TileDesc)_lottery.NextItem();
        }
    }

    TileInstance GenTile(float x)
    {
        TileDesc type = GetNextTileType();
        TileInstance tileIns = new TileInstance();
        tileIns.ins = Instantiate(type.prefab, transform);
        tileIns.ins.transform.localPosition = Vector2.zero;

        SpriteRenderer[] rends = tileIns.ins.GetComponentsInChildren<SpriteRenderer>();
        Bounds bounds = new Bounds(tileIns.ins.transform.position, Vector3.zero);
        foreach(SpriteRenderer rend in rends)
        {
            rend.sortingOrder = _sortOrder;
            bounds.Encapsulate(rend.bounds);
        }

        tileIns.width = bounds.size.x;
        tileIns.ins.transform.localPosition = new Vector2(x + tileIns.width / 2, transform.position.y - bounds.min.y);

        return tileIns;
    }

    private void Awake()
    {
        _cam = Camera.main;
        _viewWidth = _cam.orthographicSize * 2 * _cam.aspect;
        float viewHeight = _cam.orthographicSize * 2;

        Vector2 pos = transform.position;
        pos.x = _cam.transform.position.x - _viewWidth / 2;
        pos.y = _cam.transform.position.y - viewHeight / 2;
        transform.position = pos;

        if (!gameObject.TryGetComponent<Scroller>(out _scroller))
            _scroller = gameObject.AddComponent<Scroller>();
        _scroller.speed = _speed;

        if (_isGenerative)
        {
            _lottery = new Lottery(_tileTypes);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        float total = 0;
        float target = _viewWidth;
        if(_tileTypes.Length == 0)
        {
            _valid = false;
        }
        else
        {
            int i = 0;
            while (total < target)
            {
                for (i = 0; i < _tileTypes.Length && total < target; ++i)
                {
                    TileInstance tile = GenTile(total);
                    _tileInstances.AddLast(tile);
                    total += tile.width;
                }
            }

            // gen an extra tile to avoid artifacts
            _tileInstances.AddLast(GenTile(total));
            _valid = _tileInstances.Count > 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_valid) return;

        if(!_isGenerative)
        {
            TileInstance first = _tileInstances.First.Value;
            if(first.ins.transform.position.x + first.width / 2 < _cam.transform.position.x - _viewWidth / 2)
            {
                TileInstance last = _tileInstances.Last.Value;
                
                Vector2 pos = last.ins.transform.localPosition;
                pos.x += last.width / 2 + first.width / 2;
                first.ins.transform.localPosition = pos;

                _tileInstances.RemoveFirst();
                _tileInstances.AddLast(first);
            }
        }
        else
        {
            TileInstance first = _tileInstances.First.Value;
            if (first.ins.transform.position.x + first.width / 2 < _cam.transform.position.x - _viewWidth / 2)
            {
                TileInstance last = _tileInstances.Last.Value;

                Destroy(first.ins);
                _tileInstances.RemoveFirst();
                _tileInstances.AddLast(GenTile(last.ins.transform.localPosition.x + last.width / 2));
            }
        }
    }
}
