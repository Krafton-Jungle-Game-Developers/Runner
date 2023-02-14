using UnityEngine;
using UniRx;
using System;

namespace Runner.Game
{
    public class PlayerInputController : MonoBehaviour
    {
        [SerializeField] private KeyCode executeKey = KeyCode.Mouse0;
        private Subject<Vector3> _onExecuteInput = new();
        public IObservable<Vector3> OnExecuteInputObservable => _onExecuteInput;
        
        private void Update()
        {
            if (Input.GetKeyDown(executeKey)) { _onExecuteInput.OnNext(transform.position); }
        }
    }
}
