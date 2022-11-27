using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.ogre.ai
{
    public class StateMachine<T>
    {
        private T owner;
        private State<T> currentState;
        private State<T> previousState;
        private State<T> globalState;

        public State<T> CurrentState
        {
            get { return currentState; }
            set { currentState = value; }
        }
        public State<T> PreviousState
        {
            get { return previousState; }
            set { previousState = value; }
        }
        public State<T> GlobalState
        {
            get { return globalState; }
            set { globalState = value; }
        }

        public StateMachine(T owner) { this.owner = owner; }

        public void Update()
        {
            if (globalState != null)
            {
                globalState.Execute(owner);
            }
            if (currentState != null)
            {
                currentState.Execute(owner);
            }
        }

        public void ChangeState(State<T> newState)
        {
            previousState = currentState;

            currentState.Exit(owner);

            currentState = newState;

            currentState.Enter(owner);
        }

        public bool IsInState(State<T> state)
        {
            return currentState.GetType().Equals(state.GetType());
        }
    }
}
