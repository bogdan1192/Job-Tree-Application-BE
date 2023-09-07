namespace DotNetMinimalAPIDemo.EntityClasses
{
#nullable disable
    public partial class Task
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public Boolean HasChildren { get; set; }
    }

}