using System.Collections;
using System.Net.WebSockets;
using UnityEditor;
using UnityEngine;

public class Mover : MonoBehaviour
{
	//[SerializeField]
	//private float _moveTime = 1f;
	//	[SerializeField]
	//	private float _delayTime = 2f;
	//[SerializeField]
	//private Vector3[] _positions;
    [SerializeField]
	private Vector3 _start;
	[SerializeField]
	private Vector3 _end;
	[SerializeField]
	private float _speed;
	[SerializeField]
	private float _delay;

    private IEnumerator Start()
    {
		var center = transform.position;

		var distance = Vector3.Distance(_end, _start);

		var timeNeeded = distance / _speed;

		yield return StartCoroutine(Move(_start + center, _end + center, timeNeeded));
	}

    private IEnumerator Move(Vector3 _start, Vector3 _end, float timeNeeded)
    {
		var time = 0f;
		while (true)
        {
            transform.position = Vector3.Lerp(_start, _end, time / timeNeeded);
            time += Time.deltaTime;
            while (time >= timeNeeded)
            {
                time = 0f;
				(_start, _end) = (_end, _start);
                yield return new WaitForSeconds(_delay);
            }
			yield return null;
		}

	}
	
    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
            return;
        
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(gameObject.transform.position + _start, 0.3f);
		Gizmos.DrawSphere(gameObject.transform.position + _end, 0.3f);
		Gizmos.DrawLine(gameObject.transform.position + _start, gameObject.transform.position + _end);
    }
}
