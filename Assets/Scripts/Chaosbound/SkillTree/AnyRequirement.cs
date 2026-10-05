using System;
using UnityEngine;

[Serializable]
public sealed class AnyRequirement : NodeRequirement
{
    [SerializeReference]
    public NodeRequirement[] Requirements;
}