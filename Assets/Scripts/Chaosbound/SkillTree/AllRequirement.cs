using System;
using UnityEngine;

[Serializable]
public sealed class AllRequirement : NodeRequirement
{
    [SerializeReference]
    public NodeRequirement[] Requirements;
}