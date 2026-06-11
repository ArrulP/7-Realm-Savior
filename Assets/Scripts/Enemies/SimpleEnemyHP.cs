using UnityEngine;

public class SimpleEnemyHP : MonoBehaviour
{
    [Header("Player Attack Handlers")]
    [SerializeField] private BobAttacks bob;


    [Header("Health")]
    [SerializeField] private float currHP = 100f;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "BobAbility")
        {

            float dmg = 0f;

            if (!bob.canAttack)
            {
                dmg = bob.MB1Dmg;
                
            }else if (!bob.canHeavyAttack)
            {
                dmg = bob.MB2Dmg;
            }
            else
            {
                dmg = bob.MB1Dmg;
            }

            currHP -= dmg;
            if(currHP <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}
