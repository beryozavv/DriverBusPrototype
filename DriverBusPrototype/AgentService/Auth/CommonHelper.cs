using System.Security.Cryptography;
using System.Text;

namespace DriverBusPrototype.AgentService.Auth;

public class CommonHelper
{
    public static long RoundToNearestInterval(DateTime currentTime, TimeSpan interval) => currentTime.Ticks / interval.Ticks * interval.Ticks;

    public static string CalculateHash(string input)
    {
        using (SHA256 shA256 = SHA256.Create())
        {
            byte[] hash = shA256.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < hash.Length; ++index)
                stringBuilder.Append(hash[index].ToString("x2"));
            return stringBuilder.ToString();
        }
    }
}