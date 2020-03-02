using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class Rules : MonoBehaviour
{
    public static float eBounceLevel;
    public static float eBounceLevelRand;
    static Rules()
    {
        Rules.eBounceLevel = 90f;
        Rules.eBounceLevelRand = 10f;
    }

}