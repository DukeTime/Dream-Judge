namespace DefaultNamespace.DialogueSystem
{
    using System.Collections.Generic;

    public static class PlayerFlags
    {
        private static HashSet<string> flags = new HashSet<string>();

        public static void SetFlag(string flag)
        {
            flags.Add(flag);
        }

        public static bool HasFlag(string flag)
        {
            return flags.Contains(flag);
        }

        public static void ClearFlag(string flag)
        {
            flags.Remove(flag);
        }
    }
}