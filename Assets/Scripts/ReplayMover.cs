using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(PositionSaver))]
    public class ReplayMover : MonoBehaviour
    {
        private PositionSaver _save;

        private int _index;
        private PositionSaver.Data _prev;
        private float _duration;

        private void Start()
        {
            //todo comment: зачем нужны эти проверки?
            //чтобы не использовался несуществующий или пустой файл
            if (!TryGetComponent(out _save) || _save.Records.Count == 0)
            {
                Debug.LogError("Records incorrect value", this);
                //todo comment: Для чего выключается этот компонент?
                //чтобы отключить Update и предотвратить некорректную работу 
                enabled = false;
            }
        }

        private void Update()
        {
            var curr = _save.Records[_index];
            //todo comment: Что проверяет это условие (с какой целью)?
            //проверяет пришло ли время движения куба до след. точки
            if (Time.time > curr.Time)
            {
                _prev = curr;
                _index++;
                //todo comment: Для чего нужна эта проверка?
                //для того, чтобы остановить программу, т.к. куб прошёл уже свой путь
                if (_index >= _save.Records.Count)
                {
                    enabled = false;
                    Debug.Log($"<b>{name}</b> finished", this);
                }
            }
            //todo comment: Для чего производятся эти вычисления (как в дальнейшем они применяются)?
            //расчитывается для плавности передвижения куба от точки до точки, чем больше времени прошло с времени текущей точки для передвижения осталось тем ближе будет куб к точке
            var delta = (Time.time - _prev.Time) / (curr.Time - _prev.Time);
            //todo comment: Зачем нужна эта проверка?
            //чтобы в случае когда дельта равна NaN (деление на ноль), заменить его на 0(чтобы не использовать дельту со значением бесконечность)
            if (float.IsNaN(delta))
                delta = 0f;
            //todo comment: Опишите, что происходит в этой строчке так подробно, насколько это возможно
            //передвигает куб по прямой с точки _prev до точки _current, дельта нужна для плавности движения
            transform.position = Vector3.Lerp(_prev.Position, curr.Position, delta);
        }
    }
}