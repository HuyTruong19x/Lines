using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "new Game Setting", menuName ="Game Setting/new Game setting")]
public class GameSetting : ScriptableObject
{
    public List<Color> Colors;
    [Range(1, 100)]
    public int RateSpawnGhostBall;
}
