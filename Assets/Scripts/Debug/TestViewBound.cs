// Draw a yellow sphere at top-right corner of the near plane
// for the selected camera in the Scene view.
using UnityEngine;
using System.Collections;

public class TestViewBound : MonoBehaviour
{
    void OnDrawGizmosSelected()
    {
        Camera camera = GetComponent<Camera>();
        Vector3 p1 = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));
        Vector3 p2 = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(p1, 0.1F);
        Gizmos.DrawSphere(p2, 0.1F);
    }
}