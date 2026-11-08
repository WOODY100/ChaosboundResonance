using System;

namespace Chaosbound.Content.Currency
{
    public sealed class CurrencyResolver
    {
        private readonly CurrencyDatabase database;

        public CurrencyResolver(
            CurrencyDatabase database)
        {
            this.database =
                database ??
                throw new ArgumentNullException(
                    nameof(database));
        }

        public bool TryResolve(
            string contentId,
            out CurrencyDefinition definition)
        {
            return database.TryGet(
                contentId,
                out definition);
        }
    }
}