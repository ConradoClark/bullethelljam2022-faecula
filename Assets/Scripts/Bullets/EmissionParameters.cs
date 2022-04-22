using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Vector2 = UnityEngine.Vector2;

[Serializable]
public class EmissionParameters
{
    public IndicatorPositions IndicatorPosition;
    public Vector2 IndicatorPositionDirection;
    public Vector2 Offset;
    public Vector2 Direction;
    public float Intensity;
    public float Delay;
    public float Range;

    private EmissionParameters Clone()
    {
        return new EmissionParameters
        {
            IndicatorPosition = IndicatorPosition,
            IndicatorPositionDirection = IndicatorPositionDirection,
            Offset = Offset,
            Direction = Direction,
            Intensity = Intensity,
            Delay = Delay,
            Range = Range,
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

    public EmissionParameters WithIntensity(float intensity)
    {
        var clone = Clone();
        clone.Intensity = intensity;
        return clone;
    }

    public EmissionParameters WithDelay(float delay)
    {
        var clone = Clone();
        clone.Delay = delay;
        return clone;
    }

    public EmissionParameters WithRange(float range)
    {
        var clone = Clone();
        clone.Range = range;
        return clone;
    }
}
