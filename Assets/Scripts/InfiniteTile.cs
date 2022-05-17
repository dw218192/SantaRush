using System;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;
using Random = System.Random;

[RequireComponent(typeof(Scroller))]
public class InfiniteTile : MonoBehaviour, IResolutionScaleHandler
{
    struct TileInstance
    {
        public GameObject ins;
        public float width;
    };


    [SerializeField] TilePool _pool = null;
    [SerializeField] bool _useGameStageSpeed = false;
    [DisableIf("_useGameStageSpeed")]
    [SerializeField] float _speed = 0;
    [SerializeField] int _sortOrder = 0;
    [SerializeField] bool _isGenerative = false;

    bool _valid = true;
    Camera _cam = null;
    Scroller _scroller = null;
    float _viewWidth = 0;

    LinkedList<TileInstance> _tileInstances = new LinkedList<TileInstance>();

    TileInstance GenTile(float x)
    {
        TilePool.TileDesc type = _pool.GetNextTile();
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

    void Awake()
    {
        _cam = Camera.main;
        _viewWidth = GameConsts.worldCameraMax.x - GameConsts.worldCameraMin.x;
        transform.position = GameConsts.worldCameraMin;
        /*
        float viewHeight = _cam.orthographicSize * 2;
        Vector2 pos = transform.position;
        pos.x = _cam.transform.position.x - _viewWidth / 2;
        pos.y = _cam.transform.position.y - viewHeight / 2;
        transform.position = pos;
        */
        _scroller = GetComponent<Scroller>();
    }

    // Start is called before the first frame update
    void Start()
    {
        float total = -_viewWidth;
        float target = _viewWidth;

        while (total < target)
        {
            int i;
            for (i = 0; i < _pool.Items.Length && total < target; ++i)
            {
                TileInstance tile = GenTile(total);
                _tileInstances.AddLast(tile);
                total += tile.width;
            }
        }

        // gen an extra tile to avoid artifacts
        _tileInstances.AddLast(GenTile(total));
        _valid = _tileInstances.Count > 1;


        // begin scroll
        _scroller.BaseSpeed = _useGameStageSpeed ? -GameConsts.gameManager.StageTable.InitScrollSpeed : -_speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_valid) return;

        if(!_isGenerative)
        {
            TileInstance first = _tileInstances.First.Value;
            if(first.ins.transform.position.x + first.width / 2 < GameConsts.worldCameraMin.x - _viewWidth / 2)
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
            if (first.ins.transform.position.x + first.width / 2 < GameConsts.worldCameraMin.x - _viewWidth / 2)
            {
                TileInstance last = _tileInstances.Last.Value;

                Destroy(first.ins);
                _tileInstances.RemoveFirst();
                _tileInstances.AddLast(GenTile(last.ins.transform.localPosition.x + last.width / 2));
            }
        }
    }

    public void OnResolutionScale(ResolutionScaleEventData eventData)
    {
        _viewWidth = GameConsts.worldCameraMax.x - GameConsts.worldCameraMin.x;
        transform.position = GameConsts.worldCameraMin;

        foreach (var tile in _tileInstances)
            Destroy(tile.ins);
        _tileInstances.Clear();

        float total = -_viewWidth;
        float target = _viewWidth;
        while (total < target)
        {
            int i;
            for (i = 0; i < _pool.Items.Length && total < target; ++i)
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
