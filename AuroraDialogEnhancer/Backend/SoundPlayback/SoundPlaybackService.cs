using System.IO;
using System.Media;

namespace AuroraDialogEnhancer.Backend.SoundPlayback;

public class SoundPlaybackService
{
    public void PlaySound(Stream stream)
    {
        using var soundPlayer = new SoundPlayer(stream);
        soundPlayer.Play();
    }
}
