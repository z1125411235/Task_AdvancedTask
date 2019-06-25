using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour {

    public static Dictionary<int, string> items = new Dictionary<int, string>()
    {
        {0, "Apple"},
        {1, "Stick"},
        {2, "Paper"},
    };
}
