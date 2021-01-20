using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public Direction Direction { get; set; }

    [SerializeField] public Sprite icon;
    [SerializeField] public Sprite weaponSprite;
    [SerializeField] protected int projectileLimit;
    protected HashSet<Projectile> projectileCount;
    [SerializeField] protected List<Projectile> projectiles;

    private int _currentLevelIndex;
    private int _experience;

    public int Experience
    {
        get { return _experience; }
        set
        {
            if (value < 0)
            {
                _experience = 0;
                LevelDown();
            }
            else
            {
                _experience = value;

                if (ExperienceFraction(_currentLevelIndex) >= 1)
                    LevelUp();
                else if (ExperienceFraction(_currentLevelIndex) <= 0)
                    LevelDown();
            }
        }
    }
    public int MaxLevel { get { return projectiles.Count; } }
    protected int MaxLevelIndex { get { return MaxLevel - 1; } }

    public float ExperienceFraction()
    {
        return ExperienceFraction(_currentLevelIndex, _experience);
    }
    public float ExperienceFraction(int level)
    {
        return ExperienceFraction(level, _experience);
    }
    public float ExperienceFraction(int levelIndex, int experience)
    {
        if (levelIndex > projectiles.Count - 1 || levelIndex < 0)
        {
            Debug.LogError("Incorrect level input when retrieving experience fraction");
            return -1f;
        }
        return (float)experience / (float)projectiles[levelIndex].requiredExperience;
    }

    private void LevelUp()
    {
        if (_currentLevelIndex >= projectiles.Count - 1)
        {
            _experience = projectiles[_currentLevelIndex].requiredExperience;
            Debug.Log("Level Max, implement logic");
        }
        else
        {
            _experience -= projectiles[_currentLevelIndex].requiredExperience;
            _currentLevelIndex++;
            Debug.Log("Level Up, implement logic");
        }
    }

    private void LevelDown()
    {
        if (_currentLevelIndex <= 0)
        {
            _experience = 0;
            Debug.Log("Already on lowest possible level, implement logic");
        }
        else
        {
            _currentLevelIndex--;
            _experience = projectiles[_currentLevelIndex].requiredExperience + _experience; // + eftersom experience är negativ
            Debug.Log("Level Down, implement logic");
        }
    }
    protected Projectile GetCurrentProjectile { get { return projectiles[_currentLevelIndex]; } }

    public delegate void onProjectileDestroy(Projectile projectile);
    public onProjectileDestroy OnProjectileDestroy;
    protected virtual void DestroyProjectle(Projectile projectile) { }

    public virtual void Awake() { }
    public virtual void DoUpdate() { }
}