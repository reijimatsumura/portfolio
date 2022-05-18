using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Request : MonoBehaviour
{
    public int requestID = 1;

    public int requestSiez = 9;

    public int[,] materialItemID = new int[,]
    {
        {  0,  0,  0},
        {  2,  0,  6},
        {  1,  0,  1},
    };
}
