using System;
using System.Collections.Generic;
using DifferentMethods.Extensions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.Linq;

namespace FSM
{
    public partial class FiniteStateMachineEditorWindow
    {

        void HandleKeyEvents()
        {
            if (Event.current.keyCode == KeyCode.Escape)
            {
                hotState = null;
                hotTransition = null;
                dragState = null;
                dropZone = null;
            }
        }

        void HandleCommandEvents()
        {
            var ev = Event.current;
            if (ev.type == EventType.ValidateCommand)
            {
                switch (ev.commandName)
                {
                    case "Delete":
                        if (selectedState != null) ev.Use();
                        break;
                }
            }
            if (Event.current.type == EventType.ExecuteCommand)
            {
                switch (Event.current.commandName)
                {
                    case "Delete":
                        Undo.RegisterCompleteObjectUndo(target, "Delete State");
                        target.states.Remove(selectedState);
                        break;
                }
            }
        }

        void OnEnable()
        {
            Undo.undoRedoPerformed -= Repaint;
            Undo.undoRedoPerformed += Repaint;
        }

        void OnDisable()
        {
            Undo.undoRedoPerformed -= Repaint;
        }
    }
}