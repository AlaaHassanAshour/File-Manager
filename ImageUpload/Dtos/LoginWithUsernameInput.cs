namespace ImageUpload.Dtos
{
    public sealed class LoginWithUsernameInput
    {
        public required string Username { get; set; }

        public required string Password { get; set; }
    }
}
