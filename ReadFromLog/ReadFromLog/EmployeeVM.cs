using System.Collections.Generic;

namespace ReadFromLog
{
    public class EmployeeVM
    {
        public string EmployeeName { get; set; }
        public bool IsInEmployee { get; set; }
        public int CurrentTimeStamp { get; set; }
        public List<EmployeeInOutVM> EmployeeInOutVM { get; set; }
    }
}
