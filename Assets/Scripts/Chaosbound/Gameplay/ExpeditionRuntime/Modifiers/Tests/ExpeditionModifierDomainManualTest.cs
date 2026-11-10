using System;
using System.Collections.Generic;
using System.Reflection;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using Chaosbound.Gameplay.ExpeditionRuntime.Cleanup.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Cleanup.Stages;
using Chaosbound.Gameplay.ExpeditionRuntime.Modifiers;
using UnityEngine;
using Chaosbound.Gameplay.ExpeditionRuntime.Modifiers.Integration;
using Player.Enums;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Modifiers.Tests
{
    /// <summary>
    /// Manual domain regression tests following the project's ContextMenu test pattern.
    /// Attach this component to an empty GameObject and run the Context Menu command.
    /// </summary>
    public sealed class ExpeditionModifierDomainManualTest : MonoBehaviour
    {
        [ContextMenu("Run Expedition Modifier Domain Tests")]
        private void RunTests()
        {
            int passed = 0;

            TestIndependentApplications();
            passed++;

            TestRefreshDuration();
            passed++;

            TestStackMagnitudeAndDuration();
            passed++;

            TestExpirationAndNoDoubleDiscount();
            passed++;

            TestStateNotifications();
            passed++;

            TestInvalidDefinitionCombination();
            passed++;

            TestMultiEffectDefinition();
            passed++;

            TestCleanupStageClearsApplications();
            passed++;

            TestPlayerTemporaryIntegration();
            passed++;

            TestEnemyModifierIntegration();
            passed++;

            Debug.Log(
                $"[Expedition Modifier Domain Tests] ALL PASSED: {passed} test groups.");
        }

        private static void TestIndependentApplications()
        {
            ExpeditionModifierDefinition definition = CreateDefinition(
                "test.independent",
                ExpeditionModifierLifetime.Timed,
                ExpeditionModifierStackingPolicy.Independent,
                1,
                30f,
                CreateEffect(ExpeditionModifierTarget.Player, "Damage", 0.20f));

            ExpeditionModifierState state = new ExpeditionModifierState();
            ExpeditionModifierDomainDirector director = new ExpeditionModifierDomainDirector();

            ExpeditionModifierApplyResult first =
                director.ApplyModifier(state, definition, "producer.a", TimeSpan.Zero);
            ExpeditionModifierApplyResult second =
                director.ApplyModifier(state, definition, "producer.b", TimeSpan.FromSeconds(5));

            Assert(first.Status == ExpeditionModifierApplyStatus.Added, "Independent first apply should be Added.");
            Assert(second.Status == ExpeditionModifierApplyStatus.Added, "Independent second apply should be Added.");
            Assert(state.Modifiers.Count == 2, "Independent should keep two applications.");
            Assert(first.Modifier.ApplicationId != second.Modifier.ApplicationId,
                "Independent applications must have different ApplicationIds.");

            Debug.Log("[Expedition Modifier Domain Tests] PASS: Independent applications.");
            DestroyDefinition(definition);
        }

        private static void TestRefreshDuration()
        {
            ExpeditionModifierDefinition definition = CreateDefinition(
                "test.refresh",
                ExpeditionModifierLifetime.Timed,
                ExpeditionModifierStackingPolicy.RefreshDuration,
                5,
                30f,
                CreateEffect(ExpeditionModifierTarget.Player, "Damage", 0.20f));

            ExpeditionModifierState state = new ExpeditionModifierState();
            ExpeditionModifierDomainDirector director = new ExpeditionModifierDomainDirector();

            ExpeditionModifierApplyResult first =
                director.ApplyModifier(state, definition, "producer.a", TimeSpan.Zero);

            director.RemoveExpiredModifiers(state, TimeSpan.FromSeconds(12));

            ExpeditionModifierApplyResult refreshed =
                director.ApplyModifier(state, definition, "producer.b", TimeSpan.FromSeconds(12));

            Assert(refreshed.Status == ExpeditionModifierApplyStatus.Updated, "Refresh should update existing application.");
            Assert(refreshed.Modifier.ApplicationId == first.Modifier.ApplicationId,
                "Refresh must preserve ApplicationId.");
            Assert(refreshed.Modifier.StackCount == 1, "Refresh must preserve stack count.");
            AssertApproximately(refreshed.Modifier.Effects[0].Value, 0.20f,
                "Refresh must preserve effect magnitude.");
            Assert(refreshed.Modifier.RemainingDuration == TimeSpan.FromSeconds(30),
                "Refresh must reset RemainingDuration to BaseDuration.");
            Assert(refreshed.Modifier.TotalGrantedDuration == TimeSpan.FromSeconds(30),
                "Refresh must reset TotalGrantedDuration to BaseDuration.");

            Debug.Log("[Expedition Modifier Domain Tests] PASS: RefreshDuration.");
            DestroyDefinition(definition);
        }

        private static void TestStackMagnitudeAndDuration()
        {
            ExpeditionModifierDefinition definition = CreateDefinition(
                "test.stack",
                ExpeditionModifierLifetime.Timed,
                ExpeditionModifierStackingPolicy.StackMagnitudeAndDuration,
                3,
                30f,
                CreateEffect(ExpeditionModifierTarget.Player, "Damage", 0.20f));

            ExpeditionModifierState state = new ExpeditionModifierState();
            ExpeditionModifierDomainDirector director = new ExpeditionModifierDomainDirector();

            ExpeditionModifierApplyResult first =
                director.ApplyModifier(state, definition, "producer.a", TimeSpan.Zero);

            ExpeditionModifierApplyResult second =
                director.ApplyModifier(state, definition, "producer.a", TimeSpan.FromSeconds(10));

            Assert(second.Status == ExpeditionModifierApplyStatus.Updated, "Second stack should update application.");
            Assert(second.Modifier.StackCount == 2, "Second acquisition should produce two stacks.");
            AssertApproximately(second.Modifier.Effects[0].Value, 0.40f,
                "Two stacks should double the base magnitude.");
            Assert(second.Modifier.RemainingDuration == TimeSpan.FromSeconds(50),
                "Ten elapsed seconds from 30, plus 30 granted, should leave 50 seconds.");
            Assert(second.Modifier.TotalGrantedDuration == TimeSpan.FromSeconds(60),
                "TotalGrantedDuration should accumulate each grant.");

            ExpeditionModifierApplyResult third =
                director.ApplyModifier(state, definition, "producer.a", TimeSpan.FromSeconds(10));
            ExpeditionModifierApplyResult capped =
                director.ApplyModifier(state, definition, "producer.a", TimeSpan.FromSeconds(10));

            Assert(third.Modifier.StackCount == 3, "Third acquisition should reach MaxStacks.");
            AssertApproximately(third.Modifier.Effects[0].Value, 0.60f,
                "Three stacks should triple base magnitude.");
            Assert(capped.Modifier.StackCount == 3, "Stack count must not exceed MaxStacks.");
            AssertApproximately(capped.Modifier.Effects[0].Value, 0.60f,
                "Magnitude must not increase beyond MaxStacks.");
            Assert(capped.Modifier.RemainingDuration == TimeSpan.FromSeconds(110),
                "Duration must continue accumulating at MaxStacks.");
            Assert(capped.Modifier.TotalGrantedDuration == TimeSpan.FromSeconds(120),
                "TotalGrantedDuration must continue accumulating at MaxStacks.");

            Debug.Log("[Expedition Modifier Domain Tests] PASS: StackMagnitudeAndDuration and MaxStacks.");
            DestroyDefinition(definition);
        }

        private static void TestExpirationAndNoDoubleDiscount()
        {
            ExpeditionModifierDefinition definition = CreateDefinition(
                "test.clock",
                ExpeditionModifierLifetime.Timed,
                ExpeditionModifierStackingPolicy.StackMagnitudeAndDuration,
                4,
                30f,
                CreateEffect(ExpeditionModifierTarget.Player, "Damage", 0.10f));

            ExpeditionModifierState state = new ExpeditionModifierState();
            ExpeditionModifierDomainDirector director = new ExpeditionModifierDomainDirector();

            ExpeditionModifierApplyResult first =
                director.ApplyModifier(state, definition, "producer", TimeSpan.Zero);

            // Stage advances the clock to 10 seconds: 20 remain.
            director.RemoveExpiredModifiers(state, TimeSpan.FromSeconds(10));

            // Applying at the exact same ElapsedTime must not discount those 10 seconds again.
            ExpeditionModifierApplyResult second =
                director.ApplyModifier(state, definition, "producer", TimeSpan.FromSeconds(10));

            Assert(second.Modifier.RemainingDuration == TimeSpan.FromSeconds(50),
                "Same-time reapplication must not discount the same elapsed interval twice.");

            // Advance by another 20 seconds; remaining becomes 30.
            director.RemoveExpiredModifiers(state, TimeSpan.FromSeconds(30));
            Assert(state.Modifiers.Count == 1, "Application should remain active before expiration.");
            Assert(state.Modifiers[0].RemainingDuration == TimeSpan.FromSeconds(30),
                "Clock should subtract only the newly elapsed 20 seconds.");

            director.RemoveExpiredModifiers(state, TimeSpan.FromSeconds(60));
            Assert(state.Modifiers.Count == 0, "Application should be removed at zero remaining duration.");

            Debug.Log("[Expedition Modifier Domain Tests] PASS: Expiration and no double discount.");
            DestroyDefinition(definition);
        }

        private static void TestStateNotifications()
        {
            ExpeditionModifierDefinition definition = CreateDefinition(
                "test.events",
                ExpeditionModifierLifetime.Timed,
                ExpeditionModifierStackingPolicy.RefreshDuration,
                1,
                15f,
                CreateEffect(ExpeditionModifierTarget.Player, "Damage", 0.15f));

            ExpeditionModifierState state = new ExpeditionModifierState();
            ExpeditionModifierDomainDirector director = new ExpeditionModifierDomainDirector();

            int added = 0;
            int updated = 0;
            int removed = 0;
            int cleared = 0;

            state.ModifierAdded += modifier =>
            {
                Assert(state.FindByApplicationId(modifier.ApplicationId) != null,
                    "ModifierAdded must fire after state insertion.");
                added++;
            };
            state.ModifierUpdated += (previous, current) =>
            {
                Assert(state.FindByApplicationId(current.ApplicationId) == current,
                    "ModifierUpdated must fire after state replacement.");
                updated++;
            };
            state.ModifierRemoved += modifier =>
            {
                Assert(state.FindByApplicationId(modifier.ApplicationId) == null,
                    "ModifierRemoved must fire after state removal.");
                removed++;
            };
            state.ModifiersCleared += modifiers =>
            {
                Assert(state.Modifiers.Count == 0,
                    "ModifiersCleared must fire after state is cleared.");
                cleared++;
            };

            ExpeditionModifierApplyResult first =
                director.ApplyModifier(state, definition, "producer", TimeSpan.Zero);
            director.ApplyModifier(state, definition, "producer", TimeSpan.FromSeconds(1));
            director.RemoveModifier(state, first.Modifier.ApplicationId);
            director.ApplyModifier(state, definition, "producer", TimeSpan.FromSeconds(2));
            director.Clear(state);

            Assert(added == 2, "Expected two ModifierAdded notifications.");
            Assert(updated == 1, "Expected one ModifierUpdated notification.");
            Assert(removed == 1, "Expected one ModifierRemoved notification.");
            Assert(cleared == 1, "Expected one ModifiersCleared notification.");

            Debug.Log("[Expedition Modifier Domain Tests] PASS: State notifications.");
            DestroyDefinition(definition);
        }

        private static void TestInvalidDefinitionCombination()
        {
            ExpeditionModifierDefinition definition = CreateDefinition(
                "test.invalid",
                ExpeditionModifierLifetime.Expedition,
                ExpeditionModifierStackingPolicy.RefreshDuration,
                2,
                0f,
                CreateEffect(ExpeditionModifierTarget.Player, "Damage", 0.20f));

            ExpeditionModifierState state = new ExpeditionModifierState();
            ExpeditionModifierApplyResult result =
                new ExpeditionModifierDomainDirector().ApplyModifier(
                    state, definition, "producer", TimeSpan.Zero);

            Assert(result.Status == ExpeditionModifierApplyStatus.Rejected,
                "Expedition + RefreshDuration must be rejected.");
            Assert(state.Modifiers.Count == 0,
                "Rejected definition must not mutate state.");

            Debug.Log("[Expedition Modifier Domain Tests] PASS: Invalid policy/lifetime combination.");
            DestroyDefinition(definition);
        }

        private static void TestMultiEffectDefinition()
        {
            ExpeditionModifierDefinition definition = CreateDefinition(
                "test.multi",
                ExpeditionModifierLifetime.Timed,
                ExpeditionModifierStackingPolicy.StackMagnitudeAndDuration,
                3,
                20f,
                CreateEffect(ExpeditionModifierTarget.Player, "Damage", 0.10f),
                CreateEffect(ExpeditionModifierTarget.Enemy, "MoveSpeed", -0.05f));

            ExpeditionModifierState state = new ExpeditionModifierState();
            ExpeditionModifierDomainDirector director = new ExpeditionModifierDomainDirector();

            director.ApplyModifier(state, definition, "producer", TimeSpan.Zero);
            ExpeditionModifierApplyResult stacked =
                director.ApplyModifier(state, definition, "producer", TimeSpan.FromSeconds(1));

            Assert(stacked.Modifier.Effects.Count == 2,
                "All effects from a definition must be present in the application.");
            AssertApproximately(stacked.Modifier.Effects[0].Value, 0.20f,
                "Player effect should scale with stack count.");
            AssertApproximately(stacked.Modifier.Effects[1].Value, -0.10f,
                "Enemy effect should scale with stack count and preserve sign.");

            Debug.Log("[Expedition Modifier Domain Tests] PASS: Multi-effect definition.");
            DestroyDefinition(definition);
        }



        private static void TestCleanupStageClearsApplications()
        {
            ExpeditionRuntimeState runtimeState = new ExpeditionRuntimeState();
            ExpeditionModifierDomainDirector director =
                new ExpeditionModifierDomainDirector();

            ExpeditionModifier modifier = new ExpeditionModifier(
                new[]
                {
                    new ExpeditionModifierEffect(
                        ExpeditionModifierTarget.Player,
                        "Damage",
                        ModifierType.Percent,
                        0.20f)
                },
                ExpeditionModifierLifetime.Expedition,
                TimeSpan.Zero,
                TimeSpan.Zero);

            director.AddModifier(runtimeState.Modifiers, modifier);

            int clearedNotifications = 0;
            runtimeState.Modifiers.ModifiersCleared += removed =>
            {
                Assert(runtimeState.Modifiers.Modifiers.Count == 0,
                    "Cleanup notification must occur after applications are cleared.");
                Assert(removed.Count == 1,
                    "Cleanup notification should include the removed applications.");
                clearedNotifications++;
            };

            ExpeditionModifierCleanupStage cleanupStage =
                new ExpeditionModifierCleanupStage(director);

            cleanupStage.Execute(new ExpeditionCleanupContext(runtimeState));

            Assert(runtimeState.Modifiers.Modifiers.Count == 0,
                "Expedition cleanup must clear all modifier applications.");
            Assert(clearedNotifications == 1,
                "Expedition cleanup must emit one ModifiersCleared notification.");

            Debug.Log("[Expedition Modifier Domain Tests] PASS: Expedition cleanup clears applications.");
        }


        private static void TestPlayerTemporaryIntegration()
        {
            GameObject playerObject = new GameObject("ModifierIntegrationTestPlayer");
            PlayerModifierSystem playerModifiers =
                playerObject.AddComponent<PlayerModifierSystem>();

            // A Run-layer source must remain after temporary sources are removed.
            playerModifiers.AddSource(
                ModifierLayer.Run,
                new ModifierSource("test.run.damage")
                {
                    Modifiers = new List<StatModifier>
                    {
                        new StatModifier
                        {
                            StatType = StatType.Damage,
                            ModifierType = ModifierType.Percent,
                            Value = 10f
                        }
                    }
                });

            ExpeditionModifierState state = new ExpeditionModifierState();
            ExpeditionPlayerModifierIntegration integration =
                new ExpeditionPlayerModifierIntegration(state, playerModifiers);

            ExpeditionModifier first = new ExpeditionModifier(
                "player-app-1",
                "test.player.damage",
                "test.producer",
                new[]
                {
                    new ExpeditionModifierEffect(
                        ExpeditionModifierTarget.Player,
                        "Damage",
                        ModifierType.Percent,
                        0.20f)
                },
                ExpeditionModifierLifetime.Timed,
                ExpeditionModifierStackingPolicy.Independent,
                1,
                1,
                TimeSpan.FromSeconds(30),
                TimeSpan.FromSeconds(30),
                TimeSpan.FromSeconds(30),
                TimeSpan.Zero,
                TimeSpan.Zero);

            ExpeditionModifier second = new ExpeditionModifier(
                "player-app-2",
                "test.player.damage",
                "test.producer",
                new[]
                {
                    new ExpeditionModifierEffect(
                        ExpeditionModifierTarget.Player,
                        "Damage",
                        ModifierType.Percent,
                        0.10f)
                },
                ExpeditionModifierLifetime.Timed,
                ExpeditionModifierStackingPolicy.Independent,
                1,
                1,
                TimeSpan.FromSeconds(30),
                TimeSpan.FromSeconds(30),
                TimeSpan.FromSeconds(30),
                TimeSpan.Zero,
                TimeSpan.Zero);

            ExpeditionModifierDomainDirector director =
                new ExpeditionModifierDomainDirector();

            director.AddModifier(state, first);
            director.AddModifier(state, second);

            // Base 20 + Run 10% + Temporary 20% + 10% = 28.
            AssertApproximately(
                playerModifiers.GetStat(StatType.Damage),
                28f,
                "Player integration should convert fractional percentages and keep Run sources.");

            ExpeditionModifier updatedSecond = new ExpeditionModifier(
                second.ApplicationId,
                second.DefinitionId,
                second.SourceId,
                new[]
                {
                    new ExpeditionModifierEffect(
                        ExpeditionModifierTarget.Player,
                        "Damage",
                        ModifierType.Percent,
                        0.30f)
                },
                second.Lifetime,
                second.StackingPolicy,
                second.StackCount,
                second.MaxStacks,
                second.BaseDuration,
                second.RemainingDuration,
                second.TotalGrantedDuration,
                second.CreatedAt,
                second.LastUpdatedAt);

            state.Replace(second, updatedSecond);

            AssertApproximately(
                playerModifiers.GetStat(StatType.Damage),
                32f,
                "ModifierUpdated should replace the matching temporary source.");

            director.RemoveModifier(state, first.ApplicationId);

            AssertApproximately(
                playerModifiers.GetStat(StatType.Damage),
                28f,
                "Removing one application must preserve the other temporary and Run sources.");

            director.Clear(state);

            AssertApproximately(
                playerModifiers.GetStat(StatType.Damage),
                22f,
                "ModifiersCleared should remove only this integration's temporary sources.");

            integration.Dispose();
            UnityEngine.Object.DestroyImmediate(playerObject);

            Debug.Log("[Expedition Modifier Domain Tests] PASS: Player Temporary integration.");
        }


        private static void TestEnemyModifierIntegration()
        {
            ExpeditionRuntimeState expeditionState = new ExpeditionRuntimeState();

            EnemyVariantData variant =
                ScriptableObject.CreateInstance<EnemyVariantData>();
            SetPrivateField(variant, "m_BaseHealth", 100f);
            SetPrivateField(variant, "m_BaseDamage", 10f);
            SetPrivateField(variant, "m_MoveSpeed", 3.5f);

            GameObject enemyObject = new GameObject("EnemyModifierIntegrationTest");
            EnemyRuntimeContext context = enemyObject.AddComponent<EnemyRuntimeContext>();
            EnemyRuntimeStats stats = enemyObject.AddComponent<EnemyRuntimeStats>();
            EnemyHealth health = enemyObject.AddComponent<EnemyHealth>();
            EnemyRuntimeNavigation navigation = enemyObject.AddComponent<EnemyRuntimeNavigation>();

            context.Initialize(variant, expeditionState);
            stats.Initialize();
            health.Initialize();
            SetPrivateField(health, "<CurrentHealth>k__BackingField", 50f);

            ExpeditionEnemyModifierIntegration integration =
                new ExpeditionEnemyModifierIntegration(
                    expeditionState.Modifiers, stats, health, navigation);

            ExpeditionModifierDomainDirector director =
                new ExpeditionModifierDomainDirector();

            ExpeditionModifier modifier = new ExpeditionModifier(
                "enemy-app-1",
                "test.enemy.stats",
                "test.producer",
                new[]
                {
                    new ExpeditionModifierEffect(ExpeditionModifierTarget.Enemy, "MaxHealth", ModifierType.Percent, 0.20f),
                    new ExpeditionModifierEffect(ExpeditionModifierTarget.Enemy, "Damage", ModifierType.Percent, 0.50f),
                    new ExpeditionModifierEffect(ExpeditionModifierTarget.Enemy, "MoveSpeed", ModifierType.Percent, 0.20f)
                },
                ExpeditionModifierLifetime.Expedition,
                ExpeditionModifierStackingPolicy.Independent,
                1, 1,
                TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero,
                TimeSpan.Zero, TimeSpan.Zero);

            director.AddModifier(expeditionState.Modifiers, modifier);

            AssertApproximately(stats.MaxHealth, 120f, "Enemy MaxHealth should refresh from expedition modifiers.");
            AssertApproximately(health.MaxHealth, 120f, "EnemyHealth.MaxHealth should follow refreshed runtime stats.");
            AssertApproximately(health.CurrentHealth, 60f, "Current health should preserve its 50% ratio after MaxHealth changes.");
            AssertApproximately(stats.Damage, 15f, "Enemy damage should refresh from expedition modifiers.");
            AssertApproximately(stats.MoveSpeed, 4.2f, "Enemy movement speed stat should refresh from expedition modifiers.");

            director.RemoveModifier(expeditionState.Modifiers, modifier.ApplicationId);

            AssertApproximately(stats.MaxHealth, 100f, "Removing the modifier should restore base MaxHealth.");
            AssertApproximately(health.MaxHealth, 100f, "EnemyHealth.MaxHealth should restore with the runtime stat.");
            AssertApproximately(health.CurrentHealth, 50f, "Current health ratio should remain 50% after removing MaxHealth modifier.");
            AssertApproximately(stats.Damage, 10f, "Removing the modifier should restore base Damage.");
            AssertApproximately(stats.MoveSpeed, 3.5f, "Removing the modifier should restore base MoveSpeed.");

            integration.Dispose();
            UnityEngine.Object.DestroyImmediate(enemyObject);
            UnityEngine.Object.DestroyImmediate(variant);

            Debug.Log("[Expedition Modifier Domain Tests] PASS: Enemy modifier integration and health ratio.");
        }

        private static ExpeditionModifierDefinition CreateDefinition(
            string contentId,
            ExpeditionModifierLifetime lifetime,
            ExpeditionModifierStackingPolicy policy,
            int maxStacks,
            float durationSeconds,
            params ExpeditionModifierEffectConfiguration[] configuredEffects)
        {
            ExpeditionModifierDefinition definition =
                ScriptableObject.CreateInstance<ExpeditionModifierDefinition>();

            SetPrivateField(definition, "contentId", contentId);
            SetPrivateField(definition, "displayName", contentId);
            SetPrivateField(definition, "stackingPolicy", policy);
            SetPrivateField(definition, "maxStacks", maxStacks);
            SetPrivateField(definition, "durationSeconds", durationSeconds);
            SetPrivateField(definition, "lifetime", lifetime);
            SetPrivateField(
                definition,
                "effects",
                new List<ExpeditionModifierEffectConfiguration>(configuredEffects));

            return definition;
        }

        private static ExpeditionModifierEffectConfiguration CreateEffect(
            ExpeditionModifierTarget target,
            string statId,
            float value)
        {
            ExpeditionModifierEffectConfiguration effect =
                new ExpeditionModifierEffectConfiguration();

            SetPrivateField(effect, "target", target);
            SetPrivateField(effect, "statId", statId);
            SetPrivateField(effect, "operation", ModifierType.Percent);
            SetPrivateField(effect, "value", value);

            return effect;
        }

        private static void SetPrivateField(object target, string fieldName, object value)
        {
            FieldInfo field = target.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            if (field == null)
                throw new MissingFieldException(target.GetType().Name, fieldName);

            field.SetValue(target, value);
        }

        private static void Assert(bool condition, string message)
        {
            if (!condition)
                throw new InvalidOperationException(
                    "[Expedition Modifier Domain Tests] FAIL: " + message);
        }

        private static void AssertApproximately(float actual, float expected, string message)
        {
            if (Math.Abs(actual - expected) > 0.0001f)
                throw new InvalidOperationException(
                    "[Expedition Modifier Domain Tests] FAIL: " + message +
                    $" Expected={expected}, Actual={actual}");
        }

        private static void DestroyDefinition(ExpeditionModifierDefinition definition)
        {
            if (definition != null)
                DestroyImmediate(definition);
        }
    }
}
