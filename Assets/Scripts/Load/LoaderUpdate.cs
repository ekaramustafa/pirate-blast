using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderUpdate : MonoBehaviour
{
    private void LateUpdate()
    {
        Loader.LoadTargetScene();
    }
}