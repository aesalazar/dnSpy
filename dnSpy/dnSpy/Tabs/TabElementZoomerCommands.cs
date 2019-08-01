using dnSpy.Contracts.Controls;
using dnSpy.Contracts.DnSpy.Command;

namespace dnSpy.Tabs {

	[ExportUserCommand(ControlConstants.TABELEMENTZOOMER_COMMAND_INCREASE, ControlConstants.TABELEMENTZOOMER_GUID_STRING, "Ctrl +")]
	internal sealed class TabZoomIncreaseUserCommands : IUserCommand {
	}

	[ExportUserCommand(ControlConstants.TABELEMENTZOOMER_COMMAND_DECREASE, ControlConstants.TABELEMENTZOOMER_GUID_STRING, "Ctrl -")]
	internal sealed class TabZoomDecreaseUserCommands : IUserCommand {
	}

	[ExportUserCommand(ControlConstants.TABELEMENTZOOMER_COMMAND_RESET, ControlConstants.TABELEMENTZOOMER_GUID_STRING, "Ctrl 0")]
	internal sealed class TabZoomResetUserCommands : IUserCommand {
	}
}
