﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Faecula", menuName = "Faecula/BulletHell/IndicatorPositionList", order = 1)]
public class IndicatorPositionList : ScriptableObject
{
    public List<IndicatorPositionValue> Indicators;
}

