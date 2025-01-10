using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public interface IMouseHoverTarget {
    public void OnMouseHoverEnter();
    public void OnMouseHoverExit();
}

public interface IMouseClickTarget {
    public void OnMouseClick();
}

public interface IMouseDragTarget {
    public void OnMouseDragStart();
    public void OnMouseDragContinuous(Vector2 delta);
    public void OnMouseDragEnd();
}

public class MouseTracker : MonoBehaviour {
    public event EventHandler<MouseHoverEventArgs> MouseHoverChangedEvent;
    public event EventHandler<MouseClickEventArgs> MouseClickEvent;
    public event EventHandler<MouseDragEventArgs> MouseDragEvent;

    [CanBeNull] private IMouseHoverTarget _currentHoverTarget;
    [CanBeNull] private IMouseClickTarget _currentClickTarget;
    [CanBeNull] private IMouseDragTarget _currentDragTarget;
    private Vector2 _lastMouseDragPosition;

    private readonly List<RaycastHit2D> _raycastHit2DResults = new(10);
    private CameraController _cameraController;

    private void Start() {
        Physics2D.queriesHitTriggers = true;
        _cameraController = CameraController.Instance;
    }

    private void Update() {
        if (Camera.main == null) {
            Debug.LogWarning("Main camera not found");
            return;
        }

        var ray = Camera.main!.ScreenPointToRay(Input.mousePosition);
        var raySize = Physics2D. GetRayIntersection(ray, Mathf.Infinity, _raycastHit2DResults);
        var hit = raySize > 0 ? _raycastHit2DResults[0] : default;

        CheckMouseHover();
        CheckMouseClick();
        CheckMouseDrag();
        return;

        void CheckMouseHover() {
            var previousHoverTarget = _currentHoverTarget;
            if (raySize == 0) {
                _currentHoverTarget?.OnMouseHoverExit();
                _currentHoverTarget = null;
                MouseHoverChangedEvent?.Invoke(this, new MouseHoverEventArgs(null, previousHoverTarget));
                return;
            }

            var newHoverTarget = hit.collider.GetComponent<IMouseHoverTarget>();
            if (newHoverTarget == _currentHoverTarget) return;
            // Debug.Log($"Mouse hover changed from {previousHoverTarget} to {newHoverTarget}");
            _currentHoverTarget?.OnMouseHoverExit();
            _currentHoverTarget = newHoverTarget;
            _currentHoverTarget?.OnMouseHoverEnter();
            MouseHoverChangedEvent?.Invoke(this, new MouseHoverEventArgs(_currentHoverTarget, previousHoverTarget));
        }

        void CheckMouseClick() {
            if (Input.GetMouseButtonDown(0)) {
                if (raySize == 0) return;
                Debug.Log($"Mouse clicked on {hit.collider.gameObject.name}");
                var clickTarget = hit.collider.GetComponent<IMouseClickTarget>();
                if (clickTarget == null) return;
                _currentClickTarget = clickTarget;
            }
            else if (Input.GetMouseButtonUp(0)) {
                if (_currentClickTarget == null) return;
                Debug.Log($"Mouse released on {hit.collider.gameObject.name}");
                var clickTarget = hit.collider.GetComponent<IMouseClickTarget>();
                if (clickTarget == _currentClickTarget) {
                    clickTarget.OnMouseClick();
                    MouseClickEvent?.Invoke(this, new MouseClickEventArgs(clickTarget));
                }

                _currentClickTarget = null;
            }
        }

        void CheckMouseDrag() { // TODO check
            if (Input.GetMouseButtonDown(0)) {
                if (raySize == 0) return;
                Debug.Log($"Mouse drag started on {hit.collider.gameObject.name}");
                var dragTarget = hit.collider.GetComponent<IMouseDragTarget>();
                if (dragTarget == null) return;
                _currentDragTarget = dragTarget;
                _lastMouseDragPosition = _cameraController.GetMouseWorldPosition();
                dragTarget.OnMouseDragStart();
                MouseDragEvent?.Invoke(this, new MouseDragEventArgs(dragTarget, MouseDragEventArgs.MouseDragState.Start));
            }
            else if (Input.GetMouseButton(0)) {
                if (_currentDragTarget == null) return;
                // Debug.Assert(hit.collider.GetComponent<IMouseDragTarget>() == _currentDragTarget, $"Mouse drag target is not under mouse; current: {_currentDragTarget}, under mouse: {hit.collider.gameObject.name}");

                var currentMouseDragPosition = _cameraController.GetMouseWorldPosition();
                _currentDragTarget.OnMouseDragContinuous(currentMouseDragPosition - _lastMouseDragPosition);

                _lastMouseDragPosition = currentMouseDragPosition;
                MouseDragEvent?.Invoke(this, new MouseDragEventArgs(_currentDragTarget, MouseDragEventArgs.MouseDragState.Continuous));
            }
            else if (Input.GetMouseButtonUp(0)) {
                if (_currentDragTarget == null) return;
                Debug.Log($"Mouse drag ended on {hit.collider.gameObject.name}");

                _currentDragTarget.OnMouseDragEnd();
                MouseDragEvent?.Invoke(this, new MouseDragEventArgs(_currentDragTarget, MouseDragEventArgs.MouseDragState.End));
                _currentDragTarget = null;
            }
        }

    }
}

public class MouseHoverEventArgs : EventArgs {
    public IMouseHoverTarget NewHoverTarget { get; }
    public IMouseHoverTarget PreviousHoverTarget { get; }

    public MouseHoverEventArgs(IMouseHoverTarget newHoverTarget, IMouseHoverTarget previousHoverTarget) {
        NewHoverTarget = newHoverTarget;
        PreviousHoverTarget = previousHoverTarget;
    }
}

public class MouseClickEventArgs : EventArgs {
    public IMouseClickTarget ClickTarget { get; }

    public MouseClickEventArgs(IMouseClickTarget clickTarget) {
        ClickTarget = clickTarget;
    }
}

public class MouseDragEventArgs : EventArgs {
    public IMouseDragTarget DragTarget { get; }
    public MouseDragState DragState { get; }

    public MouseDragEventArgs(IMouseDragTarget dragTarget, MouseDragState dragState) {
        DragTarget = dragTarget;
        DragState = dragState;
    }

    public enum MouseDragState {
        Start,
        Continuous,
        End
    }
}