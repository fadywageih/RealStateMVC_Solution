namespace RealState.DAL.Models
{
    public class ModelBase
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;

    }
}
