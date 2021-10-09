using VRC.Core;
using VRC.SDKBase;

namespace PlayerExtension
{
    public static class PlayerExtension
    {
        public static APIUser GetAPIUser(this VRC.Player Instance) =>
            Instance.prop_APIUser_0;

        public static VRCPlayerApi GetVRCPlayerApi(this VRC.Player Instance) =>
            Instance.prop_VRCPlayerApi_0;

        public static VRCAvatarManager GetAvatarManager(this VRCPlayer Instance) =>
            Instance.prop_VRCAvatarManager_0;

        public static string GetUserID(this APIUser Instance) =>
            Instance.id;
    }
}