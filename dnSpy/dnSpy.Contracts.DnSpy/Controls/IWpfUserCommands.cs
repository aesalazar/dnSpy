using System;
using System.Collections.Generic;

namespace dnSpy.Contracts.DnSpy.Controls {
	/// <summary>
	/// Collection of user-defined commands associated with a UI Control.
	/// </summary>
	public interface IWpfUserCommands {

		/// <summary>
		/// Unique <see cref="Guid"/> for the UI Control.
		/// </summary>
		Guid Guid { get; }

		/// <summary>
		/// Collection of <see cref="IWpfUserCommand"/>s indexed by <see cref="IWpfUserCommand.Name"/>.
		/// </summary>
		IDictionary<string, IWpfUserCommand> Commands { get; }

	}
}
