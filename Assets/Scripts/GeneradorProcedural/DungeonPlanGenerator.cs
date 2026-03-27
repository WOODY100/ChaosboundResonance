using System.Collections.Generic;
using UnityEngine;

public static class DungeonPlanGenerator
{
    public static List<RoomType> GeneratePlan()
    {
        List<RoomType> plan = new List<RoomType>();

        // 🔹 Cantidades fijas
        AddRooms(plan, RoomType.Combat, 3);
        AddRooms(plan, RoomType.Neutral, 4);
        AddRooms(plan, RoomType.MiniBoss, 1);

        // 🔹 Shuffle controlado
        Shuffle(plan);

        // 🔹 Reglas estructurales
        EnforceRules(plan);

        // 🔹 Insertar Start y Boss
        plan.Insert(0, RoomType.Start);
        plan.Add(RoomType.Boss);

        return plan;
    }

    static void AddRooms(List<RoomType> plan, RoomType type, int count)
    {
        for (int i = 0; i < count; i++)
            plan.Add(type);
    }

    static void Shuffle(List<RoomType> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    static void EnforceRules(List<RoomType> plan)
    {
        // 🔹 Regla 1: evitar demasiadas neutrales seguidas
        int maxNeutralChain = 2;

        for (int i = 0; i < plan.Count - maxNeutralChain; i++)
        {
            if (plan[i] == RoomType.Neutral &&
                plan[i + 1] == RoomType.Neutral &&
                plan[i + 2] == RoomType.Neutral)
            {
                // Buscar un Combat para intercambiar
                int swapIndex = FindSwapIndex(plan, RoomType.Combat, i + 2);
                if (swapIndex != -1)
                {
                    (plan[i + 2], plan[swapIndex]) = (plan[swapIndex], plan[i + 2]);
                }
            }
        }

        // 🔹 Regla 2: MiniBoss no muy temprano
        int miniBossIndex = plan.IndexOf(RoomType.MiniBoss);
        if (miniBossIndex < 2)
        {
            int swapIndex = FindSwapIndex(plan, RoomType.Combat, miniBossIndex);
            if (swapIndex != -1)
            {
                (plan[miniBossIndex], plan[swapIndex]) = (plan[swapIndex], plan[miniBossIndex]);
            }
        }
    }

    static int FindSwapIndex(List<RoomType> plan, RoomType target, int start)
    {
        for (int i = start; i < plan.Count; i++)
        {
            if (plan[i] == target)
                return i;
        }
        return -1;
    }
}