using System;

namespace StateMachine
{
	public class Machine<T> where T : struct, IConvertible
	{
		protected T _currentState;

		public bool isStateActive = false;

		public Machine()
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException("T must be an enumeration");
			}
		}

		//Does not allow transition to the same state it's already in
		public virtual void SetState(T input)
		{
			if(_currentState.GetHashCode() != input.GetHashCode())
			{
				Exit(_currentState);
				_currentState = input;
				Enter(_currentState);
			}
		}

		//Fires exit and enter ALWAYS, even from the same state
		public virtual void ForceState(T input)
		{
			Exit(_currentState);
			_currentState = input;
			Enter(_currentState);
		}

		public virtual T GetState()
		{
			return _currentState;
		}

		public virtual void Enter(T whatState)
		{

		}

		public virtual void Exit(T whatState)
		{
			
		}
	}
}