using System;
using Licht.Interfaces.Generation;
using Random = UnityEngine.Random;

public class UnityRandomGenerator : IGenerator<int, float>
{
    public int Seed { get; set; }
    public float Generate()
    {
        return Random.value;
    }
}
