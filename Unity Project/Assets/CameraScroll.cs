using UnityEngine;
using System.Collections;

public class CameraScroll : MonoBehaviour {

    Vector3 prevPos = Vector3.zero;
    private float min = 30f;
    private float ROTSpeed = 2f;
    private float max = 60f;

    float curZoomPos, zoomTo;
    float zoomFrom = 60f;

                          

    void LateUpdate () {

	    if (Input.GetMouseButtonDown(0))
	    {
	        var t = Input.mousePosition;
	        t.z = 63;
            prevPos = Camera.main.ScreenToWorldPoint(t);

        }
	    if (Input.GetMouseButton(0))
	    {
            var t = Input.mousePosition;
            t.z = 63;
            var pos = Camera.main.ScreenToWorldPoint(t);
            Debug.Log(pos + " " + prevPos);
            pos.y  = transform.position.y;
	        prevPos.y = pos.y;

            transform.Translate(prevPos - pos,Space.World);


	    }

        float y = Input.mouseScrollDelta.y;
        if (y >= 1)
        {
            zoomTo = 5f;
        }
        else if (y <= -1)
        {
            zoomTo = -5f;
        }

        Camera.main.fieldOfView = Mathf.Clamp(zoomTo  + Camera.main.fieldOfView,20f,60f ) ;
        zoomTo = 0;

    }
}
