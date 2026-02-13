using Domain.Common;

namespace Domain.Entities
{
    public class Department : BaseEntity
    {
        public string Name { get; set; } // Örn: Yazılım, İK, Muhasebe
        public string Description { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }
}