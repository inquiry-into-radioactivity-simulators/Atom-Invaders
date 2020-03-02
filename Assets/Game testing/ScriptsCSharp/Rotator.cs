using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class Rotator : MonoBehaviour
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
            Vector3 v = Random.onUnitSphere;
            this.xF = v.x;
            this.yF = v.y;
            this.zF = v.z;
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

    public virtual void Update()
    {
        if (this.x)
        {
            this.transform.Rotate(((Vector3.right * this.xF) * this.speed) * Time.deltaTime);
        }
        if (this.y)
        {
            this.transform.Rotate(((Vector3.up * this.yF) * this.speed) * Time.deltaTime);
        }
        if (this.z)
        {
            this.transform.Rotate(((Vector3.forward * this.zF) * this.speed) * Time.deltaTime);
        }
    }

    public Rotator()
    {
        this.xF = 1f;
        this.yF = 1f;
        this.zF = 1f;
    }

}