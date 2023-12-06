namespace Kalkatos.Firecard.Core
{
    public class Player
    {
        public int Index;
        public string Role;
        public string Team;

        internal string id;

        public Player (PlayerData data)
        {
            Index = data.Index;
            Role = data.Role;
            Team = data.Team;
        }
    }
}