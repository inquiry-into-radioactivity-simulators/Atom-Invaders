using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class Thinger : MonoBehaviour
{
    public Texture2D[] tex;
    private Vector3 pos;
    private Vector3 scale;
    private Vector3 last;
    private bool zoom;
    public virtual void Start()
    {
        this.pos = this.transform.localPosition;
        this.scale = this.transform.localScale;
        this.zoom = Status.zoomed;
    }

    public virtual void Update()
    {
        if ((this.last == this.transform.position) || (this.zoom != Status.zoomed))
        {
            UnityEngine.Object.Destroy(this.gameObject);
        }
        if ((Projectile) this.transform.parent.GetComponent(typeof(Projectile)))
        {
            this.GetComponent<Renderer>().material.mainTexture = this.tex[((Projectile) this.transform.parent.GetComponent(typeof(Projectile))).type];
        }
        this.transform.eulerAngles = new Vector3(0, 180, 0);
        this.transform.localPosition = this.pos * (((1 - Status.zoomAmt) + 0.003f) / 1.003f);
        this.transform.localScale = this.scale * (((1 - Status.zoomAmt) + 0.003f) / 1.003f);
        this.last = this.transform.position;
    }

    public Thinger()
    {
        this.tex = new Texture2D[4];
    }

}