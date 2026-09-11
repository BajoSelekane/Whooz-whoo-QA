namespace Whooz_whoo.Domain.Entities
{
    // Minimal DTOs to resolve missing-type errors. Move to proper files/namespaces as needed.
    public class ViewerInfo
    {
        public Guid UserId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
