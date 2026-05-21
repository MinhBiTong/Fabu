using Application.DTOs.Responses.Search;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Entities;
using CustomerServiceEntity = Domain.Entities.CustomerService;
using ServiceEntity = Domain.Entities.Service;

namespace Application.Services.Search;

public sealed class SearchDocumentMapper : ISearchDocumentMapper
{
    public bool CanMap(object entity)
        => entity is User or Customer or Account or RechargePlan or ServiceEntity or Transaction or Payment
            or PostpaidBill or Coupon or Feedback or AuditLog or CustomerServiceEntity;

    public string? GetDocumentId(object entity)
    {
        var mapped = Map(entity);
        return mapped?.Id;
    }

    public SearchDocument? Map(object entity)
    {
        return entity switch
        {
            User user => MapUser(user),
            Customer customer => MapCustomer(customer),
            Account account => MapAccount(account),
            RechargePlan plan => MapRechargePlan(plan),
            ServiceEntity service => MapService(service),
            Transaction transaction => MapTransaction(transaction),
            Payment payment => MapPayment(payment),
            PostpaidBill bill => MapPostpaidBill(bill),
            Coupon coupon => MapCoupon(coupon),
            Feedback feedback => MapFeedback(feedback),
            AuditLog auditLog => MapAuditLog(auditLog),
            CustomerServiceEntity customerService => MapCustomerService(customerService),
            _ => null
        };
    }

    private static SearchDocument MapUser(User user)
        => Build(
            "User",
            user.Id,
            user.FullName,
            $"{user.Username} - {user.Email} - {user.PhoneNumber}",
            $"{user.FullName} {user.Username} {user.Email} {user.PhoneNumber}",
            status: user.IsActive ? "Active" : "Inactive",
            userId: user.Id,
            createdAt: user.CreatedDate,
            updatedAt: user.ModifiedDate,
            metadata: new Dictionary<string, string?>
            {
                ["Email"] = user.Email,
                ["PhoneNumber"] = user.PhoneNumber,
                ["Username"] = user.Username
            });

    private static SearchDocument MapCustomer(Customer customer)
        => Build(
            "Customer",
            customer.Id,
            customer.FullName,
            $"{customer.MobileNumber} - {customer.CustomerType} - {customer.Address}",
            $"{customer.FullName} {customer.MobileNumber} {customer.CustomerType} {customer.Address}",
            customerId: customer.Id,
            userId: customer.UserId,
            createdAt: customer.CreatedDate,
            updatedAt: customer.ModifiedDate,
            metadata: new Dictionary<string, string?>
            {
                ["MobileNumber"] = customer.MobileNumber,
                ["CustomerType"] = customer.CustomerType,
                ["Address"] = customer.Address
            });

    private static SearchDocument MapAccount(Account account)
        => Build(
            "Account",
            account.Id,
            $"Account #{account.Id}",
            $"Customer {account.CustomerId}, balance {account.Balance:N0}, credit {account.CreditLimit:N0}",
            $"account customer {account.CustomerId} balance {account.Balance} credit {account.CreditLimit} status {account.Status}",
            customerId: account.CustomerId,
            amount: account.Balance,
            status: account.Status.ToString(),
            metadata: new Dictionary<string, string?>
            {
                ["CreditLimit"] = account.CreditLimit.ToString("0.##"),
                ["LastRechargeDate"] = account.LastRechargeDate?.ToString("O")
            });

    private static SearchDocument MapRechargePlan(RechargePlan plan)
        => Build(
            "RechargePlan",
            plan.Id,
            plan.PlanName,
            $"{plan.Amount:N0}, bonus {plan.BonusAmount:N0}, validity {plan.ValidityDays?.ToString() ?? "unlimited"} days",
            $"{plan.PlanName} {plan.Amount} {plan.BonusAmount} {plan.ValidityDays} {plan.Description}",
            amount: plan.Amount,
            status: plan.IsActive ? "Active" : "Inactive",
            metadata: new Dictionary<string, string?>
            {
                ["BonusAmount"] = plan.BonusAmount.ToString("0.##"),
                ["ValidityDays"] = plan.ValidityDays?.ToString()
            });

