using System.Runtime.InteropServices;

namespace ZNT.Evolution.Core.Asset;

internal class SoundAsset : FMODAsset
{
    public FMOD.Sound Sound { get; private set; }

    public FMOD.Studio.EventDescription Event { get; private set; }

    public void CreateSound(byte[] input)
    {
        if (Sound.hasHandle()) Sound.release();
        Sound.clearHandle();
        var info = new FMOD.CREATESOUNDEXINFO
        {
            cbsize = Marshal.SizeOf(typeof(FMOD.CREATESOUNDEXINFO)),
            length = (uint)input.Length
        };
        var result = FMODUnity.RuntimeManager.LowlevelSystem
            .createSound(input, FMOD.MODE.OPENMEMORY | FMOD.MODE.CREATESAMPLE, ref info, out var sound);
        if (result is not FMOD.RESULT.OK) return;
        Sound = sound;
    }

    public void FetchEvent(string type)
    {
        var programmer = type.ToUpper() switch
        {
            "LOUD" => "event:/Evolution/ProgrammerSound/Loud",
            "NORMAL" => "event:/Evolution/ProgrammerSound/Normal",
            "MENU" => "event:/Evolution/ProgrammerSound/Menu",
            "MUSIC" => "event:/Evolution/ProgrammerSound/Music",
            "UNZOOMED" => "event:/Evolution/ProgrammerSound/Unzoomed",
            _ => "event:/Evolution/ProgrammerSound/Normal"
        };
        var result = FMODUnity.RuntimeManager.StudioSystem.getEvent(programmer, out var description);
        if (result is not FMOD.RESULT.OK) return;
        Event = description;
    }

    private void OnDestroy()
    {
        if (Sound.hasHandle()) Sound.release();
    }
}