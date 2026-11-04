#if UNITY_EDITOR
namespace Chaosbound.Development.Analytics.Editor
{
    // Session lifecycle is now centralized in ExpeditionAnalyticsEditorService.
    // This type is intentionally retained so existing Unity asset references
    // and its .meta GUID remain stable during the V1 -> V2.1 update.
    internal static class ExpeditionAnalyticsEditorLifecycle
    {
    }
}
#endif
