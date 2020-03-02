using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class Mover : MonoBehaviour
{
    public bool x;
    public bool y;
    public bool z;
    public float speed;
    public bool random;
    private float xF;
    private float yF;
    private float zF;
    public virtual void Start()
    {
        if (this.random)
        {
            this.xF = Random.Range(-1f, 1f);
            this.yF = Random.Range(-1f, 1f);
            this.zF = Random.Range(-1f, 1f);
        }
    }

    public virtual void SetFactors(float xx, float yy, float zz)
    {
        this.xF = xx;
        this.yF = yy;
        this.zF = zz;
    }

    public virtual Vector3 GetFactors()
    {
        return new Vector3(this.xF, this.yF, this.zF);
    }

    public virtual void Accel(Vector3 accel)
    {
        Vector3 vel = new Vector3(this.xF * this.speed, this.yF * this.speed, this.zF * this.speed);
        vel = vel + accel;
        float sp = vel.magnitude;
        if (sp > 0)
        {
            vel = vel / sp;
        }
        this.xF = vel.x;
        this.yF = vel.y;
        this.zF = vel.z;
        this.speed = sp;
    }

    public virtual void Update()
    {
        if (this.x)
        {
            this.transform.Translate(((Vector3.right * this.xF) * this.speed) * Time.deltaTime, Space.World);
        }
        if (this.y && false)
        {
            this.transform.Translate(((Vector3.up * this.yF) * this.speed) * Time.deltaTime, Space.World);
        }
        if (this.z)
        {
            this.transform.Translate(((Vector3.forward * this.zF) * this.speed) * Time.deltaTime, Space.World);
        }
    }

    public Mover()
    {
        this.xF = 1f;
        this.yF = 1f;
        this.zF = 1f;
    }

}