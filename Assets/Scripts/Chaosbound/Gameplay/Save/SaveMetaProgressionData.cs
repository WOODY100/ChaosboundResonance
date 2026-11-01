using System;

namespace Chaosbound.Gameplay.Save
{
    [Serializable]
    public sealed class SaveMetaProgressionData
    {
        public int Experience;

        public int MetaLevel = 1;

        public int SkillPoints;
    }
}
