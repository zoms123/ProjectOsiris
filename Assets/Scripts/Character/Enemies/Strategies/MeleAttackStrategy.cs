using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class MeleAttackStrategy : IAttackStrategy
{
    private readonly Vector3 attackPoint;
    private readonly float radius;
    private readonly LayerMask whatIsDamageable;
    private readonly string tag;
    private readonly float attackDamage;
    private readonly PlayerDetector playerDetector;
    
    public MeleAttackStrategy(Vector3 attackPoint, float radius, LayerMask whatIsDamageable, string tag, float attackDamage, PlayerDetector playerDetector)
    {
        this.attackPoint = attackPoint;
        this.radius = radius;
        this.whatIsDamageable = whatIsDamageable;
        this.tag = tag;
        this.attackDamage = attackDamage;
        this.playerDetector = playerDetector;
}

    public void Execute()
    {
        LifeSystem lifesystem = playerDetector.Player.GetComponent<LifeSystem>();
        lifesystem.ReceiveDamage(attackDamage); // TODO check the coroutine of this method
        /*
        Collider[] collidersTouched = Physics.OverlapSphere(attackPoint, radius, LayerMask.NameToLayer("HitBox"));
        foreach (Collider collider in collidersTouched)
        {
            if (!collider.gameObject.CompareTag(tag))
            {
                LifeSystem lifesystem = collider.gameObject.GetComponent<LifeSystem>();
                lifesystem.ReceiveDamage(attackDamage); // TODO check the coroutine of this method
            }
        }
        */
    }
}