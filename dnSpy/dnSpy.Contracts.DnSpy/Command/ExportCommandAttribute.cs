using System;
using System.ComponentModel.Composition;

namespace dnSpy.Contracts.DnSpy.Command {

	/// <summary>
	/// UI Control Command configurable by the user.
	/// </summary>
	public interface IUserCommand {
	}

	/// <summary>
	/// <see cref="IUserCommand"/> attribute metadata.
	/// </summary>
	public interface IUserCommandMetadata {

		/// <summary>
		/// User-friendly command name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Unique UI Control GUID string.
		/// </summary>
		string ControlGuid { get; }

		/// <summary>
		/// Key and optional modifiers to invoke the command.
		/// </summary>
		string Command { get; }
	}

	/// <summary>
	/// Specifies the needed metadata associated with a <see cref="IUserCommand"/>.
	/// </summary>
	[MetadataAttribute, AttributeUsage(AttributeTargets.Class)]
	public sealed class ExportUserCommandAttribute : ExportAttribute, IUserCommandMetadata {
		/// <summary>
		/// Creates a new instance of <see cref="ExportUserCommandAttribute"/>.
		/// </summary>
		/// <param name="commandName">User-friendly command name.</param>
		/// <param name="controlGuid">Unique UI Control GUID string.</param>
		/// <param name="command">Key and optional modifiers to invoke the command.</param>
		public ExportUserCommandAttribute(string commandName, string controlGuid, string command)
			: base(typeof(IUserCommand)) {
			Name = commandName;
			ControlGuid = controlGuid;
			Command = command;
		}

		/// <summary>
		/// User-friendly command name.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Unique UI Control GUID string.
		/// </summary>
		public string ControlGuid { get; }

		/// <summary>
		/// Key and optional modifiers to invoke the command.
		/// </summary>
		public string Command { get; }
	}
}
