using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace FSM
{
    public partial class FiniteStateMachineEditorWindow
    {
        struct DropZone
        {
            public Rect rect;
            public int index;
            public bool last;

            public DropZone(Rect rect, int index, bool lastZone = false)
            {
                this.index = index;
                this.last = lastZone;
                this.rect = rect;
                if (lastZone) //use bottom of rect instead of top
                    this.rect.y = this.rect.yMax + 4;
                else
                    this.rect.y -= 20;
                this.rect.height = 16;
            }
        }

        DropZone? dropZone;
        List<DropZone> dropZones = new List<DropZone>();

        void StartDrag()
        {
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.paths = new string[] { "states.woot." };
            DragAndDrop.visualMode = DragAndDropVisualMode.Move;
            DragAndDrop.StartDrag(selectedState.name);
            dragState = selectedState;
        }

        void UpdateDrag()
        {
            var pos = Event.current.mousePosition;
            dropZone = null;
            if (DragAndDrop.paths != null && DragAndDrop.paths.Length == 1 && DragAndDrop.paths[0] == "states.woot.")
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                foreach (var dz in dropZones)
                {
                    var rect = dz.rect;
                    if (rect.Contains(pos))
                    {
                        dropZone = dz;
                        DragAndDrop.AcceptDrag();
                        return;
                    }
                }
            }
        }

        void ExitDrag()
        {
            dragState = null;
            dropZone = null;
        }

        void PerformDrag()
        {
            if (dropZone != null)
            {
                Undo.RecordObject(target, "Drag & Drop");
                var index = target.states.IndexOf(dragState);
                var dz = dropZone.Value;
                if (dz.index < index)
                {
                    target.states.RemoveAt(index);
                    target.states.Insert(dz.index, dragState);
                }
                if (dz.index > index)
                {
                    target.states.Insert(dz.index, dragState);
                    target.states.RemoveAt(index);
                }
            }
            ExitDrag();
        }

        void HandleDragAndDropEvents()
        {
            var e = Event.current;
            if (e.type == EventType.MouseDrag && e.delta.sqrMagnitude > 1 && selectedState != null && selectedState.backgroundRect.Contains(e.mousePosition) && e.button == 0)
            {
                StartDrag();
                e.Use();
            }
            if (e.type == EventType.DragUpdated)
            {
                UpdateDrag();
                e.Use();
            }
            if (e.type == EventType.DragExited)
            {
                ExitDrag();
                e.Use();
            }
            if (e.type == EventType.DragPerform)
            {
                PerformDrag();
                e.Use();
            }

            if (dropZone != null)
            {
                var rect = dropZone.Value.rect;
                // if (dropZone.Value.last)
                //     rect.y = rect.yMax;
                EditorGUI.DrawRect(rect, Color.white);
            }

        }



    }
}