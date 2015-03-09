using UnityEngine;
using System.Collections;

public class ViewportCameraController : MonoBehaviour {

    public GameObject[] viewportTargets;
    public float cameraSmoothTime = 0.15f;
    public Transform origin;
    public float minimumDistance;
    public float maximumDistance;
    public Vector2 padding;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    private Vector3 cameraVelocity;
	void Update () {
        if (viewportTargets.Length == 0) return;
        Rect viewport = new Rect();
        for (var i = 0; i < viewportTargets.Length; ++i) {
            if (viewportTargets[i] == null) continue;
            var target = viewportTargets[i].GetComponent<Collider> ().bounds;
            var center = target.center;
            var extent = target.extents;

            var lowX = center.x;
            var highX = center.x;
            if (lowX < viewport.xMin) viewport.xMin = lowX;
            if (highX > viewport.xMax) viewport.xMax = highX;

            var lowY = center.y - extent.y;
            var highY = center.y + extent.y;
            if (lowY < viewport.yMin) viewport.yMin = lowY;
            if (highY > viewport.yMax) viewport.yMax = highY;
        }
        var frustumHeight = Mathf.Max (viewport.height + padding.y, (viewport.width + padding.x) / Camera.main.aspect);
        var distance = Mathf.Clamp(frustumHeight * 0.5f / Mathf.Tan (Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad), minimumDistance, maximumDistance > minimumDistance ? maximumDistance : Mathf.Infinity);
        var targetCamPos = new Vector3 (viewport.center.x, viewport.center.y, origin.position.z - distance);
        var currentCamPos = Camera.main.transform.position;
        Camera.main.transform.position = Vector3.SmoothDamp(currentCamPos, targetCamPos, ref cameraVelocity, cameraSmoothTime);
	}
}
