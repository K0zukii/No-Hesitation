using UnityEngine;

//Implement the Singleton pattern to make sure that only one music playing across all the scenes
public class MusicManager : MonoBehaviour
{

    public static MusicManager Instance;

    //Verify that there is no MusicManager already in scene, if not, don't destroy this
    void Awake()
    {
        //Verify if a MusicManager already exist, it that's the case we destroy it
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return; //Stop the execution of the code to not overwrite the Instance
        }

        //Otherwise, we become the unique MusicManager and protecte ourselves from scene loading
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