    private static SearchDocument MapService(ServiceEntity service)
        => Build(
            "Service",
            service.Id,
            service.ServiceName,
            $"{service.ServiceCode} - {service.Category} - {service.Price:N0}",
            $"{service.ServiceName} {service.ServiceCode} {service.Category} {service.DataAmountMB} {service.Price} {service.Description}",
            amount: service.Price,
            status: service.IsActive ? "Active" : "Inactive",
            metadata: new Dictionary<string, string?>
            {
                ["ServiceCode"] = service.ServiceCode,
                ["Category"] = service.Category,
                ["DataAmountMB"] = service.DataAmountMB.ToString(),
                ["ValidityDays"] = service.ValidityDays?.ToString()
            });

    private static SearchDocument MapTransaction(Transaction transaction)
        => Build(
            "Transaction",
            transaction.Id,
            transaction.TransactionRef,
            $"{transaction.TransactionType} - {transaction.Amount:N0} - {transaction.Status}",
            $"{transaction.TransactionRef} {transaction.TransactionType} {transaction.Amount} {transaction.Status} {transaction.PaymentMethod}",
            customerId: transaction.CustomerId,
            amount: transaction.Amount,
            status: transaction.Status.ToString(),
            createdAt: transaction.CreatedDate,
            updatedAt: transaction.ModifiedDate,
            metadata: new Dictionary<string, string?>
            {
                ["PaymentMethod"] = transaction.PaymentMethod.ToString(),
                ["PaymentId"] = transaction.PaymentId?.ToString(),
                ["CompletedAt"] = transaction.CompletedAt?.ToString("O")
            });

    private static SearchDocument MapPayment(Payment payment)
        => Build(
            "Payment",
            payment.Id,
            payment.PaymentRef,
            $"{payment.Amount:N0} - {payment.PaymentMethod} - {payment.Status}",
            $"{payment.PaymentRef} {payment.Amount} {payment.PaymentMethod} {payment.Status} bill {payment.BillId}",
            amount: payment.Amount,
            status: payment.Status.ToString(),
            createdAt: payment.CreatedDate,
            updatedAt: payment.ModifiedDate,
            metadata: new Dictionary<string, string?>
            {
                ["BillId"] = payment.BillId?.ToString(),
                ["PaymentDate"] = payment.PaymentDate.ToString("O"),
                ["PaymentMethod"] = payment.PaymentMethod.ToString()
            });

    private static SearchDocument MapPostpaidBill(PostpaidBill bill)
        => Build(
            "PostpaidBill",
            bill.Id,
            $"Postpaid bill #{bill.Id}",
            $"Customer {bill.CustomerId}, month {bill.BillMonth:yyyy-MM}, total {bill.TotalAmount:N0}, paid {bill.PaidAmount:N0}, status {bill.Status}",
            $"{bill.Id} {bill.CustomerId} {bill.BillMonth:yyyy-MM} {bill.TotalAmount} {bill.PaidAmount} {bill.Status}",
            customerId: bill.CustomerId,
            amount: bill.TotalAmount,
            status: bill.Status.ToString(),
            createdAt: bill.CreatedDate,
            updatedAt: bill.ModifiedDate,
            metadata: new Dictionary<string, string?>
            {
                ["BillMonth"] = bill.BillMonth.ToString("yyyy-MM"),
                ["DueDate"] = bill.DueDate.ToString("O"),
                ["PaidAmount"] = bill.PaidAmount.ToString("0.##")
            });

    private static SearchDocument MapCoupon(Coupon coupon)
        => Build(
            "Coupon",
            coupon.Id,
            coupon.Code,
            $"{coupon.Name} - {coupon.DiscountType} {coupon.DiscountValue}",
            $"{coupon.Code} {coupon.Name} {coupon.DiscountType} {coupon.DiscountValue} min {coupon.MinRechargeAmount}",
            userId: coupon.CreatedByUserId,
            amount: coupon.MinRechargeAmount,
            status: coupon.IsActive ? "Active" : "Inactive",
            metadata: new Dictionary<string, string?>
            {
                ["Name"] = coupon.Name,
                ["DiscountType"] = coupon.DiscountType.ToString(),
                ["DiscountValue"] = coupon.DiscountValue.ToString("0.##"),
                ["ValidFrom"] = coupon.ValidFrom.ToString("O"),
                ["ValidTo"] = coupon.ValidTo.ToString("O")
            });

