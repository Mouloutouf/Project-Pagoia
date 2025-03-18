using System.Collections.Generic;
using UnityEngine;

#region Simple
// Single Responsibility Principle Broken
// Open-closed Principle Broken

public class _BrokenEnemy
{
    // Lol scripting is so easy bro

    public float speed;
    private float currentSpeed;

    public float health;
    private float currentHealth;

    public void Attack() { }
    public void Move() { }

    public void Display() { }

    public void Damage() { }
    public void Heal() { }
    public void Die() { }
}
#endregion

#region Complex
// No it isn't
// Single Responsibility Principle Respected (Split of the Broken Enemy into multiple objects, with single responsibilities

#region Data Scriptables
public class _Data : ScriptableObject
{
}

public class _EntityData : _Data
{
    public float health;
}

public class _EnemyData : _EntityData
{
    public float speed;

    public List<_Action> actions;
}
#endregion

#region Game Elements
public abstract class _GameElement<T> : MonoBehaviour where T : _Data
{
    public T data;
}

public class _Entity : _GameElement<_EntityData>, IHealthElement, IDisplayElement
{
    public new _EntityData data;

    public float Health => data.health;
    public float CurrentHealth { get; private set; }

    public void Damage(float _value) { }
    public void Heal(float _value) { }
    public void Die() { }

    public void Display() { }
}

#region Enemies
public class _BrokenLiskovEnemy : _Entity
{
    public _EnemyData enemyData;

    public float Speed => enemyData.speed;
    public List<_Action> Actions => enemyData.actions;
}

public class _Enemy : _Entity
{
    // This would usually not work with the Liskov substitution principle, but here, it's using a substituted type for the data too, so I can override it,
    // and it takes it as a valid substitution.
    public new _EnemyData data;

    public float Speed => data.speed;
    public List<_Action> Actions => data.actions;
}
#endregion
#endregion

#region Actions
public class _Action
{
    public void Execute() { }
}
public class _MoveAction : _Action
{
}
public class _AttackAction : _Action
{
}
#endregion

#region Interfaces
public interface IDisplayElement {
    public void Display();
}

public interface IHealthElement {
    public void Damage(float _value);
    public void Heal(float _value);
    public void Die();
}
#endregion

#region User Behaviours
public class _Displayer : MonoBehaviour
{
    public void DisplayEntities(IDisplayElement[] _displayables)
    {
        foreach (var displayable in _displayables)
        {
            displayable.Display();
        }
    }
}

public class _DealDamage : MonoBehaviour
{
    public LayerMask hitLayer;
    public float damage;

    private void OnTriggerEnter(Collider _other)
    {
        if (_other.gameObject.layer == hitLayer.value)
        {
            if (_other.TryGetComponent(out IHealthElement healthElement))
            {
                OnHit(healthElement, damage);
            }
        }
    }

    public void OnHit(IHealthElement _healthElement, float _dmg)
    {
        _healthElement.Damage(_dmg);
    }
}
#endregion
#endregion
