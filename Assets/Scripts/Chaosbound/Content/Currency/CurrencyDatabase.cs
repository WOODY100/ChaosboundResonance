using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Content.Currency
{
    [CreateAssetMenu(
        fileName = "CurrencyDatabase",
        menuName = "Chaosbound/Currency/Currency Database")]
    public sealed class CurrencyDatabase : ScriptableObject
    {
        [SerializeField]
        private List<CurrencyDefinition> currencies =
            new List<CurrencyDefinition>();

        private Dictionary<string, CurrencyDefinition> currenciesById;
        private HashSet<string> ambiguousIds;

        public IReadOnlyList<CurrencyDefinition> Currencies =>
            currencies;

        private void OnEnable()
        {
            RebuildIndex();
        }

        public bool TryGet(
            string contentId,
            out CurrencyDefinition definition)
        {
            definition = null;

            if (string.IsNullOrWhiteSpace(contentId))
                return false;

            EnsureIndex();

            contentId = contentId.Trim();

            if (ambiguousIds.Contains(contentId))
                return false;

            return currenciesById.TryGetValue(
                contentId,
                out definition);
        }

        private void EnsureIndex()
        {
            if (currenciesById == null ||
                ambiguousIds == null)
            {
                RebuildIndex();
            }
        }

        private void RebuildIndex()
        {
            currenciesById =
                new Dictionary<string, CurrencyDefinition>(
                    StringComparer.Ordinal);

            ambiguousIds =
                new HashSet<string>(
                    StringComparer.Ordinal);

            if (currencies == null)
                return;

            for (int i = 0; i < currencies.Count; i++)
            {
                CurrencyDefinition candidate =
                    currencies[i];

                if (candidate == null)
                    continue;

                string contentId =
                    candidate.ContentId;

                if (string.IsNullOrWhiteSpace(contentId))
                    continue;

                contentId = contentId.Trim();

                if (ambiguousIds.Contains(contentId))
                    continue;

                if (currenciesById.ContainsKey(contentId))
                {
                    currenciesById.Remove(contentId);
                    ambiguousIds.Add(contentId);
                    continue;
                }

                currenciesById.Add(
                    contentId,
                    candidate);
            }
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            ValidateCurrencies();
            RebuildIndex();
        }

        private void ValidateCurrencies()
        {
            if (currencies == null)
                return;

            HashSet<string> registeredIds =
                new HashSet<string>(
                    StringComparer.Ordinal);

            for (int i = 0; i < currencies.Count; i++)
            {
                CurrencyDefinition candidate =
                    currencies[i];

                if (candidate == null)
                {
                    Debug.LogWarning(
                        $"{nameof(CurrencyDatabase)}: " +
                        $"Null currency reference at index {i}.",
                        this);

                    continue;
                }

                string contentId =
                    candidate.ContentId;

                if (string.IsNullOrWhiteSpace(contentId))
                {
                    Debug.LogError(
                        $"{nameof(CurrencyDatabase)}: " +
                        $"Currency '{candidate.name}' " +
                        "has an empty ContentId.",
                        candidate);

                    continue;
                }

                contentId = contentId.Trim();

                if (!registeredIds.Add(contentId))
                {
                    Debug.LogError(
                        $"{nameof(CurrencyDatabase)}: " +
                        $"Duplicate ContentId '{contentId}'.",
                        this);
                }
            }
        }

#endif
    }
}