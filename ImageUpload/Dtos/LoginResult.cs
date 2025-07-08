namespace ImageUpload.Dtos
{
    public sealed class LoginResult<TUserInfo>
    {
        public required string AccessToken { get; set; }
        public required int ExpiresIn { get; set; }
        public required string? RefreshToken { get; set; }
        public required TUserInfo? UserInfo { get; set; }
        public string? ErrorMessage { get; set; }

    }

}
