using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using dnlib.DotNet;
using dnSpy.Contracts.Command;
using dnSpy.Contracts.Settings;

namespace dnSpy.Commands {
	[Export, Export(typeof(ICommandManager))]
	internal sealed class CommandManager : ICommandManager {

        [ImportingConstructor]
        CommandManager(ISettingsService settingsService) {

	        _settingsService = settingsService;
	        _settingsSection = _settingsService.GetOrCreateSection(SETTINGS_GUID);
	        LoadKeyInputs();
        }

		#region Fields

		/// <summary>
		/// GUID used when persisting application settings.
		/// </summary>
		static readonly Guid SETTINGS_GUID = new Guid("DC94d98e-56AB-4387-b736-7bf239841937");

		/// <summary>
		/// Collection of Named <see cref="KeyInput"/> indexed by registering control <see cref="Guid"/>. 
		/// </summary>
		readonly IDictionary<Guid, IDictionary<string, IList<KeyInput>>> _keyInputs = new Dictionary<Guid, IDictionary<string, IList<KeyInput>>>();

        /// <summary>
		/// Settings service to store keys.
		/// </summary>
		readonly ISettingsService _settingsService;

        /// <summary>
		/// Settings section to store keys.
		/// </summary>
        readonly ISettingsSection _settingsSection;

		#endregion

		#region Methods

		/// <summary>
		/// Loads the <see cref="_settingsSection"/> to <see cref="_keyInputs"/>.
		/// </summary>
		void LoadKeyInputs() {

			//Load each control section
			foreach (var controlSec in _settingsSection.Sections) {

				//Init the control section
				var control = new Dictionary<string, IList<KeyInput>>();
				_keyInputs[Guid.Parse(controlSec.Name)] = control;

				//Load each command section
				foreach (var commandSec in controlSec.Sections) {

					//Create the keys and modifer
					var keys = commandSec.Sections.Select(s => {
						Enum.TryParse<Key>(s.Name, true, out var key);
						return new KeyInput(key, s.Attribute<ModifierKeys>(nameof(KeyInput.Modifiers)));
					}).ToList();

					control[commandSec.Name] = keys;
				}
			}
		}

		/// <summary>
		/// Retrieves the associated collection of <see cref="KeyInput"/> collections registered
		/// to the <paramref name="guid"/> if available.
		/// </summary>
		/// <param name="guid">GUID associated with the collection of inputs.</param>
		/// <param name="inputDictionary">Collection of key inputs indexed by action name.</param>
		/// <returns>Indication if the the GUID was found in the manager registry.</returns>
		public bool TryGetKeyInputs(Guid guid, out IDictionary<string, IList<KeyInput>> inputDictionary) {
			var found = _keyInputs.ContainsKey(guid);
			inputDictionary = found ? _keyInputs[guid] : new Dictionary<string, IList<KeyInput>>();
			return found;
		}

		/// <summary>
		/// Adds a collection of key inputs to be persisted in the application settings.
		/// </summary>
		/// <param name="guid">GUID associated with the collection of inputs.</param>
		/// <param name="name">Readable name of the section.</param>
		/// <param name="inputDictionary">Collection of key inputs indexed by action name.</param>
		/// <exception cref="InvalidKeyException"><paramref name="guid"/> is already present.</exception>
		public void AddKeyInputs(Guid guid, string name, IDictionary<string, IList<KeyInput>> inputDictionary) {
			if (_keyInputs.ContainsKey(guid))
				throw new InvalidKeyException($"Guid '{guid}' is already present.");

			_keyInputs[guid] = inputDictionary;

			//Create a main section for the calling control
			var parent =_settingsSection.CreateSection(guid.ToString());
			parent.Attribute(nameof(name), name);

			//Store each action name
			foreach (var kvp in inputDictionary) {
				var section = parent.CreateSection(kvp.Key);

				//Store each key binding
				foreach (var inKvp in kvp.Value) {
					var inSection = section.CreateSection(inKvp.Key.ToString());
					inSection.Attribute(nameof(inKvp.Modifiers), inKvp.Modifiers);
				}
			}
		}
		
		#endregion

	}
}
