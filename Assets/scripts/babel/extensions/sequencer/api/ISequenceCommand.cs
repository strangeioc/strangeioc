using System;
using babel.extensions.command.api;

namespace babel.extensions.sequencer.api
{
	public interface ISequenceCommand
	{
		void Execute();
		void Retain();
		void Release();
		void Dispose();
		void Cancel();
		void BreakSequence();

		bool retain{ get; }
		bool cancelled{ get; }
		int sequenceId{ get; set; }
		object data{ get; set; }

	}
}

