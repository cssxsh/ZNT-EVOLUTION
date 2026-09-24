using AssetsTools.NET.Texture;

namespace ZNT.Evolution.Tool;

internal static class Program
{
    private static string GamePath => Environment.GetEnvironmentVariable("ZNTGamePath") ?? ".";

    public static void Main(string[] args)
    {
        using var data = new GameData(GamePath);
        foreach (var texture in data.LoadTexture2D())
        {
            TextureFile.ReadTextureFile(texture.baseField);
        }
    }
}