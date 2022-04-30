using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStart : MonoBehaviour
{
    public GameObject Player1Prefab;
    public GameObject Player2Prefab;
    public GameObject Player1 { get; private set; }
    public GameObject Player2 { get; private set; }
    [SerializeField] private GameObject Player1Spawnpoint;
    [SerializeField] private GameObject Player2Spawnpoint;
    [SerializeField] private Material material1;
    [SerializeField] private Material materrial2;

    private PlayerInputManager inputManager;

    private void Awake()
    {
        inputManager = FindObjectOfType<PlayerInputManager>();
    }

    private void Start()
    {
        Player1 = Instantiate<GameObject>(Player1Prefab, Player1Spawnpoint.transform.position, Quaternion.identity);
        Player1.GetComponent<SpriteRenderer>().material = material1;
        Player2 = Instantiate<GameObject>(Player2Prefab, Player2Spawnpoint.transform.position, Quaternion.identity);
        Player2.GetComponent<SpriteRenderer>().material = materrial2;

        // assign the correct input to the correct player -> "InputUser" seems to be the place this information can be acces in
        // https://docs.unity3d.com/Packages/com.unity.inputsystem@1.3/manual/UserManagement.html

    }
}
