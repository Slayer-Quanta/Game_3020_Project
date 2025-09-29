using UnityEngine;

public class GlobalReferences : MonoBehaviour
{
    //This code is for making the bullet impact effect in the area that has tags enemy or target
    public static GlobalReferences Instance{ get; set; }

    public GameObject bulletimpactEffectPrefab;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }



}
