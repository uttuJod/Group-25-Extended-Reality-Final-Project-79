using UnityEngine;

public enum BodyPartZone
{
    Head,
    Torso,
    Limb
}

public class BodyPartHitbox : MonoBehaviour
{
    public BodyPartZone zone = BodyPartZone.Torso;
    public ZombieHealth owner;

    void Awake()
    {
        if (owner == null)
            owner = GetComponentInParent<ZombieHealth>();
    }
}