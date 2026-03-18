//using Domain.Abstractions;
//using Domain.Abstractions.Entities;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Domain.Abstractions;
//using Domain.Abstractions.Entities;
//using Microsoft.AspNetCore.Identity; // <-- THÊM THƯ VIỆN NÀY
//using System;
//using System.Collections.Generic;


//namespace Domain.Entities
//{
//    // 1. Chuyển sang kế thừa IdentityUser<long>
//    // 2. Kế thừa trực tiếp các interface Audit để giữ lại tính năng Tracking/SoftDelete
//    public class User : IdentityUser<long>, IAuditable, ISoftDelete, IDateTracking, IUserTracking
//    {
//        // QUAN TRỌNG: Đã xóa Username, Email, PhoneNumber, PasswordHash 
//        // vì chúng đã có sẵn bên trong IdentityUser<long> rồi!

//        // Các thuộc tính riêng của bạn giữ nguyên:
//        public string FullName { get; set; }
//        public DateTime? DateOfBirth { get; set; }
//        public bool IsActive { get; set; } = true;

//        // Navigation properties giữ nguyên:
//        public virtual ICollection<Customer> Customers { get; set; }
//        public virtual ICollection<Feedback> Feedbacks { get; set; }
//        public virtual ICollection<Coupon> Coupons { get; set; }

//        // Bổ sung lại các trường Tracking/SoftDelete (Vì chúng ta đã bỏ EntityAuditSoftDeleteBase)
//        public DateTime CreatedDate { get; set; }
//        public DateTime? ModifiedDate { get; set; }
//        public string CreatedBy { get; set; }
//        public string ModifiedBy { get; set; }
//        public bool IsDeleted { get; set; }
//        public DateTime? DeletedAt { get; set; }
//    }
//}
using Domain.Abstractions;
using Domain.Abstractions.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class User : IdentityUser<long>, IEntityBase<long>, IAuditable, ISoftDelete, IDateTracking, IUserTracking
    {

        public string Username
        {
            get => base.UserName;
            set => base.UserName = value;
        }
        public string FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual ICollection<Customer> Customers { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<Coupon> Coupons { get; set; }

        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        public bool IsDeleted { get; set; }
        public DateTimeOffset DeletedAt { get; set; }

        public void MarkAsDeleted()
        {
            IsDeleted = true;
            DeletedAt = DateTimeOffset.UtcNow;
        }
    }
}