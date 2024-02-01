using System.Text;
using Models;

namespace Extensions
{
    public static class GUIExtension
    {
        private const string StatusIconTag = "<sprite=\"StatusIcon\"";
        
        public static string GetSpriteStatusIcon(StatsType index)
        {
            return StatusIconTag + $" index={(int)index}>";
        }

        public static string SpaceBetweenWord(string target)
        {
            var result = new StringBuilder(target);
            var spaceAmount = 0;
            for (var i = 1; i < target.Length; i++) // pass index 0
            {
                var c = target[i];
                if (c >= 41 && c <= 90)
                {
                    result.Insert(i + spaceAmount, " ");
                    spaceAmount++;
                }
            }

            return result.ToString();
        }
    }
}