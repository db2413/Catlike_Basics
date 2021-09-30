using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWeightedDistributionByTotalArea <T>
{
    SortedList<float, T> areas;
    int seed;
    System.Random rand;
    float totalArea;

    public RandomWeightedDistributionByTotalArea(SortedList<float, T> areas, int seed)
    {
        this.areas = areas;
        this.seed = seed;
        rand = new System.Random(seed);
        totalArea = 0;
        foreach (var area in areas.Keys)
        {
            totalArea += area;
        }
    }
    public T Next()
    {
        seed++;
        float targetArea = (float)rand.NextDouble() * totalArea;
        return  areas[Search(targetArea)];
    }

    int Search(float target)
    {
        var sortedAreas = areas.Keys;

        // Edge cases
        if (target < sortedAreas[0])
        {
            return 0;
        }
        else if (target > sortedAreas[sortedAreas.Count-1]) {
            return sortedAreas.Count - 1;
        }

        int lo = 0;
        int high = sortedAreas.Count - 1;

        while (lo <= high)
        {
            int mid = (lo + high) / 2;

            if (target < sortedAreas[mid])
            {
                high = mid - 1;
            }
            else if (target > sortedAreas[mid])
            {
                lo = mid + 1;
            }
            else
            {
                return mid;
            }
            // lo = hi + 1 , so desired value is inbetween these two indices. Return lo since that area belongs to it
        }
        return lo;
    }
}
