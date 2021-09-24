using UnityEngine;

public class XObsController : MonoBehaviour
{
	private static readonly int TakeHit = Animator.StringToHash("takeHit");

	private void OnCollisionEnter(Collision other)
	{
		if(!other.collider.CompareTag("Player")) return;
		
		GetComponent<Animator>().SetTrigger(TakeHit);
		var rb = GetComponent<Rigidbody>();
		rb.isKinematic = false;
		rb.AddExplosionForce(350f, other.contacts[0].point, 5f);
	}
}
