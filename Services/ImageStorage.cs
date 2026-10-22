namespace Web4976_Assignment01.Services;

public class ImageStorage(IWebHostEnvironment env)
{
    public const int MaxBytes = 2 * 1024 * 1024;

    public async Task<string?> SaveAsync(IFormFile? image)
    {
        if (image == null || image.Length == 0)
            return null;

        if (image.Length > MaxBytes)
            throw new ArgumentException("Choose an image smaller than 2 MB.");

        var ext = Path.GetExtension(image.FileName).ToLowerInvariant();

        if (ext is not (".jpg" or ".jpeg" or ".png" or ".webp"))
            throw new ArgumentException("Use a JPG, PNG, or WebP image.");

        await using var input = image.OpenReadStream();
        var header = new byte[12];
        var read = await input.ReadAtLeastAsync(header, 12, false);
        var valid = read >= 12 && (ext switch
        {
            ".jpg" or ".jpeg" => header[0] == 0xff && header[1] == 0xd8 && header[2] == 0xff,
            ".png" => header.AsSpan(0, 8).SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }),
            ".webp" => System.Text.Encoding.ASCII.GetString(header, 0, 4) == "RIFF" && System.Text.Encoding.ASCII.GetString(header, 8, 4) == "WEBP",
            _ => false
        });

        if (!valid)
            throw new ArgumentException("The file contents do not match the selected image format.");

        var directory = Path.Combine(env.WebRootPath, "uploads");
        Directory.CreateDirectory(directory);
        var name = Guid.NewGuid().ToString("N") + ext;
        await using var output = File.Create(Path.Combine(directory, name));
        await output.WriteAsync(header.AsMemory(0, read));
        await input.CopyToAsync(output);

        return name;
    }

    public void Delete(string? name)
    {
        if (name == null || name.StartsWith("seed-") || name != Path.GetFileName(name))
            return;
        var path = Path.Combine(env.WebRootPath, "uploads", name);

        if (File.Exists(path))
            File.Delete(path);
    }
}
