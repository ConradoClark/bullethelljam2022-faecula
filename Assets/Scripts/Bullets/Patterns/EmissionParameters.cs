using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class EmissionParameters
{
    public IndicatorPositions IndicatorPosition;
    public Vector2 IndicatorPositionDirection;
    public Vector2 Offset;
    public Vector2 Direction;

    private EmissionParameters Clone()
    {
        return new EmissionParameters
        {
            IndicatorPosition = IndicatorPosition,
            IndicatorPositionDirection = IndicatorPositionDirection,
            Offset = Offset,
            Direction = Direction
        };
    }

    public EmissionParameters WithIndicatorPosition(IndicatorPositions position, Vector2 direction)
    {
        var clone = Clone();
        clone.IndicatorPosition = position;
        clone.IndicatorPositionDirection = direction;
        return clone;
    }

    public EmissionParameters WithOffset(Vector2 offset)
    {
        var clone = Clone();
        clone.Offset = offset;
        return clone;
    }

    public EmissionParameters WithDirection(Vector2 direction)
    {
        var clone = Clone();
        clone.Direction = direction;
        return clone;
    }
}
