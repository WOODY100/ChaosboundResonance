using System;
using System.Collections.Generic;

using Chaosbound.Systems.Dialogue.Runtime.Actions.Definitions;
using Chaosbound.Systems.Dialogue.Runtime.Actions.Execution;

namespace Chaosbound.Systems.Dialogue.Runtime.Actions.Registry
{
    public sealed class DialogueActionRegistry
    {
        private readonly Dictionary<Type, IDialogueAction> actions =
            new Dictionary<Type, IDialogueAction>();

        public bool Register<TDefinition>(IDialogueAction action)
            where TDefinition : DialogueActionDefinition
        {
            if (action == null)
            {
                UnityEngine.Debug.LogError(
                    "[DialogueActionRegistry] Cannot register a null action.");

                return false;
            }

            Type definitionType = typeof(TDefinition);

            if (actions.ContainsKey(definitionType))
            {
                UnityEngine.Debug.LogError(
                    "[DialogueActionRegistry] An action is already registered for: "
                    + definitionType.Name);

                return false;
            }

            actions.Add(definitionType, action);

            return true;
        }

        public bool TryResolve(
            DialogueActionDefinition definition,
            out IDialogueAction action)
        {
            action = null;

            if (definition == null)
            {
                UnityEngine.Debug.LogError(
                    "[DialogueActionRegistry] Cannot resolve a null definition.");

                return false;
            }

            Type definitionType = definition.GetType();

            if (!actions.TryGetValue(definitionType, out action))
            {
                UnityEngine.Debug.LogError(
                    "[DialogueActionRegistry] No action registered for definition: "
                    + definitionType.Name);

                return false;
            }

            return true;
        }

        public bool IsRegistered<TDefinition>()
            where TDefinition : DialogueActionDefinition
        {
            return actions.ContainsKey(typeof(TDefinition));
        }

        public void Clear()
        {
            actions.Clear();
        }
    }
}