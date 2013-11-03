/*
 * Copyright 2013 ThirdMotion, Inc.
 *
 *	Licensed under the Apache License, Version 2.0 (the "License");
 *	you may not use this file except in compliance with the License.
 *	You may obtain a copy of the License at
 *
 *		http://www.apache.org/licenses/LICENSE-2.0
 *
 *		Unless required by applicable law or agreed to in writing, software
 *		distributed under the License is distributed on an "AS IS" BASIS,
 *		WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *		See the License for the specific language governing permissions and
 *		limitations under the License.
 */

/**
 * @class strange.extensions.sequencer.impl.Sequencer
 * 
 * @deprecated
 */

using System;
using System.Collections.Generic;
using strange.extensions.dispatcher.api;
using strange.extensions.injector.api;
using strange.extensions.sequencer.api;
using strange.extensions.command.api;
using strange.extensions.command.impl;
using strange.framework.api;
using strange.framework.impl;

namespace strange.extensions.sequencer.impl
{
	public class Sequencer : CommandBinder, ISequencer, ITriggerable
	{

		public Sequencer ()
		{
		}

		override public IBinding GetRawBinding()
		{
			return new SequenceBinding (resolver);
		}
		
		override public void ReactTo(object key, object data)
		{
			ISequenceBinding binding = GetBinding (key) as ISequenceBinding;
			if (binding != null)
			{
				nextInSequence (binding, data, 0);
			}
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

		/// Instantiate and Inject the ISequenceCommand.
		new virtual protected ISequenceCommand createCommand(object cmd, object data)
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
					ISequenceBinding binding = activeSequences [command] as ISequenceBinding;
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

		new public virtual ISequenceBinding Bind<T> ()
		{
			return base.Bind<T> () as ISequenceBinding;
		}

		new public virtual ISequenceBinding Bind (object value)
		{
			return base.Bind (value) as ISequenceBinding;
		}
	}
}

