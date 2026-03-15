using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoundsCheck))]
public class ProjectileHero : MonoBehaviour
{
    private BoundsCheck bndCheck;
    private Renderer rend;

    [Header("Dynamic")]
    public Rigidbody rigid;
    [SerializeField]                                                         // a
    private eWeaponType _type;

    [Header("Phaser Settings")]
    public float waveFrequency = 20f;
    public float waveAmplitude = 5f;

    // This public property masks the private field _type
    public eWeaponType type
    {                                              // c
        get { return (_type); }
        set { SetType(value); }
    }


    void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        rend = GetComponent<Renderer>();                                     // d
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Remove Laser
        if (_type != eWeaponType.laser)
        {
            if (bndCheck.LocIs(BoundsCheck.eScreenLocs.offUp))
            {
                Destroy(gameObject);
            }
        }

        // Phaser wave 
        if (_type == eWeaponType.phaser)
        {
            Vector3 v = rigid.linearVelocity;
            v.x = Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;
            rigid.linearVelocity = v;
        }

        // Laser movement
        if (_type == eWeaponType.laser && Hero.S != null)
        {
            Vector3 pos = transform.position;
            pos.x = Hero.S.transform.position.x;
            pos.y = Hero.S.transform.position.y + 15f;
            transform.position = pos;
        }
    }

    void OnTriggerStay(Collider other)
    {
        // lasers should use continuous damage
        if (_type != eWeaponType.laser) return;

        Enemy e = other.GetComponent<Enemy>();

        if (e != null)
        {
            WeaponDefinition def = Main.GET_WEAPON_DEFINITION(_type);

            // apply damage over time
            e.TakeDamage(def.damagePerSec * Time.deltaTime);
        }
    }

    /// <summary>
    /// Sets the _type private field and colors this projectile to match the 
    ///   WeaponDefinition.
    /// </summary>
    /// <param name="eType">The eWeaponType to use.</param>
    public void SetType(eWeaponType eType)
    {
        _type = eType;
        WeaponDefinition def = Main.GET_WEAPON_DEFINITION(_type);
        rend.material.color = def.projectileColor;
    }

    /// <summary>
    /// Allows Weapon to easily set the velocity of this ProjectileHero
    /// </summary>
    public Vector3 vel
    {
        get { return rigid.linearVelocity; }
        set
        {
            if (!rigid.isKinematic)
            {
                rigid.linearVelocity = value;
            }
        }
    }
}