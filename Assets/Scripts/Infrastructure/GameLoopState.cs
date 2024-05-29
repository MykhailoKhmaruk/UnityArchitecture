using UnityEngine;

namespace Infrastructure
{
    public class GameLoopState : IState, IUpdatable
    {
        public GameLoopState(GameStateMachine stateMachine)
        {
        }

        public void Enter()
        {
        }

        public void Exit()
        {
        }

        public void Update()
        {
            // Debug.Log($"=> Game loop update {Time.frameCount}");
        }
    }
}