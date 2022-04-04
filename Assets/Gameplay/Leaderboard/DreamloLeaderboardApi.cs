using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.Networking;

public class DreamloLeaderboardApi
{
    public enum ApiAction
    {
        Add,
        AddAndFetch,
        Remove,
        Clear,
        Fetch,
    }

    public struct Entry
    {
        public string Username;
        public int Score;
        public int Seconds;
        public string Text;
        public DateTime DateTime;
    }

    private readonly DreamloLeaderboard m_Leaderboard;
    public event Action<IEnumerable<Entry>> OnEntriesReceived;
    public event Action<string, ApiAction> OnError;

    public const string Domain = "dreamlo.com";
    private string UrlScheme => m_Leaderboard.UseSSL ? "https" : "http";
    private string BaseURL => $"{UrlScheme}://{Domain}/lb";
    public string BasePrivateURL => $"{BaseURL}/{m_Leaderboard.PrivateCode}";
    public string BasePublicURL => $"{BaseURL}/{m_Leaderboard.PublicCode}";

    public DreamloLeaderboardApi(DreamloLeaderboard leaderboard)
    {
        m_Leaderboard = leaderboard;
    }

    public IEnumerator Add(string username, int score, int seconds = 0, string text = "")
    {
        var url = $"{BasePrivateURL}/add/{username}/{score}/{seconds}/{text}";
        return SendRequest(url, false, ApiAction.Add);
    }

    public IEnumerator AddAndFetch(string username, int score, int seconds = 0, string text = "")
    {
        var url = $"{BasePrivateURL}/add-pipe/{username}/{score}/{seconds}/{text}";
        return SendRequest(url, true, ApiAction.AddAndFetch);
    }

    public IEnumerator Remove(string username)
    {
        var url = $"{BasePrivateURL}/delete/{username}";
        return SendRequest(url, false, ApiAction.Remove);
    }

    public IEnumerator Clear()
    {
        var url = $"{BasePrivateURL}/clear";
        return SendRequest(url, false, ApiAction.Clear);
    }

    public IEnumerator Fetch()
    {
        var suffix = FetchPathSuffix(m_Leaderboard.SortOrder);
        var url = $"{BasePrivateURL}/pipe{suffix}";
        return SendRequest(url, true, ApiAction.Fetch);
    }

    public IEnumerator Fetch(string username)
    {
        var url = $"{BasePrivateURL}/pipe-get/{username}";
        return SendRequest(url, true, ApiAction.Fetch);
    }

    private string FetchPathSuffix(DreamloLeaderboard.ESortOrder sortOrder)
    {
        switch (sortOrder)
        {
            case DreamloLeaderboard.ESortOrder.LowestScore: return "-asc";
            case DreamloLeaderboard.ESortOrder.HighestTime: return "-seconds";
            case DreamloLeaderboard.ESortOrder.LowestTime: return "-seconds-asc";
            case DreamloLeaderboard.ESortOrder.MostRecent: return "-date";
            case DreamloLeaderboard.ESortOrder.Oldest: return "-date-asc";
            default: return "";
        }
    }

    private IEnumerator SendRequest(string url, bool expectData, ApiAction apiAction)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
            if (expectData)
            {
                var entries = ParseEntires(www.downloadHandler.text);
                OnEntriesReceived?.Invoke(entries);
            }
            else
            {
                var status = www.downloadHandler.text;
                if (status != "OK")
                {
                    OnError?.Invoke(status, apiAction);
                }
            }
        }
    }

    private IEnumerable<Entry> ParseEntires(string data)
    {
        CultureInfo provider = CultureInfo.InvariantCulture;
        var lines = data.Split('\n');
        for (var i = 0; i < lines.Length; ++i)
        {
            var parts = lines[i].Split('|');
            yield return new Entry()
            {
                Username = parts[0],
                Score = int.Parse(parts[1]),
                Seconds = int.Parse(parts[2]),
                Text = parts[3],
                DateTime = DateTime.ParseExact(parts[4], "M/d/yyyy h:m:s tt", provider, DateTimeStyles.AssumeUniversal),
            };
        }
    }
}
