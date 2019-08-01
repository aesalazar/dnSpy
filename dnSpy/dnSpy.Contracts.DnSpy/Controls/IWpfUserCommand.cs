using System.Collections.Generic;
using System.Windows.Input;

namespace dnSpy.Contracts.DnSpy.Controls {

	/// <summary>
	/// UI Command parameters based on user configuration.
	/// </summary>
	public interface IWpfUserCommand {

		/// <summary>
		/// User-friendly command name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// <see cref="ModifierKeys"/> that should be invoked when triggered.
		/// </summary>
		ModifierKeys Modifiers { get; }

		/// <summary>
		/// Collection of <see cref="Key"/>s that should be invoked when triggered.
		/// </summary>
		IEnumerable<Key> Keys { get; }
	}
}
