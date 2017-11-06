using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
	public float maxHealth;
	public bool regen;
	public float regenRate;

	float health;

	private void Awake()
	{
		health = maxHealth;
	}

	private void Update()
	{
		if (regen)
			RegenerateHealth(regenRate * Time.deltaTime);
	}

	public void TakeDamage(float damage)
	{
		health = Mathf.Clamp(health - damage, 0.0f, maxHealth);

		if (Mathf.Abs(health) < Mathf.Epsilon)
		{
			Destroy(gameObject);
		}
	}

	public void RegenerateHealth(float hp)
	{
		health = Mathf.Clamp(health + hp, 0.0f, maxHealth);
	}

	public float GetHealth()
	{
		return health;
	}

	public float GetMaxHealth()
	{
		return maxHealth;
	}
}
