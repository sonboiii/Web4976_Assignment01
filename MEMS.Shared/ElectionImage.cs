namespace MEMS.Shared;

public static class ElectionImage
{
    public const int MaxBytes = 2 * 1024 * 1024;
    public const string Requirements = "Choose a JPG, PNG or WebP image up to 2 MB.";

    public static string? ContentType(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length is < 12 or > MaxBytes) return null;
        if (bytes.StartsWith((ReadOnlySpan<byte>)[137, 80, 78, 71, 13, 10, 26, 10])) return "image/png";
        if (bytes.StartsWith((ReadOnlySpan<byte>)[255, 216, 255])) return "image/jpeg";
        if (bytes.StartsWith("RIFF"u8) && bytes.Slice(8, 4).SequenceEqual("WEBP"u8)) return "image/webp";
        return null;
    }
}
