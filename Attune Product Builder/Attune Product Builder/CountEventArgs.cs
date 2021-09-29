using System;

namespace Attune.Kernel.Build
{
	/// <summary>
	/// Provides data for the DatabaseProcessed and TableProcessed events.
	/// </summary>
	public sealed class CountEventArgs : EventArgs
	{
		private int count;

		/// <summary>
		/// Initializes a new instance of the CountEventArgs class.
		/// </summary>
		/// <param name="count">The count to be reported.</param>
		public CountEventArgs(int count)
		{
			this.count = count;
		}

		/// <summary>
		/// Gets the count.
		/// </summary>
		public int Count
		{
			get { return count; }
		}
	}
}
