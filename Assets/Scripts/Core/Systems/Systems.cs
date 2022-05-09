using UnityEngine;
/// <summary>
/// Don't specifically need anything here other than the fact it's persistent.
/// I like to keep one main object which is never killed, with sub-systems as children.
/// </summary>
public class Systems : PersistentSingleton<Systems>
{
    // Currently not needed due to SceneManagement being the 
    // 
    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    //public static void Execute()
    //{
    //    if (!GameObject.Find("Systems"))
    //    {
    //        Instantiate(Resources.Load("Systems"));
    //        print("Initialized Systems prefab.");
    //    }
    //    else
    //    {
    //        print("Systems prefab already exists.");
    //    }
    //}
}
