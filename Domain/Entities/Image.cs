namespace Domain.Entities;

public class Image
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string FileExtenstions { get; set; } = string.Empty;

    public ImageType Imagetype { get; set; }
    public string TypeId { get; set; } = string.Empty;
    public enum ImageType
    {
        User,
        Resturant,
        Food
    }
    //public string? UserId { get; set; } = string.Empty;
    //public ApplicataionUser? User { get; set; } = default!;


    ////public string? ResturantId { get; set; } = string.Empty;
    ////public Resturant? Resturant { get; set; } = default!;


    ////public string? FoodId { get; set; } = string.Empty;
    ////public Food? Food { get; set; } = default!;
}
