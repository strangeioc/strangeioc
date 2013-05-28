using System;
using System.Collections.Generic;
using babel.extensions.dispatcher.api;
using babel.extensions.injector.api;
using babel.extensions.sequencer.api;
using babel.extensions.command.api;
using babel.framework.api;
using babel.framework.impl;

namespace babel.extensions.sequencer.impl
{
	public class Sequencer : Binder, ISequencer, ITriggerable
	{
		[Inject]
		public IInjectionBinder injectionBinder{ get; set;}

		protected Dictionary<ISequenceCommand, ISequenceBinding> activeSequences = new Dictionary<ISequenceCommand, ISequenceBinding> ();

		public Sequencer ()
		{
		}

		override public IBinding GetRawBinding()
		{
			return new SequenceBinding (resolver);
		}

		public void ReactTo(object key)
		{
			ReactTo(key, null);
		}
		
		public void ReactTo(object key, object data)
		{
			ISequenceBinding binding = GetBinding (key) as ISequenceBinding;
			if (binding != null)
			{
				nextInSequence (binding, data, 0);
			}
		}

		public void Stop(object key)
		{
			ISequenceBinding binding = GetBinding (key) as ISequenceBinding;
			if (binding != null)
			{
				if (activeSequences.ContainsValue (binding))
				{
					foreach(KeyValuePair<ISequenceCommand, ISequenceBinding> sequence in activeSequences)
					{
						if (sequence.Value == binding)
						{
							ISequenceCommand command = sequence.Key;
							removeSequence (command);
						}
					}
				}
			}
		}

		public void BreakSequence (ISequenceCommand command)
		{
			removeSequence (command);
		}

		private void removeSequence(ISequenceCommand command)
		{
			if (activeSequences.ContainsKey (command))
			{
				command.Cancel();
				activeSequences.Remove (command);
			}
		}

		private void invokeCommand(Type cmd, ISequenceBinding binding, object data, int depth)
		{
			ISequenceCommand command = createCommand (cmd, data);
			command.sequenceId = depth;
			trackCommand (command, binding);
			executeCommand (command);
			ReleaseCommand (command);
		}

		virtual protected ISequenceCommand createCommand(object cmd, object data)
		{
			injectionBinder.Bind<ISequenceCommand> ().To (cmd);
			ISequenceCommand command = injectionBinder.GetInstance<ISequenceCommand> () as ISequenceCommand;
			command.data = data;
			injectionBinder.Unbind<ISequenceCommand> ();
			return command;
		}

		private void trackCommand (ISequenceCommand command, ISequenceBinding binding)
		{
			activeSequences [command] = binding;
		}

		private void executeCommand(ISequenceCommand command)
		{
			if (command == null)
			{
				return;
			}
			command.Execute ();
		}

		public void ReleaseCommand (ISequenceCommand command)
		{
			if (command.retain == false)
			{
				if (activeSequences.ContainsKey(command))
				{
					ISequenceBinding binding = activeSequences [command];
					object data = command.data;
					activeSequences.Remove (command);
					nextInSequence (binding, data, command.sequenceId + 1);
				}
			}
		}

		private void nextInSequence(ISequenceBinding binding, object data, int depth)
		{
			object[] values = binding.value as object[];
			if (depth < values.Length)
			{
				Type cmd = values [depth] as Type;
				invokeCommand (cmd, binding, data, depth);
			}
			else
			{
				if (binding.isOneOff)
				{
					Unbind (binding);
				}
			}
		}

		private void failIf(bool condition, string message, SequencerExceptionType type)
		{
			if (condition)
			{
				throw new SequencerException(message, type);
			}
		}

		public void Trigger<T>(object data)
		{
			Trigger (typeof(T), data);
		}

		public void Trigger(object key, object data)
		{
			ReactTo(key, data);
		}
	}
}

