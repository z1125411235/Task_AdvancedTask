using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MethodHelper
{

    public static string LoadResourceTextfile(string path)
    {
        TextAsset targetFile = Resources.Load<TextAsset>(path);
        return targetFile.text;
    }

}
