using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfViewDestroyer : MonoBehaviour
{
    Camera _cam = null;

    // Start is called before the first frame update
    void Start()
    {
        _cam = Camera.main;
    }

    Vector2 GetHalfViewDimension()
    {
        return new Vector2(_cam.orthographicSize * _cam.aspect, _cam.orthographicSize);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 camSpacePos = _cam.transform.InverseTransformPoint(transform.position);
        Vector2 view = GetHalfViewDimension();
        if (camSpacePos.y < -view.y || camSpacePos.y > view.y || camSpacePos.x < -view.x || camSpacePos.x > view.x)
            Destroy(gameObject);
    }
}