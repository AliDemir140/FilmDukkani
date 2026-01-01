namespace Application.Constants
{
    public static class RoleNames
    {
        public const string User = "User";
        public const string Admin = "Admin";

        public const string Accounting = "Accounting";
        public const string Warehouse = "Warehouse";
        public const string Purchasing = "Purchasing";

        public static readonly string[] EmployeeRoles = { Admin, Accounting, Warehouse, Purchasing };
    }
}
