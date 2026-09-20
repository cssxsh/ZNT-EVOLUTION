using System.Runtime.InteropServices;

namespace ZNT.Evolution.Core.Asset;

public class SoundAsset : FMODAsset
{
    public FMOD.Sound Sound { get; private set; }

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

    private void OnDestroy()
    {
        if (Sound.hasHandle()) Sound.release();
    }
}