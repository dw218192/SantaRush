using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugMgr : MonoBehaviour
{
#if UNITY_EDITOR
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

    private void OnGUI()
    {
        Rect r = new Rect(10, 10, 200, 100);
        GUI.Box(r, "Debug Menu");
        GUI.BeginGroup(r);
        
        const float butDist = 30;
        const float butWidth = 70;
        r = new Rect(10, 20, butWidth, 20);
        
        if(GUI.Button(r, "Test"))
        {
            if(wagonPartPrefab != null)
                GameObject.Find("Wagon").GetComponent<Wagon>().AddPart(wagonPartPrefab);
        }

        r.y += butDist;

        str = GUI.TextField(r, str);
        r.x += butWidth + 10;
        if (GUI.Button(r, "Set Time Scale"))
        {
            Time.timeScale = float.Parse(str);
        }
        r.x -= butWidth + 10;

        r.y += butDist;
        if(GUI.Button(r, "退出游戏"))
        {
            Application.Quit(0);
        }

        GUI.EndGroup();
    }
#endif
}
