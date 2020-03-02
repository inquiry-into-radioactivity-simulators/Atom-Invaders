using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class Orbital : MonoBehaviour
{
    public static ArrayList instances;
    public int index;
    public bool show;
    public int electrons;
    public bool secondPass;
    public ArrayList planes;
    private float radius;
    public virtual void Start()
    {
        SphereCollider sphere = this.GetComponent<SphereCollider>();
        if(sphere != null) {
            this.radius = sphere.radius;
        } else {
            CapsuleCollider capsule = this.GetComponent<CapsuleCollider>();
            this.radius = capsule.radius;
        }
        
        var inChildren = this.GetComponentsInChildren<MeshRenderer>();
        var self = this.GetComponent<MeshRenderer>();
        planes = new ArrayList();
        if(self) {
            planes.Add(self.gameObject);
        }
        foreach (MeshRenderer r in inChildren) {
            planes.Add(r.gameObject);
        }
        
        //this.transform.localPosition * 0.8f;
    }

    public virtual void OnEnable()
    {
        Orbital.instances.Add(this);
    }

    public virtual void OnDisable()
    {
        int del = -1;
        int i = 0;
        while (i < Orbital.instances.Count)
        {
            if (Orbital.instances[i] == this)
            {
                del = i;
                i = Orbital.instances.Count;
            }
            i++;
        }
        if (del != -1)
        {
            Orbital.instances.RemoveAt(del);
        }
    }

    public virtual void Update()
    {
        foreach (GameObject p in this.planes)
        {
            if (Vector3.Scale(new Vector3(1, 0, 1), p.transform.parent.forward).sqrMagnitude > 0.0001f)
            {
                p.transform.rotation = Quaternion.LookRotation(Vector3.Scale(new Vector3(1, 0, 1), p.transform.parent.forward), Camera.main.transform.forward);
            }
            if (this.index > 1)
            {
                p.transform.localScale = Vector3.Lerp(new Vector3(19, 19, 19), new Vector3(20, 20, 12), Mathf.Abs(p.transform.parent.up.y));
            }
            p.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.25f, 0.35f, 0.4f, ((1 - Status.zoomAmt) * 0.5f) * (float)(this.electrons) / 2f));
            p.GetComponent<Renderer>().enabled = (this.show && (Status.zoomAmt != 1)) && (this.electrons > 0);
        }
        this.GetComponent<Collider>().isTrigger = !((this.show && (Status.zoomAmt != 1)) && (this.electrons > 0));
    }

    public Orbital()
    {
        this.show = true;
        this.radius = 15f;
    }

    static Orbital()
    {
        Orbital.instances = new ArrayList();
    }

}