using UnityEngine;

public interface IPooledObject {
    //every object that extends this interface can access this function.
    void OnObjectSpawn();
}
