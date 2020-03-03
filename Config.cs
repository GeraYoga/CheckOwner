using Rocket.API;

namespace GY.CheckOwner
{
    public class Config : IRocketPluginConfiguration
    {
        public bool UseGesture;
        public string GesturePermission;
        public float Distance;
        public void LoadDefaults()
        {
            UseGesture = true;
            GesturePermission = "gy.point.check";
            Distance = 5f;
        }
    }
}