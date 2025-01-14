namespace RoleBasedUserManagementApi.Models
{
    public class ChangeRole
    {
        public string UserEmail { get; set; } = string.Empty;
        public string NewRole { get; set; } = string.Empty;

    }
}
