using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentCollectible : MonoBehaviour, IDataInterface
{
    public bool bDoSaveData, isStartWeb;
    public int ID;
    public static ShuffleBag<GameObject> shuffleBag;
    [SerializeField] GameObject[] parentParts;

    [SerializeField]GameObject DestroyParticles;

    Tween adjustScaleTween;

    GameObject childUI;
    void Start()
    {
        if (bDoSaveData && !isStartWeb)
            childUI = transform.GetChild(0).gameObject;
        if (shuffleBag == null && !isStartWeb)
        {
            shuffleBag = new ShuffleBag<GameObject>();
            shuffleBag.shuffleList = parentParts;
        }
        if (!isStartWeb)
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
        Instantiate(DestroyParticles, transform.position, Quaternion.identity);
        if (!isStartWeb)
        {
            if (GetComponent<Collider>())
                GetComponent<Collider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;
            adjustScaleTween = new Tween(transform.localScale, Vector3.zero, Time.time, 3);
            bDoSaveData = false;
            Destroy(childUI);
            GameManager1.mCentipedeBody.AddParentPartUI();
        }
        else
        {
            GameManager1.uiButtons.Movement();
            Destroy(gameObject);
        }
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

        if (bDoSaveData && childUI != null)
        {
            if (Vector3.Distance(transform.position, GameManager1.playerObj.transform.position) < 20)
            {
                if (!childUI.activeSelf)
                    childUI.SetActive(true);
            }
            else if (childUI.activeSelf)
                childUI.SetActive(false);
        }
    }

    public void LoadData(SaveableData saveableData)
    {
        if(bDoSaveData)
        {
            if (!saveableData.bParentPartActive.dictionary.ContainsKey(ID))
                Destroy(gameObject);
        }
    }

    public void SaveData(SaveableData saveableData)
    {
        if(bDoSaveData)
        {
            saveableData.bParentPartActive.dictionary.Add(ID, true);
        }
    }
}