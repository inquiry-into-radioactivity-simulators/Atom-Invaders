using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class MenuGUI : MonoBehaviour
{
    public GUISkin skin;
    public int margin;
    public Transform obj;
    public virtual void Start() //Screen.SetResolution (450, 400, false);
    {
    }

    public virtual void OnGUI()
    {
        GUI.skin = this.skin;
        GUI.color = Status.eColor;
        GUILayout.Space(220);
        /*
	GUILayout.BeginHorizontal();
	GUILayout.Space(margin);
	GUILayout.Label("Nuclear Physics Simulator");
	GUILayout.Label("v0.8");
	GUILayout.Label("Forest Johnson");
	GUILayout.EndHorizontal();
	GUILayout.Space(60);
	*/
        GUILayout.BeginHorizontal(new GUILayoutOption[] {});
        GUILayout.Space(this.margin);
        if (GUILayout.Button("Atom Simulator", new GUILayoutOption[] {}))
        {
            Application.LoadLevel(1);
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(new GUILayoutOption[] {});
        GUILayout.Space(this.margin);
        GUILayout.Button("Radiation Simulator", new GUILayoutOption[] {});
        GUILayout.EndHorizontal();
        Vector3 point = Camera.main.WorldToScreenPoint(this.obj.position);
        GUIStyle style = new GUIStyle("label");
        string displayString = Mathf.Repeat(Time.timeSinceLevelLoad, 7) > 3.7f ? "v1.0" : "Radiation Sim";
        Vector2 size = style.CalcSize(new GUIContent(displayString));
        GUI.Label(new Rect(point.x, Screen.height - (point.y + 40), size.y * 0.7f, 30), displayString);
    }

    public MenuGUI()
    {
        this.margin = 4;
    }

}