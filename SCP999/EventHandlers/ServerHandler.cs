using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.CustomRoles.API;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using System;
using Exiled.Events.EventArgs.Scp096;
using Exiled.Events.EventArgs.Warhead;

namespace SCP999;
public class ServerHandler
{
    /// <summary>
    /// Does not allow SCP-999 to turn off the warhead
    /// </summary>
    public void OnWarheadStop(StoppingEventArgs ev)
    {
        if (CustomRole.Get(typeof(Scp999Role)).Check(ev.Player))
        {
            ev.IsAllowed = false;
        }
    }
    
    /// <summary>
    /// Does not allow SCP-999 to enrage SCP-096
    /// </summary>
    public void OnScpEnraging(EnragingEventArgs ev)
    {
        if (CustomRole.Get(typeof(Scp999Role)).Check(ev.Player))
        {
            ev.IsAllowed = false;
        }
    }
    
    /// <summary>
    /// Does not add SCP-999 for SCP-096 to targets
    /// </summary>
    public void OnAddingTarget(AddingTargetEventArgs ev)
    {
        if (CustomRole.Get(typeof(Scp999Role)).Check(ev.Player))
        {
            ev.IsAllowed = false;
        }
    }
    
    /// <summary>
    /// If the SCP-999 dies, then his original body should not appear
    /// </summary>
    public void OnSpawningRagdoll(SpawningRagdollEventArgs ev)
    {
        if (CustomRole.Get(typeof(Scp999Role)).Check(ev.Player))
        {
            ev.IsAllowed = false;
        }
    }
    
    /// <summary>
    /// Does not allow SCP-106 to teleport SCP-999 to a pocket dimension
    /// </summary>
    public void OnEnteringPocketDimension(EnteringPocketDimensionEventArgs ev)
    {
        if (CustomRole.Get(typeof(Scp999Role)).Check(ev.Player))
        {
            ev.IsAllowed = false;
        }
    }
}