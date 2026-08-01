using System;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Scene
{
    [Serializable]
    public sealed class SceneAuthoring
    {
        [SerializeField]
        private string m_SceneName = string.Empty;

        public string SceneName => m_SceneName;
    }
}