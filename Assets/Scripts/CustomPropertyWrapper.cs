using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public static class CustomPropertyWrapper
{
    public static int incrementProperty(Player player, string key, int value)
    {
        if (player.CustomProperties[key] == null)
        {
            return -1;
        }

        int newValue = value + (int)player.CustomProperties[key];
        player.SetCustomProperties(new Hashtable() { { key, newValue } });
        return 1;
    }

    public static int decrementProperty(Player player, string key, int value)
    {
        if (player.CustomProperties[key] == null)
        {
            return -1;
        }

        int newValue = (int)player.CustomProperties[key] - value;
        player.SetCustomProperties(new Hashtable() { { key, newValue } });
        return 1;
    }

    public static int setPropertyInt(Player player, string key, int value)
    {
        if (player.CustomProperties[key] == null)
        {
            return -1;
        }
        player.SetCustomProperties(new Hashtable() { { key, value } });
        return 1;
    }
    public static int setPropertyBool(Player player, string key, bool value)
    {
        if (player.CustomProperties[key] == null)
        {
            return -1;
        }

        player.SetCustomProperties(new Hashtable() { { key, value } });
        return 1;
    }

    public static int setPropertyString(Player player, string key, string value)
    {
        if (player.CustomProperties[key] == null)
        {
            return -1;
        }

        player.SetCustomProperties(new Hashtable() { { key, value } });
        return 1;
    }
}
