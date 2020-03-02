using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class FollowCam : MonoBehaviour
{
    public GameObject proj;
    public Transform camt;
    public Camera cam;
    public RenderTexture tex;
    public Renderer texdisplay;
    public Renderer pointerRenderer;
    public Transform pointer;
    public BoxCollider bounds;
    public float startTime;
    public virtual void Start()
    {
        GameObject cobj = new GameObject("FollowCam");
        this.cam = (Camera) cobj.AddComponent(typeof(Camera));
        this.cam.backgroundColor = Color.black;
        this.tex = new RenderTexture(128, 128, 16);
        this.tex.isPowerOfTwo = true;
        this.cam.targetTexture = this.tex;
        this.texdisplay.material.mainTexture = this.tex;
        this.camt = cobj.transform;
        Bounds bo = new Bounds(Vector3.zero, new Vector3(1, 2, 1));
        bo.SetMinMax(Camera.main.ViewportPointToRay(new Vector3(0, 0, 0.03f)).origin, Camera.main.ViewportPointToRay(new Vector3(1, 1, 0.03f)).origin);
        GameObject colobj = new GameObject("collider");
        this.bounds = (BoxCollider) colobj.AddComponent(typeof(BoxCollider));
        this.bounds.isTrigger = true;
        this.bounds.center = bo.center;
        this.bounds.size = bo.size;
        this.startTime = Time.time;
    }

    public virtual void LateUpdate()
    {
        if (!this.proj)
        {
            UnityEngine.Object.Destroy(this.gameObject);
            UnityEngine.Object.Destroy(this.tex);
            UnityEngine.Object.Destroy(this.bounds);
            return;
        }
        this.camt.position = this.proj.transform.position + new Vector3(0, 0.67f, 0);
        this.camt.eulerAngles = new Vector3(90, 0, 0);
        this.texdisplay.transform.eulerAngles = new Vector3(270, 180, 0);
        float angle = Mathf.Atan2(this.proj.transform.position.z, this.proj.transform.position.x);
        float s = (this.bounds.size.x + this.bounds.size.z) * 0.4f;
        var viewPortProjectile = Camera.main.WorldToViewportPoint(this.proj.transform.position);
        
        if (viewPortProjectile.x < 0 || viewPortProjectile.x > 1 || viewPortProjectile.y < 0 || viewPortProjectile.y > 1)
        {
            this.pointer.transform.position = this.bounds.ClosestPointOnBounds(this.bounds.center + ((this.proj.transform.position - this.bounds.center).normalized * s));
        }
        else
        {
            this.pointer.transform.position = Vector3.one * 9999;
        }
        this.pointer.transform.rotation = Quaternion.LookRotation(Vector3.Scale(this.proj.transform.position - this.pointer.transform.position, new Vector3(1, 0, 1)));
   
        if(!Status.zoomed) {
            UnityEngine.Object.Destroy(proj);
            UnityEngine.Object.Destroy(bounds.gameObject);
            UnityEngine.Object.Destroy(cam.gameObject);
            UnityEngine.Object.Destroy(gameObject);
        }
    }


    
}