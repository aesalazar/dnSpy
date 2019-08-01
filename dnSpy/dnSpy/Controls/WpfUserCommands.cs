using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using dnSpy.Contracts.DnSpy.Command;
using dnSpy.Contracts.DnSpy.Controls;

namespace dnSpy.Controls {
	sealed class WpfUserCommands : IWpfUserCommands {

		readonly IEnumerable<IUserCommandMetadata> metadata;

		public WpfUserCommands(Guid guid, IEnumerable<IUserCommandMetadata> metadata) {
			Guid = guid;
			this.metadata = metadata;

			var ucs = ParseMetadata();
			Commands = new ReadOnlyDictionary<string, IWpfUserCommand>(
				ucs.ToDictionary(uc => uc.Name, uc => uc)	
			);
		}

		public Guid Guid { get; }

		public IDictionary<string, IWpfUserCommand> Commands { get; }

		private IEnumerable<IWpfUserCommand> ParseMetadata() {
			return metadata.Select(uc => {
				var modifiers = ModifierKeys.None;
				var keys = new List<Key>();

				foreach (var part in uc.Command.Split(' ')) {
					var p = part.ToLower();

					//get the modifiers
					if (p == "ctrl") modifiers |= ModifierKeys.Control;
					else if (p == "alt") modifiers |= ModifierKeys.Alt;
					else if (p == "shift") modifiers |= ModifierKeys.Shift;
					else if (p == "win") modifiers |= ModifierKeys.Windows;

					//get the keys
					else if (p == "+") keys.AddRange(new[] {Key.Add, Key.OemPlus});
					else if (p == "-") keys.AddRange(new[] {Key.Subtract, Key.OemMinus});
					else if (p == "0") keys.AddRange(new[] {Key.D0, Key.NumPad0});
				}

				return new WpfUserCommand(uc.Name, modifiers, keys);
			});
		}
	}
}
