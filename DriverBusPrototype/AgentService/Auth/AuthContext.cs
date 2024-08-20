﻿namespace DriverBusPrototype.AgentService.Auth;

public class AuthContext
{
    public const string API_KEY_HEADER = "X-API-KEY";

    public string GetApiKey() => CommonHelper.CalculateHash(CommonHelper.RoundToNearestInterval(DateTime.Now, TimeSpan.FromSeconds(5.0)).ToString());

    public bool IsValidApiKey(string apiKey) => apiKey == this.GetApiKey();
}