    private static SearchDocument MapFeedback(Feedback feedback)
        => Build(
            "Feedback",
            feedback.Id,
            feedback.Subject,
            $"{feedback.Message} - rating {feedback.Rating} - {feedback.Status}",
            $"{feedback.Subject} {feedback.Message} rating {feedback.Rating} status {feedback.Status}",
            customerId: feedback.CustomerId,
            status: feedback.Status.ToString(),
            metadata: new Dictionary<string, string?>
            {
                ["Rating"] = feedback.Rating.ToString()
            });

    private static SearchDocument MapAuditLog(AuditLog auditLog)
        => Build(
            "AuditLog",
            auditLog.Id,
            auditLog.Action,
            $"{auditLog.EntityType} #{auditLog.EntityId} - {auditLog.Description}",
            $"{auditLog.Action} {auditLog.EntityType} {auditLog.EntityId} {auditLog.Description} {auditLog.IpAddress}",
            userId: auditLog.UserId,
            createdAt: auditLog.CreatedDate,
            updatedAt: auditLog.ModifiedDate,
            metadata: new Dictionary<string, string?>
            {
                ["EntityType"] = auditLog.EntityType,
                ["EntityId"] = auditLog.EntityId?.ToString(),
                ["IpAddress"] = auditLog.IpAddress,
                ["CreatedAt"] = auditLog.CreatedAt.ToString("O")
            });

    private static SearchDocument MapCustomerService(CustomerServiceEntity customerService)
        => Build(
            "CustomerService",
            customerService.Id,
            $"Customer service #{customerService.Id}",
            $"Customer {customerService.CustomerId}, service {customerService.ServiceId}, expires {customerService.ExpiresAt:yyyy-MM-dd}",
            $"{customerService.CustomerId} {customerService.ServiceId} {customerService.ActivatedAt} {customerService.ExpiresAt} auto renew {customerService.IsAutoRenewed}",
            customerId: customerService.CustomerId,
            createdAt: customerService.CreatedDate,
            updatedAt: customerService.ModifiedDate,
            metadata: new Dictionary<string, string?>
            {
                ["ServiceId"] = customerService.ServiceId.ToString(),
                ["ActivatedAt"] = customerService.ActivatedAt.ToString("O"),
                ["ExpiresAt"] = customerService.ExpiresAt.ToString("O"),
                ["IsAutoRenewed"] = customerService.IsAutoRenewed.ToString()
            });

    private static SearchDocument Build(
        string entityType,
        object entityId,
        string? title,
        string? summary,
        string? content,
        long? customerId = null,
        long? userId = null,
        decimal? amount = null,
        string? status = null,
        DateTimeOffset? createdAt = null,
        DateTimeOffset? updatedAt = null,
        Dictionary<string, string?>? metadata = null)
    {
        var stringId = entityId.ToString() ?? string.Empty;
        var safeTitle = title ?? $"{entityType} #{stringId}";
        var safeSummary = summary ?? string.Empty;
        var safeContent = content ?? string.Empty;

        return new SearchDocument
        {
            Id = $"{entityType.ToLowerInvariant()}:{stringId}",
            EntityType = entityType,
            EntityId = stringId,
            Title = safeTitle,
            Summary = safeSummary,
            Content = safeContent,
            Keywords = BuildKeywords(safeTitle, safeSummary, safeContent, status),
            CustomerId = customerId,
            UserId = userId,
            Amount = amount,
            Status = status,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            Metadata = metadata ?? new Dictionary<string, string?>()
        };
    }

    private static List<string> BuildKeywords(params string?[] values)
    {
        var separators = new[] { ' ', ',', '-', '_', '.', ':', ';', '/', '\\', '#' };

        return values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .SelectMany(value => value!.Split(separators, StringSplitOptions.RemoveEmptyEntries))
            .Select(value => value.Trim().ToLowerInvariant())
            .Where(value => value.Length >= 2)
            .Distinct()
            .Take(50)
            .ToList();
    }
}
