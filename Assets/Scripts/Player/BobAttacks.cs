using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class BobAttacks : MonoBehaviour
{
    private const string MB1ParamName = "MB1";
    private const string MB2ParamName = "MB2";
    private const string ShiftParamName = "LShift";

    public BoxCollider box;
    public ThirdPersonController controller;
    private Animator anim;

    [Header("Checks")]
    [SerializeField] public bool canAttack = true;
    [SerializeField] public bool canHeavyAttack = true;
    [SerializeField] public bool canShift = true;

    [Header("Combat Stats")]
    [SerializeField] public float MB1Dmg = 10f;
    [SerializeField] public float MB2Dmg = 30f;
    [SerializeField] public float sprintSPD = 30f;
    [SerializeField] public float MB1CD = 1f;
    [SerializeField] public float MB2CD = 5f;
    [SerializeField] public float ShiftCD = 10f;

    [Header("Abilities")]
    [SerializeField] private GameObject leftPunch;
    [SerializeField] private GameObject rightPunch;
    [SerializeField] private float leftPunchLifetime = 1f;
    [SerializeField] private float rightPunchLifetime = 1.5f;
    [SerializeField] private Transform punchPoint;
    
    

    private float initialMS;

    private void Awake() {
        controller = GetComponent<ThirdPersonController>();
        box = GetComponent<BoxCollider>();
        anim = GetComponent<Animator>();

        canAttack = true;
        canHeavyAttack = true;
        canShift = true;
    }
    
    private void LightPunch()
    {
        controller.canMove = false;
        anim.SetTrigger(MB1ParamName);
        GameObject spawnedPunch = Instantiate(leftPunch, punchPoint.position, punchPoint.rotation);
        Destroy(spawnedPunch, leftPunchLifetime);
        StartCoroutine(BasicAtkCoroutine());
    }

    private void HeavyPunch()
    {
        controller.canMove = false;
        anim.SetTrigger(MB2ParamName);
        GameObject spawnedPunch = Instantiate(rightPunch, punchPoint.position, punchPoint.rotation);
        Destroy(spawnedPunch, rightPunchLifetime);
        StartCoroutine(HeavyAtkCoroutine());
    }

    private void Sprint()
    {
        box.enabled = false;
        initialMS = controller.moveSpeed;
        controller.moveSpeed = sprintSPD;
        anim.SetTrigger(ShiftParamName);
        StartCoroutine(ShiftCoroutine());
    }

    private IEnumerator BasicAtkCoroutine()
    {
        yield return new WaitForSeconds(1f);
        controller.canMove = true;
        yield return new WaitForSeconds(Mathf.Abs(MB1CD - 1f));
        canAttack = true;
    }

    private IEnumerator HeavyAtkCoroutine()
    {
        yield return new WaitForSeconds(0.9f);
        controller.canMove = true;
        yield return new WaitForSeconds(Mathf.Abs(MB2CD - 1f));
        canHeavyAttack = true;
    }

    private IEnumerator ShiftCoroutine()
    {
        yield return new WaitForSeconds(1f);
        controller.moveSpeed = initialMS;
        box.enabled = true;
        yield return new WaitForSeconds(Mathf.Abs(ShiftCD -1f));
        canShift = true;
    }

    private void OnBasicAtk()
    {
        if (canAttack && controller.isGrounded)
        {
            controller.canMove = false;
            canAttack = false;
            LightPunch();
        }
    }

    private void OnHeavyAtk()
    {
        if (canHeavyAttack && controller.isGrounded)
        {
            controller.canMove = false;
            canHeavyAttack = false;
            HeavyPunch();
        }
    }
    
    private void OnDash()
    {
        if (canShift && controller.isGrounded)
        {
            Sprint();
            canShift = false;
        }
    }
}
