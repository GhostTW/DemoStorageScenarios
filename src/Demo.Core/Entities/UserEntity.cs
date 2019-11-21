namespace Demo.Core.Entities
{
    public class UserEntity
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
    }
}