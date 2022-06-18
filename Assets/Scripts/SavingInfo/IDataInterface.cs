using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//not a class, an interface where one class implements this, not inherits from
/// <summary>
/// by convention start with a capital I
/// all classes with data which needs to be saved must use this class
/// any void inside of it will need to be implemented in any class which implements this interface
/// </summary>
public interface IDataInterface
{
    void LoadData(SaveableData saveableData);

    //use a ref so actually get the reference and can modify the data itself
    void SaveData(ref SaveableData saveableData);
}
