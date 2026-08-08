using Chaosbound.Content.Expeditions.Authoring.Combat;
using Chaosbound.Content.Expeditions.Definitions.Combat;
using System;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Builders.Combat
{
    public static class CombatBuilder
    {
        public static CombatDefinition Build(
            CombatAuthoring authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(nameof(authoring));

            List<CombatTacticDefinition> tactics =
                BuildTactics(authoring.Tactics);

            return new CombatDefinition(tactics);
        }

        private static List<CombatTacticDefinition> BuildTactics(
            IReadOnlyList<CombatTacticAuthoring> authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(nameof(authoring));

            List<CombatTacticDefinition> result =
                new(authoring.Count);

            foreach (CombatTacticAuthoring tactic in authoring)
            {
                if (tactic == null)
                {
                    throw new InvalidOperationException(
                        "CombatAuthoring contains a null CombatTacticAuthoring.");
                }

                result.Add(
                    new CombatTacticDefinition());
            }

            return result;
        }
    }
}