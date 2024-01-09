using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Terrain Piece List", menuName = "Terrain Pieces")]
public class TerrainList : ScriptableObject
{

    public List<GameObject> pieces;
}
