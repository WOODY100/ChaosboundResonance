using System;

namespace Chaosbound.Gameplay.Items.Runtime
{
    public static class ItemInstanceIdGenerator
    {
        public static string Generate()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}