using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Core.GameFlow
{
    public sealed class GameFlow
    {
        private readonly Stack<GameFlowContext> contextStack;

        private GameFlowConfiguration configuration;
        private GameFlowPolicyResolver policyResolver;
        private GameFlowSimulationController simulationController;

        private IGameplayInputTarget gameplayInputTarget;

        private bool levelUpPending;
        private bool isInitialized;

        public GameFlowContext CurrentContext
        {
            get
            {
                if (contextStack.Count == 0)
                    throw new InvalidOperationException(
                        "GameFlow has no active context.");

                return contextStack.Peek();
            }
        }

        public bool IsInitialized =>
            isInitialized;

        public bool IsLevelUpPending =>
            levelUpPending;

        public event Action<GameFlowContext, GameFlowContext>
            OnContextChanged;

        public bool HasGameplayInputTarget =>
            gameplayInputTarget != null;

        public GameFlow(
            GameFlowConfiguration configuration,
            GameFlowSimulationController simulationController)
        {
            this.configuration =
                configuration
                ?? throw new ArgumentNullException(
                    nameof(configuration));

            this.simulationController =
                simulationController
                ?? throw new ArgumentNullException(
                    nameof(simulationController));

            contextStack =
                new Stack<GameFlowContext>();

            policyResolver =
                new GameFlowPolicyResolver(
                    configuration);
        }

        //==========================================================
        // Initialization
        //==========================================================

        public void Initialize()
        {
            ResetInternalState();

            contextStack.Push(
                GameFlowContext.Playing);

            isInitialized = true;

            ApplyCurrentPolicy();
        }

        //==========================================================
        // Requests
        //==========================================================

        public bool Request(
            GameFlowContext requestedContext)
        {
            EnsureInitialized();

            GameFlowContext current =
                CurrentContext;

            GameFlowTransitionDecision decision =
                GameFlowTransitionRules.Evaluate(
                    current,
                    requestedContext);

            switch (decision)
            {
                case GameFlowTransitionDecision.Reject:

                    return false;

                case GameFlowTransitionDecision.Push:

                    PushContext(
                        requestedContext);

                    return true;

                case GameFlowTransitionDecision.Pending:

                    return RegisterPending(
                        requestedContext);

                case GameFlowTransitionDecision.Replace:

                    ReplaceContext(
                        requestedContext);

                    return true;

                default:

                    return false;
            }
        }

        //==========================================================
        // Push
        //==========================================================

        private void PushContext(
            GameFlowContext context)
        {
            GameFlowContext previous =
                CurrentContext;

            contextStack.Push(context);

            ApplyCurrentPolicy();

            NotifyContextChanged(
                previous,
                context);
        }

        //==========================================================
        // Replace
        //==========================================================

        private void ReplaceContext(
            GameFlowContext context)
        {
            GameFlowContext previous =
                CurrentContext;

            contextStack.Clear();

            contextStack.Push(context);

            levelUpPending = false;

            ApplyCurrentPolicy();

            NotifyContextChanged(
                previous,
                context);
        }

        //==========================================================
        // Pending
        //==========================================================

        private bool RegisterPending(
            GameFlowContext context)
        {
            if (context != GameFlowContext.LevelUp)
                return false;

            if (levelUpPending)
                return false;

            levelUpPending = true;

            return true;
        }

        //==========================================================
        // Pop
        //==========================================================

        public bool Pop(
            GameFlowContext expectedContext)
        {
            EnsureInitialized();

            if (contextStack.Count <= 1)
                return false;

            if (CurrentContext != expectedContext)
                return false;

            GameFlowContext previous =
                contextStack.Pop();

            GameFlowContext current =
                CurrentContext;

            ApplyCurrentPolicy();

            NotifyContextChanged(
                previous,
                current);

            ProcessPendingRequests();

            return true;
        }

        //==========================================================
        // Replace
        //==========================================================

        public void Replace(
            GameFlowContext context)
        {
            EnsureInitialized();

            GameFlowContext previous =
                CurrentContext;

            contextStack.Clear();

            contextStack.Push(context);

            levelUpPending = false;

            ApplyCurrentPolicy();

            NotifyContextChanged(
                previous,
                context);
        }

        //==========================================================
        // Pending Processing
        //==========================================================

        private void ProcessPendingRequests()
        {
            if (!levelUpPending)
                return;

            if (CurrentContext != GameFlowContext.Playing)
                return;

            levelUpPending = false;

            Request(
                GameFlowContext.LevelUp);
        }

        //==========================================================
        // Query
        //==========================================================

        public bool IsActive(
            GameFlowContext context)
        {
            EnsureInitialized();

            return contextStack.Contains(
                context);
        }

        //==========================================================
        // Reset
        //==========================================================

        public void ResetFlow()
        {
            if (!isInitialized)
            {
                ResetInternalState();
                return;
            }

            ResetInternalState();
        }

        private void ResetInternalState()
        {
            contextStack.Clear();

            levelUpPending = false;

            isInitialized = false;

            simulationController.Reset();
        }

        //==========================================================
        // Policy
        //==========================================================

        private void ApplyCurrentPolicy()
        {
            GameFlowPolicy policy =
                policyResolver.Resolve(
                    CurrentContext);

            simulationController.Apply(
                policy.Simulation);

            gameplayInputTarget?.SetGameplayInputEnabled(
                policy.GameplayInput);
        }

        //==========================================================
        // Events
        //==========================================================

        private void NotifyContextChanged(
            GameFlowContext previous,
            GameFlowContext current)
        {
            OnContextChanged?.Invoke(
                previous,
                current);
        }

        //==========================================================
        // Validation
        //==========================================================

        private void EnsureInitialized()
        {
            if (!isInitialized)
            {
                throw new InvalidOperationException(
                    "GameFlow has not been initialized.");
            }
        }

        //==========================================================
        // Gameplay Input Target
        //==========================================================

        public void BindGameplayInputTarget(
            IGameplayInputTarget target)
        {
            if (target == null)
                throw new ArgumentNullException(
                    nameof(target));

            gameplayInputTarget = target;

            ApplyCurrentGameplayInputPolicy();
        }

        public void UnbindGameplayInputTarget(
            IGameplayInputTarget target)
        {
            if (target == null)
                return;

            if (!ReferenceEquals(
                gameplayInputTarget,
                target))
            {
                return;
            }

            gameplayInputTarget = null;
        }

        private void ApplyCurrentGameplayInputPolicy()
        {
            if (!isInitialized ||
                contextStack.Count == 0)
            {
                gameplayInputTarget?.SetGameplayInputEnabled(
                    false);

                return;
            }

            GameFlowPolicy policy =
                policyResolver.Resolve(
                    CurrentContext);

            gameplayInputTarget?.SetGameplayInputEnabled(
                policy.GameplayInput);
        }

        //==========================================================
        // Properties
        //==========================================================

        public bool CanSimulate
        {
            get
            {
                if (!isInitialized ||
                    contextStack.Count == 0)
                {
                    return false;
                }

                return policyResolver.Resolve(
                    CurrentContext).Simulation;
            }
        }

        public bool CanRunGameplay
        {
            get
            {
                if (!isInitialized ||
                    contextStack.Count == 0)
                {
                    return false;
                }

                return policyResolver.Resolve(
                    CurrentContext).Gameplay;
            }
        }
    }
}