using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;
using UnityEngine.UI;

public class TextManipulator : MonoBehaviour
{

    [SerializeField] Text text;
    MCentipedeBody body;
    float multiplier;

    [SerializeField]float speed;
    [SerializeField]float range;

    [SerializeField] float shakeSpeed;
    [SerializeField] float shakeRate;

    float startingShakeRate;

    Vector3 startingPosition;

    // Start is called before the first frame update
    void Start()
    {
        body = Object.FindObjectOfType<MCentipedeBody>();
        startingPosition = this.transform.position;
        startingShakeRate = shakeRate;
    }

    // Update is called once per frame
    void Update()
    {

        multiplier = body.GetMultiplier();
        text.text = "x" + multiplier;
        range = Mathf.Clamp(multiplier, 2, 10);
        float shake = Mathf.Sin(Time.time * shakeSpeed) * shakeRate;

        if (multiplier == 0)
        {
            this.transform.position = startingPosition;
        }
        else
        {
            text.fontSize = 26 + Mathf.RoundToInt(Mathf.Sin(Time.time * speed) * range);
        }

        if (multiplier > 10)
        {
            text.transform.position = new Vector3(startingPosition.x + shake, startingPosition.y, 0);
        }
        if (multiplier > 20)
        {
            shakeRate = startingShakeRate * 2;
            text.transform.position = new Vector3(startingPosition.x + shake, startingPosition.y, 0);
        }

        //transform.position = new Vector3(transform.position.x, (Mathf.Sin(Time.time) * kBobHeight + 1) * .5f + 0.8f, transform.position.z);
    }
}
