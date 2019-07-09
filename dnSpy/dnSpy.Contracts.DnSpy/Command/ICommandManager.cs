using System;
using System.Collections.Generic;
using System.Windows.Input;
using dnlib.DotNet;

namespace dnSpy.Contracts.Command {

	/// <summary>
	/// Manages and provides <see cref="KeyGesture"/>s associated registered for the application.
	/// </summary>
	public interface ICommandManager {

		/// <summary>
		/// Retrieves the associated collection of <see cref="KeyInput"/> collections registered
		/// to the <paramref name="guid"/> if available.
		/// </summary>
		/// <param name="guid">GUID associated with the collection of inputs.</param>
		/// <param name="inputDictionary">Collection of key inputs indexed by action name.</param>
		/// <returns>Indication if the the GUID was found in the manager registry.</returns>
		bool TryGetKeyInputs(Guid guid, out IDictionary<string, IList<KeyInput>> inputDictionary);

		/// <summary>
		/// Adds a collection of key inputs to be persisted in the application settings.
		/// </summary>
		/// <param name="guid">GUID associated with the collection of inputs.</param>
		/// <param name="name">Readable name of the section.</param>
		/// <param name="inputDictionary">Collection of key inputs indexed by action name.</param>
		/// <exception cref="InvalidKeyException"><paramref name="guid"/> is already present.</exception>
		void AddKeyInputs(Guid guid, string name, IDictionary<string, IList<KeyInput>> inputDictionary);
	}
}
