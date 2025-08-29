using UnityEngine;

public interface ICanInteract : ICharacter
{
    bool CanBeAttack(CampFlag camp);

    bool CanBeHealth(CampFlag camp);
}