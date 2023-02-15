using UnityEngine;
using UniRx;
using System;
using UniRx.Triggers;

namespace Runner.Game
{
    public class PlayerInputController : MonoBehaviour
    {
        [SerializeField] private KeyCode executeKey = KeyCode.Mouse0;
        private IObservable<Unit> _onExecuteInputObservable;
        public IObservable<Unit> OnExecuteInputObservable => _onExecuteInputObservable;

        private void Awake()
        {
            _onExecuteInputObservable = this.UpdateAsObservable().Where(_ => Input.GetKeyDown(executeKey));
        } 
    }
}
