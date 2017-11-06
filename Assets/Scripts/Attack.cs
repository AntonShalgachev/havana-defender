using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
	public float damage;
	public bool continuous;
	public float period;

	public string targetLayer;

	float delay = 0.0f;

	private void Update()
	{
		delay -= Time.deltaTime;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (!continuous)
		{
			TryDealDamage(collision.gameObject);
		}
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		if (continuous && delay < 0.0f)
		{
			TryDealDamage(collision.gameObject);
			delay = period;
		}
	}

	void TryDealDamage(GameObject obj)
	{
		if (obj.layer != LayerMask.NameToLayer(targetLayer))
			return;

		var health = obj.GetComponent<Health>();
		if (health)
		{
			health.TakeDamage(damage);
		}
	}
}
