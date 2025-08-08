using UnityEngine;


namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "CONFIG/GlobalConfig")]
    public class GlobalConfig : ScriptableObject
    {
        public bool TestMode = false;
    }

}