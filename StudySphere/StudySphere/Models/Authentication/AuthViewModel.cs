namespace StudySphere.ViewModels.Authentication
{
    public class AuthViewModel
    {
        public LoginViewModel Login { get; set; } = new LoginViewModel();

        public RegisterViewModel Register { get; set; } = new RegisterViewModel();
    }
}