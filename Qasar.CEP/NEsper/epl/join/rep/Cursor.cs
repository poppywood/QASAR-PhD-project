using System;

using EventBean = com.espertech.esper.events.EventBean;

namespace com.espertech.esper.epl.join.rep
{
	/// <summary>
	/// This class supplies position information for <see cref="com.espertech.esper.epl.join.exec.LookupInstructionExec"/>
	/// to use for iterating over events for lookup.
	/// </summary>

	public class Cursor
	{
		private readonly EventBean _event;
		private readonly int stream;
		private readonly Node node;

		/// <summary> Supplies current event.</summary>
		/// <returns> event
		/// </returns>
		virtual public EventBean Event
		{
			get
			{
				return _event;
			}
			
		}
		/// <summary> Returns current stream the event belongs to.</summary>
		/// <returns> stream number for event
		/// </returns>
		virtual public int Stream
		{
			get
			{
				return stream;
			}
			
		}
		/// <summary> Returns current result node the event belong to.</summary>
		/// <returns> result node of event
		/// </returns>
		virtual public Node Node
		{
			get
			{
				return node;
			}
			
		}

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="_event">is the current event</param>
        /// <param name="stream">is the current stream</param>
        /// <param name="node">is the node containing the set of events to which the event belongs to</param>
		public Cursor(EventBean _event, int stream, Node node)
		{
			this._event = _event;
			this.stream = stream;
			this.node = node;
		}
	}
}
