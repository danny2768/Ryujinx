using Ryujinx.Common.Extensions;
using Ryujinx.HLE.HOS.Services.Account.Acc.Types;
using Ryujinx.HLE.HOS.Services.Friend.ServiceCreator;
using Ryujinx.HLE.HOS.Services.Friend.ServiceCreator.Types;

namespace Ryujinx.HLE.HOS.Services.Friend
{
    [Service("friend:a", FriendServicePermissionLevel.Administrator)]
    [Service("friend:m", FriendServicePermissionLevel.Manager)]
    [Service("friend:s", FriendServicePermissionLevel.System)]
    [Service("friend:u", FriendServicePermissionLevel.User)]
    [Service("friend:v", FriendServicePermissionLevel.Viewer)]
    class IServiceCreator : IpcService
    {
        private readonly FriendServicePermissionLevel _permissionLevel;

#pragma warning disable IDE0060 // Remove unused parameter
        public IServiceCreator(ServiceCtx context, FriendServicePermissionLevel permissionLevel)
        {
            _permissionLevel = permissionLevel;
        }
#pragma warning restore IDE0060

        [CommandCmif(0)]
        // CreateFriendService() -> object<nn::friends::detail::ipc::IFriendService>
        public ResultCode CreateFriendService(ServiceCtx context)
        {
            MakeObject(context, new IFriendService(_permissionLevel));

            return ResultCode.Success;
        }

        [CommandCmif(1)] // 2.0.0+
        // CreateNotificationService(nn::account::Uid userId) -> object<nn::friends::detail::ipc::INotificationService>
        public ResultCode CreateNotificationService(ServiceCtx context)
        {
            UserId userId = context.RequestData.ReadStruct<UserId>();

            if (userId.IsNull)
            {
                return ResultCode.InvalidArgument;
            }

            MakeObject(context, new INotificationService(context, userId, _permissionLevel));

            return ResultCode.Success;
        }

        [CommandCmif(2)] // 4.0.0+
        // CreateDaemonSuspendSessionService() -> object<nn::friends::detail::ipc::IDaemonSuspendSessionService>
        public ResultCode CreateDaemonSuspendSessionService(ServiceCtx context)
        {
            MakeObject(context, new IDaemonSuspendSessionService(_permissionLevel));

            return ResultCode.Success;
        }
    }
}