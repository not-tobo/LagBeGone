using Il2CppSystem.Collections.Generic;
using PlayerExtension;
using VRC;

namespace PlayerManagerExtension
{
    public static class PlayerManagerExtension
    {
        public static List<VRC.Player> AllPlayers(this PlayerManager Instance)
        {
            return Instance.field_Private_List_1_Player_0;
        }

        public static VRC.Player GetPlayer(this PlayerManager Instance, int playerID)
        {
            List<VRC.Player> list = Instance.AllPlayers();
            foreach (VRC.Player player in list.ToArray())
            {
                if (player.GetVRCPlayerApi().playerId == playerID)
                {
                    return player;
                }
            }
            return null;
        }

        public static PlayerManager PlayerManager
        {
            get
            {
                return PlayerManager.field_Private_Static_PlayerManager_0;
            }
        }
    }
}