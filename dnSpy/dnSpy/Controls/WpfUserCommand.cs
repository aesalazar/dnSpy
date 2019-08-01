using System.Collections.Generic;
using System.Windows.Input;
using dnSpy.Contracts.DnSpy.Controls;

namespace dnSpy.Controls {

	sealed class WpfUserCommand : IWpfUserCommand {

		public WpfUserCommand(string name, ModifierKeys modifiers, IEnumerable<Key> keys) {
			Name = name;
			Modifiers = modifiers;
			Keys = keys;
		}

		public string Name { get; }
		public ModifierKeys Modifiers { get; }
		public IEnumerable<Key> Keys { get; }
	}
}
