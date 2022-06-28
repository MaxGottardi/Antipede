using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMovement : MonoBehaviour, IDataInterface
{
    public int ID;
    public Vector3 startPosition;
    bool doMove;


    // Start is called before the first frame update
    void Start()
    {
        startPosition = this.gameObject.transform.position;
        doMove = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (doMove)
        {
            if (this.gameObject.transform.position.y >= -20.0f)
            {
                this.gameObject.transform.Translate(Vector3.down * Time.deltaTime * 1f);
            }
        }
    }

    public void SetMovable()
    {
        doMove = true;
    }

    public void LoadData(SaveableData saveableData)
    {
        doMove = saveableData.wallMove.dictionary[ID];
        if (doMove)
        {
            Vector3 pos = gameObject.transform.position;
            pos.y -= 20f;
            gameObject.transform.position = pos;
        }
    }

    public void SaveData(SaveableData saveableData)
    {
        saveableData.wallMove.dictionary.Add(ID, doMove);
    }
}
