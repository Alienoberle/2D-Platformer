using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public IState currentState { get; private set; }
    private Dictionary<Type, List<Transition>> transitionList = new Dictionary<Type, List<Transition>>();
    private List<Transition> currentTransitions = new List<Transition>();
    private List<Transition> anyTransitions = new List<Transition>();
    private static List<Transition> emptyTransitions = new List<Transition>(0);

    public void AddTransition(IState from, IState to, Func<bool> predicate)
    {
        if (transitionList.TryGetValue(from.GetType(), out var transitions) == false)
        {
            transitions = new List<Transition>();
            //transitionList[from.GetType()] = transitions;
            transitionList.Add(from.GetType(), transitions);
        }

        transitions.Add(new Transition(to, predicate));
    }
    public void AddAnyTransition(IState state, Func<bool> predicate)
    {
        anyTransitions.Add(new Transition(state, predicate));
    }
    public void SetState(IState state)
    {
        if (state == currentState)
            return;

        currentState?.OnExit();
        currentState = state;

        transitionList.TryGetValue(currentState.GetType(), out currentTransitions);
        if (currentTransitions == null)
            currentTransitions = emptyTransitions;

        currentState.OnEnter();
    }
    public void Tick()
    {
        var transition = GetTransition();
        if (transition != null)
            SetState(transition.To);

        currentState?.Tick();
    }
    private Transition GetTransition()
    {
        foreach (var transition in anyTransitions)
            if (transition.Condition())
                return transition;

        foreach (var transition in currentTransitions)
            if (transition.Condition())
                return transition;

        return null;
    }
    private class Transition
    {
        public Func<bool> Condition { get;  }
        public IState To { get; }
        public Transition(IState to, Func<bool> condition)
        {
            To = to;
            Condition = condition;
        }
    }

}

