using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentCollectible : MonoBehaviour
{
    public static ShuffleBag<GameObject> shuffleBag;
    [SerializeField] GameObject[] parentParts;

    [SerializeField]GameObject DestroyParticles;

    Tween adjustScaleTween;
    void Start()
    {
        if (shuffleBag == null)
        {
            shuffleBag = new ShuffleBag<GameObject>();
            shuffleBag.shuffleList = parentParts;
        }

        SpawnComponent();
    }

    // Update is called once per frame
    void SpawnComponent()
    {
        GameObject obj = Instantiate(shuffleBag.getNext(), transform);
        obj.transform.localScale /= 3;

        Quaternion randRot =  Quaternion.Euler(0, Random.Range(0, 360), 0);
        transform.rotation = randRot;
    }

    public void Collect()
    {
        if (GetComponent<Collider>())
            GetComponent<Collider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        adjustScaleTween = new Tween(transform.localScale, Vector3.zero, Time.time, 3);

        GameManager1.mCentipedeBody.currCollectedParentParts++;
        Debug.Log(GameManager1.mCentipedeBody.currCollectedParentParts);
        Instantiate(DestroyParticles, transform.position, Quaternion.identity);
    }

    private void Update()
    {
        if (adjustScaleTween != null) //once collected over time reduce the scale
        {
            transform.localScale = adjustScaleTween.UpdatePositionEaseInBounce();
            Quaternion rot = transform.rotation;
            rot.y += 5 * Time.deltaTime;

            Vector3 eulerRot = transform.rotation.eulerAngles;
            eulerRot.y += 50 * Time.deltaTime;
            transform.localRotation = Quaternion.Euler(eulerRot);//Quaternion.Euler(eulerRot.x, eulerRot.y + 5 * Time.deltaTime, eulerRot.z);

            if (transform.localScale.x < 0.05f)
                Destroy(gameObject);
        }
    }
}