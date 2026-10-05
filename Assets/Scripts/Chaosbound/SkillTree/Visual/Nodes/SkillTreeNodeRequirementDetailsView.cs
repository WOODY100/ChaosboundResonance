using System.Text;
using TMPro;
using UnityEngine;

public sealed class SkillTreeNodeRequirementDetailsView : MonoBehaviour
{
    [Header("Content")]

    [SerializeField]
    private TMP_Text textRequirements;

    [Header("Database")]

    [SerializeField]
    private SkillTreeDatabase database;

    private void Awake()
    {
        Hide();
    }

    public void Show(
        NodeRequirement requirement)
    {
        if (requirement == null)
        {
            Hide();
            return;
        }

        StringBuilder builder =
            new StringBuilder();

        BuildRequirementText(
            requirement,
            builder,
            0);

        if (textRequirements != null)
        {
            textRequirements.text =
                builder.ToString();
        }

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (textRequirements != null)
        {
            textRequirements.text =
                string.Empty;
        }

        gameObject.SetActive(false);
    }

    private void BuildRequirementText(
        NodeRequirement requirement,
        StringBuilder builder,
        int indent)
    {
        if (requirement == null)
            return;

        NodePurchasedRequirement purchased =
            requirement as NodePurchasedRequirement;

        if (purchased != null)
        {
            AppendIndent(
                builder,
                indent);

            builder.Append("• ");
            builder.Append(
                GetNodeDisplayName(
                    purchased.NodeId));

            builder.AppendLine();

            return;
        }

        AllRequirement all =
            requirement as AllRequirement;

        if (all != null)
        {
            AppendIndent(
                builder,
                indent);

            builder.AppendLine(
                "Require all:");

            BuildChildren(
                all.Requirements,
                builder,
                indent + 1);

            return;
        }

        AnyRequirement any =
            requirement as AnyRequirement;

        if (any != null)
        {
            AppendIndent(
                builder,
                indent);

            builder.AppendLine(
                "Require one:");

            BuildChildren(
                any.Requirements,
                builder,
                indent + 1);

            return;
        }

        AppendIndent(
            builder,
            indent);

        builder.AppendLine(
            requirement.GetType().Name);
    }

    private void BuildChildren(
        NodeRequirement[] requirements,
        StringBuilder builder,
        int indent)
    {
        if (requirements == null ||
            requirements.Length == 0)
        {
            AppendIndent(
                builder,
                indent);

            builder.AppendLine(
                "No requirements.");

            return;
        }

        for (int i = 0;
             i < requirements.Length;
             i++)
        {
            BuildRequirementText(
                requirements[i],
                builder,
                indent);
        }
    }

    private void AppendIndent(
        StringBuilder builder,
        int indent)
    {
        for (int i = 0; i < indent; i++)
        {
            builder.Append("    ");
        }
    }

    private string GetNodeDisplayName(
        string nodeId)
    {
        if (string.IsNullOrWhiteSpace(nodeId))
            return "Unknown Node";

        if (database == null ||
            database.Nodes == null)
        {
            return nodeId;
        }

        for (int i = 0;
             i < database.Nodes.Count;
             i++)
        {
            SkillTreeNodeDefinition node =
                database.Nodes[i];

            if (node == null)
                continue;

            if (node.NodeId != nodeId)
                continue;

            if (!string.IsNullOrWhiteSpace(
                node.DisplayName))
            {
                return node.DisplayName;
            }

            return nodeId;
        }

        return nodeId;
    }
}