using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentObject : MonoBehaviour
{
    private void Start() {
        LevelManager.Instance.EnablePersistency(gameObject);
    }
}
