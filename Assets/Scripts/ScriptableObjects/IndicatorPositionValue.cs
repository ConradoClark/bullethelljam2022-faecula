using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Faecula", menuName = "Faecula/BulletHell/IndicatorPositionValue", order = 1)]
public class IndicatorPositionValue : ScriptableObject
{
    public IndicatorPositions IndcatorPosition;
    public Vector3 Position;
}

