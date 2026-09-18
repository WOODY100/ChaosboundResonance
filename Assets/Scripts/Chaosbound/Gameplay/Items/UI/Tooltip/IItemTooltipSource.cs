using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Items.UI.Tooltip
{
    public interface IItemTooltipSource :
        ITooltipSource,
        ITooltipSeenSource
    {
        ItemInstance CurrentItem { get; }
    }
}