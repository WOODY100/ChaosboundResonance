using System.Collections.Generic;

public sealed class SkillTreeRuntimeState
{
    private readonly HashSet<string> purchasedNodeIds =
        new HashSet<string>();

    private readonly Dictionary<string, string> selectedChoices =
        new Dictionary<string, string>();

    public IReadOnlyCollection<string> PurchasedNodeIds
    {
        get { return purchasedNodeIds; }
    }

    public IReadOnlyDictionary<string, string> SelectedChoices
    {
        get { return selectedChoices; }
    }

    public bool IsNodePurchased(string nodeId)
    {
        if (string.IsNullOrWhiteSpace(nodeId))
            return false;

        return purchasedNodeIds.Contains(nodeId);
    }

    public bool TryGetSelectedChoice(
        string nodeId,
        out string optionId)
    {
        if (string.IsNullOrWhiteSpace(nodeId))
        {
            optionId = null;
            return false;
        }

        return selectedChoices.TryGetValue(
            nodeId,
            out optionId);
    }

    public bool AddPurchasedNode(string nodeId)
    {
        if (string.IsNullOrWhiteSpace(nodeId))
            return false;

        return purchasedNodeIds.Add(nodeId);
    }

    public bool SetSelectedChoice(
        string nodeId,
        string optionId)
    {
        if (string.IsNullOrWhiteSpace(nodeId))
            return false;

        if (string.IsNullOrWhiteSpace(optionId))
            return false;

        selectedChoices[nodeId] = optionId;
        return true;
    }

    public void Clear()
    {
        purchasedNodeIds.Clear();
        selectedChoices.Clear();
    }
}