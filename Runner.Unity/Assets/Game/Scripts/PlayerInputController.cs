using UnityEngine;
using UniRx;
using System;
using UniRx.Triggers;

namespace Runner.Game
{
    public class PlayerInputController : MonoBehaviour
    {
        [SerializeField] private KeyCode executeKey = KeyCode.Mouse0;
        private IObservable<Vector3> _onExecuteInputObservable;
        public IObservable<Vector3> OnExecuteInputObservable => _onExecuteInputObservable;

        private void Awake()
        {
            _onExecuteInputObservable = this.UpdateAsObservable().Where(_ => Input.GetKeyDown(executeKey))
                                                                 .Select(_ => transform.position);
        }
    }
}
