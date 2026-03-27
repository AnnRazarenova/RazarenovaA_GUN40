using UnityEngine;

namespace DefaultNamespace
{

    [RequireComponent(typeof(PositionSaver))]
    public class EditorMover : MonoBehaviour
    {
        private PositionSaver _save;
        private float _currentDelay;

        //todo comment: Что произойдёт, если _delay > _duration?
        //тогда у нас не получится сделать хотя бы одну запись, если продолжительность времени записи будет меньше промежутка между записями
        [SerializeField, Range(0.2f, 1.0f)]
        private float _delay = 0.5f;
        [SerializeField, Min(0.2f)]
        private float _duration = 5f;

        private void Start()
        {
            //todo comment: Почему этот поиск производится здесь, а не в начале метода Update?
            //нам нужно лишь раз обратиться к _save, а в Update мы будем обращаться к нему каждый кадр
            _save = GetComponent<PositionSaver>();
            _save.Records.Clear();

            if (_duration < _delay)
            {
                _duration = _delay * 5;
            }
        }

        private void Update()
        {
            _duration -= Time.deltaTime;
            if (_duration <= 0f)
            {
                enabled = false;
                Debug.Log($"<b>{name}</b> finished", this);
                return;
            }

            //todo comment: Почему не написать (_delay -= Time.deltaTime;) по аналогии с полем _duration?
            //потому что _delay это промежуток между записью позиции, а _duration это всё время(продолжительность) записи
            _currentDelay -= Time.deltaTime;
            if (_currentDelay <= 0f)
            {
                _currentDelay = _delay;
                _save.Records.Add(new PositionSaver.Data
                {
                    Position = transform.position,
                    //todo comment: Для чего сохраняется значение игрового времени?
                    //Чтобы в скрипте ReplayMover мы смогли использовать значение Time
                    Time = Time.time,
                });
            }
        }
    }
}