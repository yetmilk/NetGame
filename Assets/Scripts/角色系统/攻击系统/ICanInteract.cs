using UnityEngine;

public interface ICanInteract : ICharacter
{
    void GetDamage(Vector3 hurtDir, float damage);

    bool CanBeAttack(CampFlag camp);

    bool CanBeHealth(CampFlag camp);

    void GetHealth(float health);
}