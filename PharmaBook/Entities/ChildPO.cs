namespace PharmaBook.Entities
{
    public class ChildPO
    {
        public int Id { get; set; }
        public int ProdID { get; set; }
        public int masterPOid { get; set; }
        public int Qty { get; set; }
        public string Remarks { get; set; }
    }
}
