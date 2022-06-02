using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "new Game Setting", menuName ="Game Setting/new Game setting")]
public class GameSetting : ScriptableObject
{
    public List<Color> Colors;
    public List<BallData> Rates = new List<BallData>();
}
[System.Serializable]
public class BallData
{
    public BALLTYPE Type;
    [Range(1, 100)]
    public int Rate;
    public Material Material;
}
