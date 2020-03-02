using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class NucleusFlare : MonoBehaviour
{
    public virtual void LateUpdate()
    {
        this.transform.position = this.transform.parent.position + (Vector3.up * 0.1f);
        this.transform.rotation = Quaternion.identity;
    }

}