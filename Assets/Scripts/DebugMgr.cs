using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System;

[Obsolete("no longer used for debugging")]
public class DebugMgr : MonoBehaviour
{
    string str = "1";

    public WagonPart wagonPartPrefab = null;
    public GameObject go = null;
    public InputAction testScreenToWorld = null;

    void TestScreenToWorld(InputAction.CallbackContext context)
    {
        if (go == null) return;

        Vector2 pos = Mouse.current.position.ReadValue();
        pos = Camera.main.ScreenToWorldPoint(pos);
        var ins = Instantiate(go);
        ins.transform.position = pos;
        StartCoroutine(DelayDestroy(ins));
    }
    
    IEnumerator DelayDestroy(GameObject ins)
    {
        yield return new WaitForSecondsRealtime(5f);
        Destroy(ins);
    }

    private void OnEnable()
    {
        testScreenToWorld.Enable();
        testScreenToWorld.performed += TestScreenToWorld;
    }

    private void OnDisable()
    {
        testScreenToWorld.Disable();
        testScreenToWorld.performed -= TestScreenToWorld;
    }

    private void Awake()
    {
        if (!GameConsts.debugMgr)
        {
            GameConsts.debugMgr = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {

    }

#if UNITY_EDITOR
    const int DRAW_BUF_SIZE=256;
    int _gizmosPointsIdx = 0;
    Vector2[] _gizmosPoints = new Vector2[DRAW_BUF_SIZE];
#endif
    void DEBUG_DrawGizmos()
    {
#if UNITY_EDITOR
        for(int i=0; i< _gizmosPointsIdx; ++i)
            Gizmos.DrawSphere(_gizmosPoints[i], 1);
#endif
    }

    private void OnDrawGizmos()
    {
        DEBUG_DrawGizmos();
    }

    #region Public Interface
    public void DEBUG_GizmosDrawPoint(Vector2 coord)
    {
#if UNITY_EDITOR
        if(_gizmosPointsIdx == DRAW_BUF_SIZE)
        {
            Debug.LogWarning("DebugMgr: gizmos point draw buffer limit reached", this);
            return;
        }
        _gizmosPoints[_gizmosPointsIdx++] = coord;
#endif
    }
    #endregion
}
