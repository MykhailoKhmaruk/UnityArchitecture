using System;
using System.Collections;
using System.Collections.Generic;
using Logic;
using UnityEngine;

namespace Infrastructure
{
  public class GameStateMachine
  {
    private readonly Dictionary<Type, IExitableState> _states;
    private IExitableState _activeState;
    private readonly ICoroutineRunner _coroutineRunner;
    private Coroutine _activeStateUpdate;

    public GameStateMachine(SceneLoader sceneLoader, LoadingCurtain curtain, ICoroutineRunner coroutineRunner)
    {
      _coroutineRunner = coroutineRunner;
      
      _states = new Dictionary<Type, IExitableState>
      {
        [typeof(BootstrapState)] = new BootstrapState(this, sceneLoader),
        [typeof(LoadLevelState)] = new LoadLevelState(this, sceneLoader, curtain),
        [typeof(GameLoopState)] = new GameLoopState(this)
      };
    }

    public void Enter<TState>() where TState : class, IState
    {
      IState state = ChangeState<TState>();
      state.Enter();
    }

    public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>
    {
      TState state = ChangeState<TState>();
      state.Enter(payload);
    }

    private TState ChangeState<TState>() where TState : class, IExitableState
    {
      _activeState?.Exit();
      DisableStateUpdate();

      TState state = GetState<TState>();
      _activeState = state;
      EnableStateUpdate(_activeState);

      return state;
    }

    private void EnableStateUpdate<TState>(TState state) where TState : class, IExitableState
    {
      if (state is IUpdatable updatableSate)
      {
        _activeStateUpdate = _coroutineRunner.StartCoroutine(UpdateState(updatableSate));
      }
    }

    private void DisableStateUpdate()
    {
      if (_activeStateUpdate == null)
      {
        return;
      }
      
      _coroutineRunner.StopCoroutine(_activeStateUpdate);
      _activeStateUpdate = null;
    }

    private static IEnumerator UpdateState(IUpdatable updatable)
    {
      while (true)
      {
        updatable.Update();
        yield return null;
      }
    }

    private TState GetState<TState>() where TState : class, IExitableState => _states[typeof(TState)] as TState;
  }
}