using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFire : MonoBehaviour
{
    private void Start()
    {
        Projectile.hasSeenChanged = false;
        Debug.Log(SettingsVariables.boolDictionary["bShootToActivate"] + "hsttd");
    }

    public Weapon[] weapons;
    // Update is called once per frame
    void Update()
    {
        if (SettingsVariables.boolDictionary["bShootToActivate"])
        {
            foreach (Weapon W in weapons)
            {
                Vector3 mousePos = MouseToWorldCoords();
                W.LookAt(mousePos);
                if (Input.GetMouseButtonDown(0))
                    W.Fire(mousePos);
            }
        }
    }

    Vector3 MouseToWorldCoords()
    {
        Ray Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(Ray, out RaycastHit Hit, 5000, 384); // Enemy and Ground Layers. (1 << 7 | 1 << 8)

        return Hit.point;
    }

    public void AdjustTxtValue(Slider slider, Text text)
    {
        text.text = slider.value.ToString();
    }
}
