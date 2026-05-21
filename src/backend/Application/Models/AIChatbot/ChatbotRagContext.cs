namespace Application.Models.AIChatbot;

public sealed class ChatbotRagContext
{
    public long? CustomerId { get; set; }
    public string? CustomerSummary { get; set; }
    public string? AccountSummary { get; set; }
    public decimal? AccountBalance { get; set; }
    public decimal? CreditLimit { get; set; }
    public List<string> ActiveRechargePlans { get; set; } = new();
    public List<string> RecentTransactions { get; set; } = new();
    public List<string> UnpaidBills { get; set; } = new();
    public List<string> RetrievedSources { get; set; } = new();

    public string ToPromptContext()
    {
        var lines = new List<string>
        {
            "Nguồn dữ liệu RAG từ database Fabu:",
            CustomerSummary is null ? "- Khách hàng: Chưa có CustomerId hoặc không tìm thấy dữ liệu." : $"- Khách hàng: {CustomerSummary}",
            AccountSummary is null ? "- Tài khoản: Chưa có dữ liệu tài khoản." : $"- Tài khoản: {AccountSummary}",
            "- Gói nạp đang hoạt động:"
        };

        lines.AddRange(ActiveRechargePlans.Count == 0
            ? new[] { "  + Chưa có gói nạp đang hoạt động." }
            : ActiveRechargePlans.Select(plan => $"  + {plan}"));

        lines.Add("- Giao dịch gần đây:");
        lines.AddRange(RecentTransactions.Count == 0
            ? new[] { "  + Chưa có giao dịch gần đây trong dữ liệu truy xuất." }
            : RecentTransactions.Select(transaction => $"  + {transaction}"));

        lines.Add("- Hóa đơn trả sau chưa tất toán:");
        lines.AddRange(UnpaidBills.Count == 0
            ? new[] { "  + Không có hóa đơn chưa thanh toán trong dữ liệu truy xuất." }
            : UnpaidBills.Select(bill => $"  + {bill}"));

        lines.Add("- Hướng dẫn thanh toán: Khách hàng có thể thanh toán/nạp tiền qua các cổng đã tích hợp như VNPay, PayPal, Stripe hoặc số dư tài khoản nếu hệ thống cho phép.");
        lines.Add("- Khiếu nại: Ghi nhận nội dung khiếu nại, số điện thoại/CustomerId, mã giao dịch hoặc mã hóa đơn nếu có, rồi chuyển admin kiểm tra.");

        return string.Join(Environment.NewLine, lines);
    }
}
