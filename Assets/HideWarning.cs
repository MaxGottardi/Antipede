using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideWarning : MonoBehaviour
{
    // Start is called before the first frame update
   public void Hide(GameObject obj)
    {
        obj.SetActive(false);
    }
}
