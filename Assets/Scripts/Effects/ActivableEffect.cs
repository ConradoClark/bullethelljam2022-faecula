using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Interfaces.Update;
using UnityEngine;


public abstract class ActivableEffect : MonoBehaviour, IActivable
{
    public bool IsActive { get; protected set; }
    public abstract bool Activate();
}

