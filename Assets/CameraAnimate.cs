using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimate : MonoBehaviour
{
    Tween tween = null;
    // Start is called before the first frame update
    [SerializeField]Vector3 creditsPos, mainPos, settingsPos;
    [SerializeField]Quaternion creditsRot, mainRot, settingsRot;
    public GameObject mainMenuObj, creditsObj, settingsObj;
    GameObject currActive = null, newActive;
    float duration = .25f;

    bool nearGoal = false;
    private void Start()
    {
        transform.position = mainPos;
        transform.rotation = mainRot;
        currActive = mainMenuObj;
    }
    public void MoveToCredits()
    {
        tween = new Tween(transform.position, creditsPos, transform.rotation, creditsRot, Time.time, duration);
        newActive = creditsObj;
        creditsObj.SetActive(true);
        nearGoal = false;
    }
    public void MoveToMainMenu()
    {
        tween = new Tween(transform.position, mainPos, transform.rotation, mainRot, Time.time, duration);
        newActive = mainMenuObj;
        mainMenuObj.SetActive(true);
        nearGoal = false;
    }
    public void MoveToSettings()
    {
        tween = new Tween(transform.position, settingsPos, transform.rotation, settingsRot, Time.time, duration);
        newActive = settingsObj;
        settingsObj.SetActive(true);
        nearGoal = false;
    }

    private void Update()
    {
        if (tween != null && !nearGoal)// && (Time.time - tween.StartTime) / tween.Duration >= 1)
        {
            transform.position = tween.UpdatePosition();
            transform.rotation = tween.UpdateRotation();
            if(Vector3.Distance(transform.position, tween.EndPos) < 0.01f)
            {
                Projectile.hasSeenChanged = false;
                if (currActive != null)
                    currActive.SetActive(false);
                currActive = newActive;
                nearGoal = true;
                transform.position = tween.EndPos;
                transform.rotation = tween.EndRot;
            }
        }
    }
}
