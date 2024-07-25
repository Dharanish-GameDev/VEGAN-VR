using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class CannonController : NetworkBehaviour
{
    public static CannonController Instance { get; private set; }

    [SerializeField] private List<Slicable> throwablesList = new List<Slicable>();

    [SerializeField] private float blastPower = 5;
    [SerializeField] private Transform shotPoint;

    // Cooldown duration in seconds
    [SerializeField] private float cooldownDuration = 1.0f;
    private bool isCooldown = false;
    private float cooldownTimer = 0f;

    private NetworkVariable <int> randomAvailableScicableListIndex = new NetworkVariable<int>(-1,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);


    public float BlastPower => blastPower;
    public Transform ShotPointTransform => shotPoint;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        randomAvailableScicableListIndex.OnValueChanged += RandomAvailableSlicableListIndexChanged;
    }

    private void Update()
    {
        if (isCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= cooldownDuration)
            {
                isCooldown = false;
                cooldownTimer = 0f;
            }
        }
        if (IsOwner)
        {
            if (Input.GetKeyDown(KeyCode.X) && !isCooldown)
            {
                SetRandomValue();
            }
        }

    }
    private void RandomAvailableSlicableListIndexChanged(int prev, int newValue)
    {
        if (!GameflowManager.Instance.CanPlayGame) return;
        if (newValue == -1) return;
        Slicable slicableTest = GetRandomAvailableThrowableList()[newValue];
        slicableTest.ShootThisSlicable();
        Debug.Log(slicableTest.gameObject.name + " is Fired");
        isCooldown = true;
    }
    public void SetRandomValue()
    {
        randomAvailableScicableListIndex.Value = GetRandomAvailableThrowableIndex(); // Assign the new value to the network variable
    }

    private int GetRandomAvailableThrowableIndex()
    {
        var availableThrowableList = throwablesList.Where(throwable => throwable.IsThrowableAvailable).ToList();

        if (!availableThrowableList.Any())
        {
            return -1;
        }
        else if (availableThrowableList.Count == 1)
        {
            return 0;
        }

        var randomIndex = Random.Range(0, availableThrowableList.Count);
        return randomIndex;
    }
    private List<Slicable> GetRandomAvailableThrowableList()
    {
        var availableThrowableList = throwablesList.Where(throwable => throwable.IsThrowableAvailable).ToList();
        return availableThrowableList;
    }
}