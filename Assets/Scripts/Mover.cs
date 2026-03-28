using System.Collections;
using System.Net.WebSockets;
using UnityEditor;
using UnityEngine;

public class Mover : MonoBehaviour
{
	//	[SerializeField]
	//	private float _moveTime = 1f;
	//	[SerializeField]
	//	private float _delayTime = 2f;
	[SerializeField]
	private Vector3[] _positions;
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
		while (true)
		{
            yield return StartCoroutine(Move(_start, _end));

            yield return new WaitForSeconds(_delay);

            yield return StartCoroutine(Move(_end, _start));

            yield return new WaitForSeconds(_delay);
        }

        yield return null;
		//if(_positions.Length < 2) yield break;
		//int prev = 0, curr = 1;
		//var time = 0f;
		//var transform = this.transform;
		//while(true)
		//{
		//	transform.position = Vector3.Lerp(_positions[prev], _positions[curr], time / _moveTime);
		//	time += Time.deltaTime;
		//	if(time >= _moveTime)
		//	{
		//		time = 0f;
		//		prev = curr;
		//		curr = (curr + 1) % _positions.Length;
		//		yield return new WaitForSeconds(_delayTime);
		//	}

		//	yield return null;
		//}
	}

    private IEnumerator Move(Vector3 _start, Vector3 _end)
    {
		var distans = Vector3.Distance(_start, _end);

		var timeNeeded = distans / _speed;

		var time = 0f;

		while (time < timeNeeded)
		{
			transform.position = Vector3.Lerp(transform.position, _start, time / timeNeeded);
			time += Time.deltaTime;

            yield return null;
        }
		//transform.position = _end;
    }

    private void OnDrawGizmos()
    {
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(_start, 1);
		Gizmos.DrawSphere(_end, 1);
		Gizmos.DrawLine(_start, _end);
    }
}
