using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class MeleAttackStrategy : IAttackStrategy
{
    private readonly float attackDamage;
    private readonly PlayerDetector playerDetector;
    
    public MeleAttackStrategy(float attackDamage, PlayerDetector playerDetector)
    {
        this.attackDamage = attackDamage;
        this.playerDetector = playerDetector;
}

    public void Execute()
    {
        LifeSystem lifesystem = playerDetector.Player.GetComponent<LifeSystem>();
        lifesystem.ReceiveDamage(attackDamage);
    }
}