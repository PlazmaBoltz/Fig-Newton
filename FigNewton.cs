using System;
using Brutal.Numerics;
using Brutal.ImGuiApi;
using StarMap.API;
using KSA;
using System.Security.Cryptography;

namespace Fig_Newton;

[StarMapMod]
public class Mod
{
    public bool ImmediateUnload => false;
    private bool _isInitialized;
    private bool _isDisposed;
    private bool _windowVisible;
    private List<Astronomical> astronomicalList;
    private CelestialSystem celestialSystem;
    private double astMass;
    private double otherMass;

    [StarMapImmediateLoad]
    public void OnImmediateLoad() { }

    [StarMapAllModsLoaded]
    public void OnFullyLoaded()
    {        
        _isInitialized = true;
    }

    [StarMapBeforeGui]
    public void OnBeforeUi(double dt)
    {
        if (!_isInitialized || _isDisposed) return;

        var astronomicalList = KSA.Universe.CurrentSystem.All.GetList();

        foreach (var astronomical in astronomicalList)
        {
            if (astronomical == null) continue;
        }
    }
    [StarMapAfterGui]
    public void OnAfterGui(double dt)
    {
        if (!_isInitialized || _isDisposed) return;
        foreach (var astronomical in astronomicalList)
        {
            if (astronomical == null) continue;
            if (astronomical.HasOrbit())
            {
                foreach (var other in astronomicalList)
                {
                    if (other is Celestial celestial)
                    {
                        var otherMass = celestial.Mass;
                    }
                    else
                    {
                        break;
                    };
                    if (astronomical is Celestial celestial2)
                    {
                        var astMass = celestial2.Mass;
                    }
                    else
                    {
                        break;
                    };
                    if (other == null || other == astronomical) continue;
                    if (other == astronomical) break;
                    if (otherMass >= 0)
                    {
                        var r = astronomical.GetPositionEcl() - other.GetPositionEcl();
                        var distance = astronomical.DistanceTo(other);
                        if (distance > 0)
                        {
                            var forceMagnitude = 0.0000000000667384*(astMass * otherMass)/(distance * distance)*dt;
                            var forceDirection = r.Normalized();
                            var force = forceDirection * forceMagnitude;
                            celestial2.Orbit.Create(celestial2.Orbit.Parent, celestial2.Orbit, celestial2.Orbit.OrbitLineColor, new StateVectors(new SimTime(0)+celestial2.Orbit.StateVectors.StateTime,new double3(0.0,0.0,0.0)+celestial2.Orbit.StateVectors.PositionCci,force/astMass+celestial2.Orbit.StateVectors.VelocityCci, new TrueAnomaly(0) + celestial2.Orbit.StateVectors.TrueAnomaly));
                        }
                    };
                };
            };
        }
    }
}