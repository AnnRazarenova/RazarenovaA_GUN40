using System;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using UnityEngine;

namespace DefaultNamespace
{
    public class PositionSaver : MonoBehaviour
    {
        [Serializable]
        public struct Data
        {
            public Vector3 Position;
            public float Time;
        }

        [SerializeField, ReadOnly, Tooltip("To fill in this field, use the context menu in the inspector and the “Create File” command")]
        private TextAsset _json;

        [SerializeField, HideInInspector]
        public List<Data> Records { get; private set; }

        private void Awake()
        {
            //todo comment: Что будет, если в теле этого условия не сделать выход из метода?
            //программа продолжит работу и, если поле _json будет равен null, в строке 31 произойдёт ошибка при обращении к _json
            if (_json == null)
            {
                gameObject.SetActive(false);
                Debug.LogError("Please, create TextAsset and add in field _json");
                return;
            }

            SaveData saveData = JsonUtility.FromJson<SaveData>(_json.text);
            if (saveData != null && saveData.Records != null)
            {
                Records = saveData.Records;
            }

            JsonUtility.FromJsonOverwrite(_json.text, this);
            //todo comment: Для чего нужна эта проверка (что она позволяет избежать)?
            //она позволяет избежать дальнейшего обращения к null списку(в случае если список Records равен null, то выделяется память на список с 10 элементами)
            if (Records == null)
                Records = new List<Data>(10);
        }

        private void OnDrawGizmos()
        {
            //todo comment: Зачем нужны эти проверки (что они позволляют избежать)?
            //она позволяет избежать использования null списка или списка с 0 элементами
            if (Records == null || Records.Count == 0) return;
            var data = Records;
            var prev = data[0].Position;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(prev, 0.3f);
            //todo comment: Почему итерация начинается не с нулевого элемента?
            //потому что нулевой элемент уже присвоен prev(предыдущему значению) и он уже прорисован(прошлая позиция объекта), а curr текущая позиция как раз после prev 
            for (int i = 1; i < data.Count; i++)
            {
                var curr = data[i].Position;
                Gizmos.DrawWireSphere(curr, 0.3f);
                Gizmos.DrawLine(prev, curr);
                prev = curr;
            }
        }

        [Serializable]
        private class SaveData
        {
            public List<Data> Records;
        }

#if UNITY_EDITOR
        [ContextMenu("Create File")]
        private void CreateFile()
        {
            //todo comment: Что происходит в этой строке?
            //создаётся файл с названием Path и типом txt, в папке проета(ассетс)
            var stream = File.Create(Path.Combine(Application.dataPath, "Path.txt"));
            //todo comment: Подумайте для чего нужна эта строка? (а потом проверьте догадку, закомментировав)
            //нужен для освобождения ресурсов файлового потока
            //stream.Dispose();
            UnityEditor.AssetDatabase.Refresh();
            //В Unity можно искать объекты по их типу, для этого используется префикс "t:"
            //После нахождения, Юнити возвращает массив гуидов (которые в мета-файлах задаются, например)
            var guids = UnityEditor.AssetDatabase.FindAssets("t:TextAsset");
            foreach (var guid in guids)
            {
                //Этой командой можно получить путь к ассету через его гуид
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                //Этой командой можно загрузить сам ассет
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                //todo comment: Для чего нужны эти проверки?
                //для поиска нужного нам ассета и чтобы он не был равен null
                if (asset != null && asset.name == "Path")
                {
                    _json = asset;
                    UnityEditor.EditorUtility.SetDirty(this);
                    UnityEditor.AssetDatabase.SaveAssets();
                    UnityEditor.AssetDatabase.Refresh();
                    //todo comment: Почему мы здесь выходим, а не продолжаем итерироваться?
                    //чтобы дальше не сикать по гуидам ассеты, т.к. нужный мы уже нашли 
                    return;
                }
            }
        }

        private void OnDestroy()
        {
            if (_json == null)
                return;

            SaveData saveData = new SaveData { Records = this.Records };

            // Сериализуем в JSON
            string text = JsonUtility.ToJson(saveData, true);

            string path = UnityEditor.AssetDatabase.GetAssetPath(_json);

            File.WriteAllText(path, text);

            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }
#endif
    }
}