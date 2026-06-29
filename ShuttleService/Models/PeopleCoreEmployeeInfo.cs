namespace ShuttleService.Models
{
    public class PeopleCoreEmployeeInfo
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string DepartmentCode { get; set; }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(FirstName) &&
                   string.IsNullOrEmpty(MiddleName) &&
                   string.IsNullOrEmpty(LastName) &&
                   string.IsNullOrEmpty(Email) &&
                   string.IsNullOrEmpty(MobileNo) &&
                   string.IsNullOrEmpty(Position);
        }
    }
}
