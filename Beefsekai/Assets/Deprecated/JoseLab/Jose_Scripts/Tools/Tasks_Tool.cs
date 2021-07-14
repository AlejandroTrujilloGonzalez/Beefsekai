using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tasks_Tool : MonoBehaviour
{
    void Update()
    {
        foreach (Tasks.Task t in Tasks.taskList)
        {
            if (Time.time > t.timeToInit)
            {
                t.action();
                Tasks.taskList.Remove(t);
                break;
            }
        }
    }

}


public static class Tasks
{
    public class Task
    {
        public float timeToInit;
        public Action action;
    }

    public static List<Task> taskList = new List<Task>();

    public static void NewTask(float _time, Action _action)
    {
        taskList.Add(new Task { timeToInit = Time.time + _time, action = _action });
    }


}

