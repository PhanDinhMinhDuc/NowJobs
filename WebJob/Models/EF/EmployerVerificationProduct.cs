using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using WebJob.Models.EF;
namespace WebJob.Models.EF
{
    [Table("tb_EmployerVerificationProduct")]
    public class EmployerVerificationProduct
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int EmployerVerificationId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.Now;
        public string TransactionId { get; set; } // ID giao dịch thanh toán
        public int PaymentMethod { get; set; } // 1: COD, 2: VNPay, 3: Khác

        [ForeignKey("EmployerVerificationId")]
        public virtual EmployerVerification EmployerVerification { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
