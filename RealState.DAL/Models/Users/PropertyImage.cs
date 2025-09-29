namespace RealState.DAL.Models.Users
{
    public class PropertyImage:ModelBase
    {
        public string Url { get; set; } = null!;
        public int PropertyId { get; set; }
        public Property Property { get; set; } = null!;
    }
}
