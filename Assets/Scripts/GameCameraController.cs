using UnityEngine;
using System.Collections;

public class GameCameraController : MonoBehaviour {

    public Camera m_Camera; // the camera
    public GameObject[] targets; // targets to encapsulate (must have a collider)
    public float cameraSmoothTime = 0.15f; // smoothing time for camera zoom
    public Transform origin; // origin point (0, 0, 0)
    public float minimumDistance; // minimum distance from targets
    public float maximumDistance; // maximum distance from targets (Mathf.Infinity if < minimumDistance)
    public Vector2 padding; // padding around gameobjects in visible viewport
    public Vector2 offsetCenter; // offset the camera center
    public bool lookAtCenter; // rotate to look at center of all targets along origin

	void Start () {
        // set up camera
        if (m_Camera == null) {
            m_Camera = Camera.main;
        }
	}
	
    // cumulative velocity
    private Vector3 cameraVelocity;
    void Update()
    {
        // quick return
        if (targets.Length == 0) return;
        // bounding rect to encapsulate all targets
        Rect viewport = new Rect();
        // set up initial viewport for 1 target
        {
            var target = targets[0].GetComponent<Collider>().bounds;
            var center = target.center;
            var extent = target.extents;
            viewport.xMin = center.x - extent.x;
            viewport.xMax = center.x + extent.x;
            viewport.yMin = center.y - extent.y;
            viewport.yMax = center.y + extent.y;
        }
        // add in the other targets
        for (var i = 1; i < targets.Length; ++i)
        {
            if (targets[i] == null) continue;
            var target = targets[i].GetComponent<Collider>().bounds;
            var center = target.center;
            var extent = target.extents;

            var lowX = center.x - extent.x;
            var highX = center.x + extent.x;
            if (lowX < viewport.xMin) viewport.xMin = lowX;
            if (highX > viewport.xMax) viewport.xMax = highX;

            var lowY = center.y - extent.y;
            var highY = center.y + extent.y;
            if (lowY < viewport.yMin) viewport.yMin = lowY;
            if (highY > viewport.yMax) viewport.yMax = highY;
        }

        // desired height
        var frustumHeight = Mathf.Max(viewport.height + padding.y, (viewport.width + padding.x) / m_Camera.aspect);
        // required distance for desired height
        var distance = Mathf.Clamp(frustumHeight * 0.5f / Mathf.Tan(m_Camera.fieldOfView * 0.5f * Mathf.Deg2Rad), minimumDistance, maximumDistance > minimumDistance ? maximumDistance : Mathf.Infinity);
        
        // smooth camera zoom
        var targetCamPos = new Vector3(viewport.center.x + offsetCenter.x, viewport.center.y + offsetCenter.y, origin.position.z - distance);
        var currentCamPos = m_Camera.transform.position;
        m_Camera.transform.position = Vector3.SmoothDamp(currentCamPos, targetCamPos, ref cameraVelocity, cameraSmoothTime);
        
        // look at center if necessary
        if (lookAtCenter) {
            m_Camera.transform.LookAt(new Vector3(viewport.center.x, viewport.center.y, origin.position.z));
        }
    }
}
