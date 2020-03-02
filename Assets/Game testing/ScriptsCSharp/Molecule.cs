using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class Molecule : MonoBehaviour
{
    public ArrayList atoms;
    public int size;
    public bool doing;
    public Mover myMov;
    public Rotator myRot;
    public virtual void Start()
    {
        foreach (Transform c in this.transform)
        {
            ((Atom) c.GetComponent(typeof(Atom))).connected = this;
            this.atoms.Add(c);
        }
    }

    public virtual void Update()
    {
        if (!this.doing)
        {
            return;
        }
        bool other = false;
        foreach (Transform c in this.atoms)
        {
            if (c && ((Atom) c.GetComponent(typeof(Atom))).dirty)
            {
                Vector3 mMF = this.myMov.GetFactors();
                Vector3 mRF = this.myRot.GetFactors();
                Vector3 rotM = Quaternion.AngleAxis(90, Vector3.up) * mMF;
                foreach (Transform a in this.atoms)
                {
                    Mover mov = ((Atom) a.GetComponent(typeof(Atom))).mov;
                    Rotator rot = ((Atom) a.GetComponent(typeof(Atom))).rot;
                    mov.enabled = true;
                    rot.enabled = true;
                    rot.SetFactors(mRF.x, mRF.y, mRF.z);
                    if (c == a)
                    {
                        mov.SetFactors(rotM.x, rotM.y, rotM.z);
                    }
                    else
                    {
                        mov.SetFactors(mMF.x, mMF.y, mMF.z);
                    }
                }
                UnityEngine.Object.Destroy(this.gameObject);
                return;
            }
            if (!c)
            {
                UnityEngine.Object.Destroy(this.gameObject);
                return;
            }
            c.transform.parent = null;
            c.transform.position = this.transform.TransformPoint(new Vector3(other ? this.size : -this.size, 0, 0));

            {
                Vector3 temp = c.transform.position;
                temp.y = 0;
                c.transform.position = temp;
            }
            c.transform.rotation = this.transform.rotation;
            other = true;
        }
    }

    public Molecule()
    {
        this.atoms = new ArrayList();
        this.size = 69;
        this.doing = true;
    }

}