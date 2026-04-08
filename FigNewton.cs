using System;
using Brutal.Numerics;
using Brutal.ImGuiApi;
using StarMap.API;

namespace Fig_Newton;

[StarMapMod]
public class Mod
{
    public bool ImmediateUnload => false;

    private bool _isInitialized;
    private bool _isDisposed;
    private bool _windowVisible;
    private FNSubmod _submod = null!;

    [StarMapImmediateLoad]
    public void OnImmediateLoad() { }

    [StarMapAllModsLoaded]
    public void OnFullyLoaded()
    {
        try
        {
            _submod = new FNSubmod();
            Patcher.Patch();
            _submod.Initialize();
            _isInitialized = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"fig-newton: Error during initialization: {ex.Message}");
        }
    }

    [StarMapBeforeGui]
    public void OnBeforeUi(double dt)
    {
        if (!_isInitialized || _isDisposed) return;
        _submod.Update(dt);

        var astronomicalList = celestialSystem.All.GetList();

        for (astronomicalList.MoveToStart(); astronomicalList.IsValid(); astronomicalList.MoveNext())
        {
            var astronomical = astronomicalList.Current();
            if (astronomical == null) continue;
            if (astronomical.HasOrbit())
            {
                astronomical.StateVectors = astronomical.StateVectors;
            };
            _submod.UpdateAstronomical(astronomical);
        }
    }

    [StarMapOnFrame]
    public void OnFrame(double dt)
    {
        if (!_isInitialized || _isDisposed) return;
        for (astronomicalList.MoveToStart(); astronomicalList.IsValid(); astronomicalList.MoveNext())
        {
            var astronomical = astronomicalList.Current();
            if (astronomical == null) continue;
            if (astronomical.HasOrbit())
            {
                for (astronomicalList.MoveToStart(); astronomicalList.IsValid(); astronomicalList.MoveNext())
                {
                    var other = astronomicalList.Current();
                    if (other == null || other == astronomical) continue;
                    if (other.Mass >= 0)
                    {
                        var r = (astronomical.x, astronomical.y, astronomical.z) - (other.x, other.y, other.z);
                        var distance = r.Magnitude();
                        if (distance > 0)
                        {
                            var forceMagnitude = 0.0000000000337384*(astronomical.Mass * other.Mass)/(distance * distance)*dt;
                            var forceDirection = r.Normalized();
                            var force = forceDirection * forceMagnitude;
                            astronomical.StateVectors += force / astronomical.Mass;
                        }
                    };
                };
            };
        } 
        _submod.Update(dt);
    }

}