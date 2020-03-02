using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class AtomSpawner : MonoBehaviour
{
    public int dist;
    public float time;
    public GameObject atom;
    public GameObject molecule;
    public float timer;
    private bool first;
    public virtual void Start()
    {
        this.timer = Status.atomMode ? this.time : this.time * 2;
    }

    public virtual void Update()
    {
        if (this.timer > (Status.atomMode ? this.time : this.time * 2))
        {
            this.timer = 0;
            Vector3 position = new Vector3((Random.value - 0.5f) * 2, 0, Random.value).normalized;
            Vector3 direction = (-position + (new Vector3((Random.value - 0.5f) * 2, 0, -Random.value) * 0.2f)).normalized;
            GameObject inst = (GameObject)UnityEngine.Object.Instantiate(Status.atomMode ? this.atom : this.molecule, (position * this.dist) * (this.first ? 0.5f : 1), Quaternion.identity);
            ((Mover) inst.GetComponent(typeof(Mover))).SetFactors(direction.x, 0, direction.z);
            this.first = false;
        }
        this.timer = this.timer + Time.deltaTime;
        foreach (Atom a in Atom.instances)
        {
            if (((a != null) && (Mathf.Abs(a.transform.position.x) > (this.dist * 1.2f))) || (a.transform.position.z < (-this.dist * 1.2f)))
            {
                UnityEngine.Object.Destroy(a.gameObject);
                return;
            }
        }
    }

    public AtomSpawner()
    {
        this.dist = 10000;
        this.time = 4f;
        this.timer = 10f;
        this.first = true;
    }

}