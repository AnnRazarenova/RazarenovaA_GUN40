using System.Collections;
using UnityEngine;

namespace Netologia.Homework
{
	public class Player : MonoBehaviour
	{
		private bool _ready;
		private Rigidbody _ball;
		
		[SerializeField]
		private Rigidbody _ballPrefab;
		[SerializeField]
		private float _startVelocity;
		[SerializeField]
		private float _lifetime;

		[SerializeField]
		private float _respawnDelay;

		private void Update()
		{
			if (!_ready) return;
			if (Input.GetKey(KeyCode.Space))
			{
				StartCoroutine(Reloader());
				_ball.isKinematic = false;
				_ball.transform.parent = null;
				_ball.velocity = transform.forward * _startVelocity;
				Destroy(_ball.gameObject, _lifetime);
			}
		}

		private IEnumerator Reloader()
		{
			_ready = false;
			yield return new WaitForSeconds(_respawnDelay);
			Spawn();
		}

		private void Spawn()
		{
            _ball = Instantiate(_ballPrefab, transform);
            
            Vector3 parentScale = transform.lossyScale;
            Vector3 desiredScale = new Vector3(1.5f, 1.5f, 1.5f);
            _ball.transform.localScale = new Vector3(
                desiredScale.x / parentScale.x,
                desiredScale.y / parentScale.y,
                desiredScale.z / parentScale.z
            );
            _ball.isKinematic = true;
            _ready = true;
        }

		private void Start()
		{
			Spawn();
		}
	}
}