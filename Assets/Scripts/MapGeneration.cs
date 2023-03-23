using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    void Start()
    {
        Vector2     lowest_border = new(-6.5f, -4.5f);
        Vector2     highest_border = new(6.5f, 4.5f);
        float       Step = 1;
        int         count = 0;
        int[]       FieldAsList;

        for (float i = lowest_border.x; i <= highest_border.x; i += Step)
        {
            count++;
        }
    }
}
