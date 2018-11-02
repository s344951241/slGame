using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class EditorCoroutineRunner {

    private class EditorCoroutine : IEnumerator
    {
        private Stack<IEnumerator> executionsStack;
        public EditorCoroutine(IEnumerator iterator)
        {
            this.executionsStack = new Stack<IEnumerator>();
            this.executionsStack.Push(iterator);
        }

        public bool MoveNext()
        {
            IEnumerator i = this.executionsStack.Peek();

            if (i.MoveNext())
            {
                object result = i.Current;
                if (result != null && result is IEnumerator)
                {
                    this.executionsStack.Push((IEnumerator)result);
                }
                return true;
            }
            else
            {
                if (this.executionsStack.Count > 1)
                {
                    this.executionsStack.Pop();
                    return true;
                }
            }
            return false;
        }

        public void Reset()
        {
            throw new System.NotSupportedException("this Operation is not supported");
        }

        public object Current
        {
            get { return this.executionsStack.Peek().Current; }
        }

        public bool Find(IEnumerator iterator)
        {
            return this.executionsStack.Contains(iterator);
        }
    }

    private static List<EditorCoroutine> editorCoroutineList;
    private static List<IEnumerator> buffer;
    public static IEnumerator StartEditorCoroutine(IEnumerator iterator)
    {
        if (editorCoroutineList == null)
        {
            editorCoroutineList = new List<EditorCoroutine>();
        }
        if (buffer == null)
        {
            buffer = new List<IEnumerator>();
        }
        if (editorCoroutineList.Count == 0)
        {
            EditorApplication.update += Update;
        }
        buffer.Add(iterator);
        return iterator;
    }

    private static bool Find(IEnumerator iterator)
    {
        foreach (EditorCoroutine editorCoroutine in editorCoroutineList)
        {
            if (editorCoroutine.Find(iterator))
            {
                return true;
            }
        }
        return false;

    }

    private static void Update()
    {
        editorCoroutineList.RemoveAll(coroutine => { return coroutine.MoveNext() == false; });

        if (buffer.Count > 0)
        {
            foreach (IEnumerator iterator in buffer)
            {
                if (!Find(iterator))
                {
                    editorCoroutineList.Add(new EditorCoroutine(iterator));
                }
            }
            buffer.Clear();
        }

        if (editorCoroutineList.Count == 0)
        {
            EditorApplication.update -= Update;
        }
    }



}
