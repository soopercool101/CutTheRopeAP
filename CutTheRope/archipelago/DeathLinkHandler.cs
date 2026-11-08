using System;
using System.Collections.Generic;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using CutTheRope.iframework;
using CutTheRope.windows;

namespace CutTheRope.archipelago;

public class DeathLinkHandler
{
    private static bool deathLinkEnabled;
    private string slotName;
    private readonly DeathLinkService service;
    private readonly Queue<DeathLink> deathLinks = new();

    /// <summary>
    /// instantiates our death link handler, sets up the hook for receiving death links, and enables death link if needed
    /// </summary>
    /// <param name="deathLinkService">The new DeathLinkService that our handler will use to send and
    /// receive death links</param>
    /// <param name="enableDeathLink">Whether we should enable death link or not on startup</param>
    public DeathLinkHandler(DeathLinkService deathLinkService, string name, bool enableDeathLink = false)
    {
        service = deathLinkService;
        service.OnDeathLinkReceived += DeathLinkReceived;
        slotName = name;
        deathLinkEnabled = enableDeathLink;

        if (deathLinkEnabled)
        {
            service.EnableDeathLink();
            FrameworkTypes._LOG("DeathLink enabled");
        }
        else
        {
            FrameworkTypes._LOG("DeathLink disabled");
        }
    }

    /// <summary>
    /// enables/disables death link
    /// </summary>
    public void ToggleDeathLink()
    {
        deathLinkEnabled = !deathLinkEnabled;

        if (deathLinkEnabled)
        {
            service.EnableDeathLink();
        }
        else
        {
            service.DisableDeathLink();
        }
    }

    /// <summary>
    /// what happens when we receive a deathLink
    /// </summary>
    /// <param name="deathLink">Received Death Link object to handle</param>
    private void DeathLinkReceived(DeathLink deathLink)
    {
        //if (!GameplayMaster.instance)
        //    return;
        deathLinks.Enqueue(deathLink);
        FrameworkTypes._LOG(string.IsNullOrWhiteSpace(deathLink.Cause)
            ? $"Received Death Link from: {deathLink.Source}"
            : deathLink.Cause);
    }

    /// <summary>
    /// can be called when in a valid state to kill the player, dequeueing and immediately killing the player with a
    /// message if we have a death link in the queue
    /// </summary>
    public void KillPlayer()
    {
        try
        {
            if (deathLinks.Count < 1) return;

            var deathLink = deathLinks.Dequeue();
            var cause = string.IsNullOrWhiteSpace(deathLink.Cause)
                ? GetGenericDeathLinkCause(deathLink)
                : deathLink.Cause;

            if (!cause.Contains(deathLink.Source))
            {
                FrameworkTypes._LOG($"{GetGenericDeathLinkCause(deathLink)} ({cause})");
                cause = $"{GetGenericDeathLinkCause(deathLink)}\n\n{cause}";
            }
            else
            {
                FrameworkTypes._LOG(cause);
            }

            // TODO: Kill player
        }
        catch (Exception e)
        {
            FrameworkTypes._LOG(e.Message);
        }
    }

    /// <summary>
    /// returns message for the player to see when a death link is received without a cause
    /// </summary>
    /// <param name="deathLink">death link object to get relevant info from</param>
    /// <returns></returns>
    private string GetGenericDeathLinkCause(DeathLink deathLink)
    {
        return $"Received death from {deathLink.Source}";
    }

    public static int DeathLinkCount = 0;
    public static int DeathLinkAmnesty = 0;
    /// <summary>
    /// called to send a death link to the multiworld
    /// </summary>
    public void SendDeathLink()
    {
        try
        {
            if (!deathLinkEnabled) return;

            // DeathLinkCount is incremented earlier in Die() for text purposes
            if (DeathLinkCount >= DeathLinkAmnesty)
            {
                DeathLinkCount = 0;
                FrameworkTypes._LOG("Sending Death Link...");

                var linkToSend = new DeathLink(slotName, $"{Global.ApClient.Player} timed out!");

                service.SendDeathLink(linkToSend);
            }
        }
        catch (Exception e)
        {
            FrameworkTypes._LOG(e.Message);
        }
    }
}