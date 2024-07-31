namespace MultiSMS.BusinessLogic.DTO
{
    public class ManageUserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Surname { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;

        public string Role { get; set; } = default!;
    }
}
