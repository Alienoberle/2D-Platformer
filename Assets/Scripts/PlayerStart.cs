using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerStart : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject Player1 { get; private set; }
    public GameObject Player2 { get; private set; }
    [SerializeField] private Vector2 Player1Spawnpoint;
    [SerializeField] private Vector2 Player2Spawnpoint;

    private PlayerInputManager inputManager;

    private void Awake()
    {
        inputManager = FindObjectOfType<PlayerInputManager>();
    }

    private void Start()
    {
        Player1 = Instantiate<GameObject>(PlayerPrefab, Player1Spawnpoint, Quaternion.identity);
        Player2 = Instantiate<GameObject>(PlayerPrefab, Player2Spawnpoint, Quaternion.identity);
        // assign the correct input to the correct player -> "InputUser" seems to be the place this information can be acces in
    }
}
