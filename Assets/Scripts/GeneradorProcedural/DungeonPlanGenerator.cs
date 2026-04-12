using System.Collections.Generic;
using UnityEngine;

public static class DungeonPlanGenerator
{
    public static List<RoomType> GeneratePlan(System.Random rng)
    {
        List<RoomType> plan = new List<RoomType>();

        // 🔹 Contenido base
        AddRooms(plan, RoomType.Combat, 3);
        AddRooms(plan, RoomType.Neutral, 4);

        // 🔹 Mezcla
        Shuffle(plan, rng);

        // 🔹 Reglas estructurales
        EnforceRules(plan);

        // 🔹 Estructura final fija
        plan.Insert(0, RoomType.Start);
        plan.Add(RoomType.Boss);

        return plan;
    }

    static void AddRooms(List<RoomType> plan, RoomType type, int count)
    {
        for (int i = 0; i < count; i++)
            plan.Add(type);
    }

    static void Shuffle(List<RoomType> list, System.Random rng)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = rng.Next(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    static void EnforceRules(List<RoomType> plan)
    {
        int maxNeutralChain = 2;

        for (int i = 0; i < plan.Count - maxNeutralChain; i++)
        {
            if (plan[i] == RoomType.Neutral &&
                plan[i + 1] == RoomType.Neutral &&
                plan[i + 2] == RoomType.Neutral)
            {
                int swapIndex = FindSwapIndex(plan, RoomType.Combat, i + 2);

                if (swapIndex >= 0 && swapIndex < plan.Count)
                {
                    (plan[i + 2], plan[swapIndex]) = (plan[swapIndex], plan[i + 2]);
                }
            }
        }
    }

    static int FindSwapIndex(List<RoomType> plan, RoomType target, int start)
    {
        if (start < 0 || start >= plan.Count)
            return -1;

        for (int i = start; i < plan.Count; i++)
        {
            if (plan[i] == target)
                return i;
        }

        return -1;
    }
}